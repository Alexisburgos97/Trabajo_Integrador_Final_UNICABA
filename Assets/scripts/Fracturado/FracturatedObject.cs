using UnityEngine;
using System.Collections;

public class FracturedObject : MonoBehaviour
{
    [SerializeField] GameObject originalObject;
    [SerializeField] GameObject fracturedObject;
    [SerializeField] GameObject explosionVFX;
    [SerializeField] float explosionMinForce = 5;
    [SerializeField] float explosionMaxForce = 10;
    [SerializeField] float explosionForceRadius = 10;
    [SerializeField] float fragScaleFactor = 1;
    [SerializeField] float _tiempo_destrucion = 7f;
    [SerializeField] AudioSource _explosion;
    [SerializeField] float _tiempo_reset = 30f;
    [SerializeField] private bool _usarUnaVez = true;    // Solo se activa una vez        
    private GameObject fractObj;
    private bool isExploded = false;
    float _espera=0;
    bool _activado = false;

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Explode();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
        */
        espera_respown();
    }

    // Este m�todo se llama autom�ticamente cuando ocurre una colisi�n f�sica
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_activado)
        {
            //Destroy(gameObject); // Destruye el objeto que tiene este script
            if (_explosion != null)
            {
                _explosion.Play();
            }
            Explode();
            if (_usarUnaVez)
                _activado = true;
            else
            {
                _activado = true;
                _espera = _tiempo_reset;
            }
        }
    }

    // Si el objeto usa colliders con "isTrigger" activado, usamos este otro m�todo
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_activado)
        {
            //Destroy(gameObject);
             if (_explosion != null)
            {
                _explosion.Play();
            }
            Explode();
            if (_usarUnaVez)
                _activado = true;
            else
            {
                _activado = true;
                _espera = _tiempo_reset;
            }
        }
    }
    void Explode()
    {
        if (isExploded || originalObject == null) return;
        isExploded = true;

        originalObject.SetActive(false);

        if (fracturedObject != null)
        {
            fractObj = Instantiate(fracturedObject, originalObject.transform.position, originalObject.transform.rotation);

            foreach (Transform t in fractObj.transform)
            {
                Rigidbody rb = t.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(
                        Random.Range(explosionMinForce, explosionMaxForce),
                        originalObject.transform.position,
                        explosionForceRadius
                    );

                    StartCoroutine(Shrink(t, 2f));
                }
            }

            // destruir fragmentos después de un tiempo razonable
            
            Destroy(fractObj, _tiempo_destrucion);
        }

        if (explosionVFX != null)
        {
            GameObject explovFX = Instantiate(explosionVFX, originalObject.transform.position, Quaternion.identity);
            Destroy(explovFX, _tiempo_destrucion);
        }
        //si solo se usa una vez el objeto se destruye
        if (_usarUnaVez)
        {
            Destroy(originalObject);
            //destruir el contenedor tambien
            Destroy(gameObject);
        }
    }

    void Reset()
    {
        StopAllCoroutines(); // 🔹 cancela shrink en curso

        if (fractObj != null)
        {
            Destroy(fractObj);
        }

        if (originalObject != null)
        {
            originalObject.SetActive(true);
        }

        isExploded = false;
    }

    void espera_respown()
    {
        if (!_usarUnaVez)
        {
            if (_espera > 0)
            {
                _espera -= Time.deltaTime;
            }
            else
            {
                if (_activado)
                {
                    Reset();
                }
                _activado = false;
                
            }
        }
    }

    IEnumerator Shrink(Transform t, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (t == null) yield break;

        Vector3 newScale = t.localScale;

        while (newScale.x > 0 && t != null)
        {
            newScale -= new Vector3(fragScaleFactor, fragScaleFactor, fragScaleFactor) * Time.deltaTime;

            if (t != null)
                t.localScale = newScale;

            yield return new WaitForSeconds(0.05f);
        }
    }
}
