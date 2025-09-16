using UnityEngine;

public class Colectables : MonoBehaviour
{
    public void Collect(Collectible.Type type)
    {
        var stats = GetComponentInParent<PlayerStats>(); // en el Player
        if (stats == null) return;

        switch (type)
        {
            case Collectible.Type.Coin:
                stats.AddCoin(1);
                Debug.Log("Monedas: " + stats.Coins);
                break;
            case Collectible.Type.Gasoline:
                stats.AddFuel(10f);
                Debug.Log("Gasolina: " + stats.Fuel);
                break;
            case Collectible.Type.Person:
                stats.AddRescued(1);
                Debug.Log("Personas rescatadas: " + stats.PeopleRescued);
                break;
        }
    }
}