using UnityEngine;

[RequireComponent (typeof(GunItem))]
public class GunAudio : MonoBehaviour
{
    GunItem _gun;

    [SerializeField]
    AudioSource _source;
    [SerializeField]
    AudioClip[] _clips;
    [SerializeField]
    Vector2 _pitchRange = new(0.95f, 1.05f);

    private void Awake()
    {
        _gun = GetComponent<GunItem>();
        _source = GetComponent<AudioSource>();
        _clips = _gun._gunData.AudioClips;
    }

    private void OnEnable()
    {
        _gun.OnFired += Play;
    }
    private void OnDisable()
    {
        _gun.OnFired -= Play;
    }
    void Play()
    {
        _source.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
        _source.PlayOneShot(_clips[Random.Range(0, _clips.Length)]);
    }
}
