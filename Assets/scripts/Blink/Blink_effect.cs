using UnityEngine;
using System.Collections;

public class BlinkingObject : MonoBehaviour
{
    public float blinkInterval = 0.5f; // Time between blinks
    private Renderer objectRenderer; // Or Light component, SpriteRenderer, etc.

    void Start()
    {
        objectRenderer = GetComponent<Renderer>(); // Get the component to enable/disable
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        while (true) // Loop indefinitely
        {
            objectRenderer.enabled = !objectRenderer.enabled; // Toggle enable state
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}