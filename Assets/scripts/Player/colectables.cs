using UnityEngine;

public class colectables : MonoBehaviour
{

    public int coins = 0;
    public float fuel = 0;
    public int peopleRescued = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void Collect(Collectible.Type type)
    {
        if (type == Collectible.Type.Coin)
        {
            coins++;
            Debug.Log("Monedas: " + coins);
        }
        if (type == Collectible.Type.Gasoline)
        {
            fuel += 10f; // por ejemplo
            Debug.Log("Gasolina: " + fuel);
        }
       if (type == Collectible.Type.Person)
        {
            peopleRescued++;
            Debug.Log("Personas rescatadas: " + peopleRescued);
        }
    }
}
