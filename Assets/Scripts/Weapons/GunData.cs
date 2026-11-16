using UnityEngine;

[CreateAssetMenu(menuName = "Guns/GunData")]
public class GunData : ScriptableObject
{
    public int Damage;
    public int LevelMultiplier;
    public int FireRate;
    public AudioClip[] AudioClips;
    public GameObject MuzzleFlashPrefab;

}
