using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(GunItem))]
public class MuzzleFlashEffect : MonoBehaviour
{
    VisualEffect _effect;
    GunItem _gun;

    private void Awake()
    {
        _gun = GetComponent<GunItem>();
        //_effect = Instantiate(_gun._gunData.MuzzleFlashPrefab, _gun.transform, false).GetComponent<VisualEffect>();
        //_effect.Stop();
    }

    private void OnEnable()
    {
        _gun.OnFired += Play;
    }
    private void OnDisable()
    {
        _gun.OnFired -= Play;
    }

    [Button]
    void Play()
    {
       // _effect.Play();
    }
}
