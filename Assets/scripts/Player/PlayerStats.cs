using Simplon;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("Fuel")]
    [SerializeField] float maxFuel = 200f;
    public float Fuel { get; private set; }

    [Header("Escudo")]
    public bool tieneEscudo;

    [Header("Counters")]
    public int Coins { get; private set; }
    public int PeopleRescued { get; private set; }

    [Header("Events")]
    public UnityEvent<float,float> onFuelChanged; // (current, max)
    public UnityEvent<int> onCoinsChanged;
    public UnityEvent<int> onPeopleChanged;

    void Awake()
    {
        Fuel = maxFuel; // inicial
        onFuelChanged?.Invoke(Fuel, maxFuel);
        onCoinsChanged?.Invoke(Coins);
        onPeopleChanged?.Invoke(PeopleRescued);
    }

    public void AddFuel(float amount)
    {
        Fuel = Mathf.Clamp(Fuel + amount, 0f, maxFuel);
        onFuelChanged?.Invoke(Fuel, maxFuel);
    }

    public void SpendFuel(float amount)
    {
        if (amount <= 0f) return;

        // Si tiene Escudo activo: no recibe daño
        if (tieneEscudo)
        {
            Debug.Log("[STATS] Daño bloqueado: escudo activo.");
            return;
        }

        Fuel = Mathf.Max(0f, Fuel - amount);
        onFuelChanged?.Invoke(Fuel, maxFuel);
        if (Fuel <= 0f)
        {
            
            Debug.Log("[GAME] Sin combustible -> Game Over");
            // GameControler.Instance.pasarNivel("loseScene");
            // GameControler.Instance.ResetVariables();
            
        };
    }

    public void AddCoin(int amount = 1)
    {
        Coins += Mathf.Max(0, amount);
        onCoinsChanged?.Invoke(Coins);
    }

    public void AddRescued(int amount = 1)
    {
        PeopleRescued += Mathf.Max(0, amount);
        onPeopleChanged?.Invoke(PeopleRescued);
    }

    public float GetMaxFuel() => maxFuel;
}