Shader "Custom/URP_TornadoFunnel"
{
    Properties
    {
        _MainTex ("Main Spiral Texture (RGBA)", 2D) = "white" {}
        _NoiseTex ("Noise Texture (R)", 2D) = "white" {}
        _Color ("Tint Color", Color) = (0.87,0.72,0.45,1) // arena dorada
        _Opacity ("Opacity", Range(0,1)) = 0.85
        _Height ("Height (world units)", Float) = 4.0
        _BaseRadius ("Radius Bottom (m)", Float) = 0.2
        _TopRadius ("Radius Top (m)", Float) = 1.5
        _VortexStrength ("Vortex Strength", Range(0,10)) = 2.0
        _SpinSpeed ("Spin Speed", Float) = 0.6
        _NoiseScale ("Noise Scale", Float) = 3.0
        _NoiseSpeed ("Noise Speed", Float) = 0.8
        _EdgeFade ("Edge Fade", Range(0,1)) = 0.35
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        Pass
        {
            Name "FORWARD"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "UnityCG.cginc"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            float4 _Color;
            float _Opacity;
            float _Height;
            float _BaseRadius;
            float _TopRadius;
            float _VortexStrength;
            float _SpinSpeed;
            float _NoiseScale;
            float _NoiseSpeed;
            float _EdgeFade;

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float height01 : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // helper: rotate 2D
            float2 rotate2D(float2 uv, float angle)
            {
                float ca = cos(angle);
                float sa = sin(angle);
                return float2(uv.x * ca - uv.y * sa, uv.x * sa + uv.y * ca);
            }

            Varyings vert(Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);

                // world pos of vertex
                float3 worldPos = mul(GetObjectToWorldMatrix(), float4(v.positionOS,1)).xyz;

                // compute normalized height along the cylinder axis (we assume cylinder's local Y is up)
                // first get local position in object space Y
                float localY = v.positionOS.y; // works for standard cylinder aligned with Y

                // Map localY (which may be from -h/2 to +h/2) into 0..1 along object height
                // We'll assume cylinder mesh is from 0..1 in Y; if not, user may need to reposition/scale.
                // To make it robust, we use _Height to normalize.
                float h = max(0.0001, _Height);
                float y01 = saturate((localY + h*0.5) / h);

                // radius interpolation: narrower at bottom (y01=0) and wider at top (y01=1)
                float radius = lerp(_BaseRadius, _TopRadius, pow(y01, 0.9));

                // compute outward direction in object space (assume mesh originally at unit radius)
                float2 dir = float2(v.positionOS.x, v.positionOS.z);
                float currentRadius = length(dir);
                float2 dirNorm = (currentRadius > 0.0001) ? dir / currentRadius : float2(1,0);

                // scale vertices radially to match funnel shape
                float scale = (radius / max(currentRadius, 0.0001));
                float3 posOS = v.positionOS;
                posOS.xz = posOS.xz * scale;

                // swirl deformation: rotate vertices around Y depending on height (to create spiral shape)
                float swirl = _VortexStrength * (1.0 - y01); // more swirl near the bottom to convey motion
                float angle = swirl * (1.0 - y01) * 3.14159 * (_SpinSpeed);
                float2 rotated = rotate2D(posOS.xz, angle);
                posOS.xz = rotated;

                // output world pos after vertex deformation
                float4 worldPosNew = mul(GetObjectToWorldMatrix(), float4(posOS,1));
                o.worldPos = worldPosNew.xyz;

                // compute UVs for texturing the funnel: use polar coords around Y
                float theta = atan2(posOS.z, posOS.x); // -pi..pi
                float r = length(posOS.xz);

                // we want a u that spirals around, so include angle + time spin
                float time = _Time.y; // _Time.y is seconds
                float u = (theta / (2.0 * 3.14159)) + time * _SpinSpeed * 0.1;
                u = frac(u);

                // v coordinate tracks height
                float vcoord = y01;

                o.uv = float2(u, vcoord);
                o.height01 = y01;

                // transform to clip space
                float4 posCS = TransformObjectToHClip(posOS);
                o.positionCS = posCS;

                return o;
            }

            half4 sampleSpiral(float2 uv, float height01)
            {
                // base spiral texture sample
                float2 uvSpiral = uv;
                // add radial fade so edges are soft
                float edge = smoothstep(1.0 - _EdgeFade, 1.0, abs(uv.x - 0.5) * 2.0);

                // sample noise to distort the uv
                float2 noiseUV = uv * _NoiseScale + float2(0, _Time.y * _NoiseSpeed);
                float n = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;

                // distort vertical coordinate by noise and height
                uvSpiral.y += (n - 0.5) * 0.25 * (1.0 - height01);

                // rotate uv around center depending on height to increase swirl
                float rot = _SpinSpeed * _Time.y * (1.0 - height01) * 0.5;
                uvSpiral = rotate2D(uvSpiral - 0.5, rot) + 0.5;

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvSpiral);

                // apply noise-driven alpha variation
                float alphaNoise = n * 0.6 + 0.4;

                // combine with radial fade (so outer parts are more transparent)
                float radial = 1.0 - smoothstep(0.0, 0.95, abs(uv.x - 0.5) * 2.0);

                col.a *= alphaNoise * radial;

                return col;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // base sampling
                half4 tex = sampleSpiral(i.uv, i.height01);

                // tint to desert cartoon color
                half4 tint = _Color;
                tex.rgb *= tint.rgb;

                // global opacity control
                tex.a *= _Opacity;

                // slightly fade towards top (less dense at top)
                tex.a *= lerp(1.0, 0.5, i.height01);

                // premultiply alpha for correct blending
                tex.rgb *= tex.a;

                return tex;
            }

            ENDHLSL
        }
    }

    FallBack "Unlit/Transparent"
}