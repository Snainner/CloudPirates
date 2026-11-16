using UnityEngine;

public class ShootingSight : MonoBehaviour
{
    BoxCollider _collider;
    public bool IsAttacking = false;
    public event System.Action OnTrigger;
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IsAttacking = true;
        OnTrigger?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        IsAttacking = false;
        OnTrigger?.Invoke();
    }

}
