using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    [SerializeField]
    AudioSource _source;
    [SerializeField]
    AudioClip _clip;
    [SerializeField]
    Vector2 _pitch;
    [SerializeField]
    Vector2 _volume;
    [SerializeField]
    float _audioSmooth;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        if (_source == null )
            _source = GetComponentInChildren<AudioSource>();
    }
    private void Start()
    {
        //Random start offset to preven phasing
        _source.time = Random.Range(0, 1f);
        _source.Play();
    }

    public void UpdateEngineSound(float currentSpeed, float maxSpeed, float minSpeed)
    {
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);

        float targetPitch = Mathf.Lerp(_pitch.x, _pitch.y, t);
        float targetVolume = Mathf.Lerp(_volume.x, _volume.y, t);   

        _source.pitch = Mathf.Lerp(_source.pitch, targetPitch, _audioSmooth * Time.deltaTime);
        _source.volume = Mathf.Lerp(_source.volume, targetVolume, _audioSmooth * Time.deltaTime);
    }
}
