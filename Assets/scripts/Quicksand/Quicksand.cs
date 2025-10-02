using UnityEngine;
using Simplon;

[RequireComponent(typeof(BoxCollider))]
public class Quicksand : MonoBehaviour
{
    [Header("Consumo")]
    [SerializeField] float enterPenalty = 5f;
    [SerializeField] float drainPerSecond = 5f;

    [Header("Feedback opcional")]
    [SerializeField] bool slowPlayer = true;
    [SerializeField] float extraDrag = 3f;

    GameControler _gc;
    float _originalDrag;
    bool _hasOriginalDrag = false;

    void Awake()
    {
        _gc = GameControler.Instance ?? FindAnyObjectByType<GameControler>();
        if (_gc == null) { Debug.LogError("[ArenaMovediza] No hay GameControler."); enabled = false; return; }

        var box = GetComponent<BoxCollider>();
        box.isTrigger = true;

        var meshCol = GetComponent<MeshCollider>();
        if (meshCol) meshCol.convex = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (enterPenalty > 0f)
            _gc.Combustible = Mathf.Max(0f, _gc.Combustible - enterPenalty);

        if (slowPlayer)
        {
            var rb = other.attachedRigidbody ?? other.GetComponentInParent<Rigidbody>();
            if (rb)
            {
                if (!_hasOriginalDrag) { _originalDrag = rb.linearDamping; _hasOriginalDrag = true; }
                rb.linearDamping = _originalDrag + extraDrag;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (drainPerSecond > 0f)
            _gc.Combustible = Mathf.Max(0f, _gc.Combustible - drainPerSecond * Time.deltaTime);

        if (_gc.Combustible <= 0f) { _gc.QuitarVida(1); _gc.ResetCombustible(); }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (slowPlayer)
        {
            var rb = other.attachedRigidbody ?? other.GetComponentInParent<Rigidbody>();
            if (rb && _hasOriginalDrag) rb.linearDamping = _originalDrag;
        }
    }
}
