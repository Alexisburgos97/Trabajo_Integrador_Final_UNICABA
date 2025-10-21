using UnityEngine;
using System.Collections;

public class FracturedObject : MonoBehaviour
{
    public GameObject originalObject;
    public GameObject fracturedObject;
    public GameObject explosionVFX;
    public float explosionMinForce = 5;
    public float explosionMaxForce = 10;
    public float explosionForceRadius = 10;
    public float fragScaleFactor = 1;
    [SerializeField] float _tiempo_destrucion=7f;

    private GameObject fractObj;
    private bool isExploded = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Explode();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
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
