Shader "Custom/TornadoFunnelSimple"
{
    Properties
    {
        _MainTex ("Noise Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 0.96, 0, 1)
        _Alpha ("Alpha", Range(0,1)) = 0.6
        _Speed ("Speed", Float) = 1.0
        _FunnelWidth ("Funnel Width", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float height : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Alpha;
            float _Speed;
            float _FunnelWidth;
            
            v2f vert(appdata v)
            {
                v2f o;
                
                float3 pos = v.vertex.xyz;
                float height = (pos.y + 0.5); // 0 abajo, 1 arriba
                
                // Forma de embudo
                float funnel = lerp(1.0 - _FunnelWidth, 1.0, height);
                
                // Rotación
                float angle = _Time.y * _Speed + height * 6.28 * 3.0;
                float s = sin(angle);
                float c = cos(angle);
                
                float x = pos.x * funnel;
                float z = pos.z * funnel;
                
                pos.x = x * c - z * s;
                pos.z = x * s + z * c;
                
                o.pos = UnityObjectToClipPos(pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.y += _Time.y * _Speed * 0.5;
                o.height = height;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                // Color
                fixed3 col = _Color.rgb * tex.r;
                
                // Fresnel
                float fresnel = 1.0 - saturate(dot(i.worldNormal, i.viewDir));
                fresnel = pow(fresnel, 3.0);
                col += fresnel * float3(1, 1, 0.5) * 0.5;
                
                // Alpha
                float alpha = _Alpha * tex.r * i.height;
                
                return fixed4(col, alpha);
            }
            ENDCG
        }
    }
    
    FallBack "Transparent/Diffuse"
}