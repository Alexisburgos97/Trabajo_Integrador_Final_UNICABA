using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum Type { Coin, Gasoline, Person }   // agregamos Person
    public Type type;

    private void OnTriggerEnter(Collider other)
    {
        var car = other.GetComponent<colectables>();
        if (car == null) return;

        car.Collect(type);
        Destroy(gameObject);
    }
}