using UnityEngine;

public class PlaneCrosshair : MonoBehaviour
{
    [SerializeField]
    RectTransform _crosshair;
    [SerializeField] 
    RectTransform _leadIndicator;
    [SerializeField]
    Transform _gunMuzzle;
    [SerializeField]
    float _aimDist = 800f;

    private void FixedUpdate()
    {
        AdjustCrosshair();
    }
    public void AdjustCrosshair()
    {
        Vector3 aimPoint = _gunMuzzle.position + _gunMuzzle.forward * _aimDist;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(aimPoint);
        _crosshair.position = screenPos;
    }
}
