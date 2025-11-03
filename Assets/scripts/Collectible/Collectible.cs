using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum Type { Coin, Gasoline, Person }   // agregamos Person
    public Type type;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("interaccion colecctable");
        var car = other.GetComponent<Colectables>();
        Debug.Log(car);
        if (car == null) return;

        car.Collect(type);
        Destroy(gameObject);
    }
}