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

    private GameObject fractObj;

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
        if (originalObject != null)
        {
            originalObject.SetActive(false);

            if (fracturedObject != null)
            {
                fractObj = Instantiate(fracturedObject) as GameObject;

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

                        StartCoroutine(Shrink(t, 2));
                    }
                }

                Destroy(fractObj, 5);
            }

            if (explosionVFX != null)
            {
                GameObject explovFX = Instantiate(explosionVFX) as GameObject;
                Destroy(explovFX, 7);
            }
        }
    }

    void Reset()
    {
        Destroy(fractObj);
        originalObject.SetActive(true);
    }

    IEnumerator Shrink(Transform t, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 newScale = t.localScale;

        while (newScale.x >= 0)
        {
            newScale -= new Vector3(fragScaleFactor, fragScaleFactor, fragScaleFactor);
            t.localScale = newScale;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
