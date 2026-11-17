using UnityEngine;

[CreateAssetMenu(menuName = "Audio/EngineSound")]
public class EngineAudio : ScriptableObject
{
    public AudioClip Clip;
    public Vector2 Pitch;
    public Vector2 Volume;
    public float AudioSmooth;

    public void StartEngineSound(AudioSource audioSource)
    {
        audioSource.clip = Clip;
        audioSource.loop = true;
        audioSource.time = Random.Range(0, Clip.length);

        audioSource.Play();
    }
    public void StopEngineSound(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    public void UpdateEngineSound(AudioSource audioSource, float currentSpeed, float maxSpeed, float minSpeed)
    {
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);

        float targetPitch = Mathf.Lerp(Pitch.x, Pitch.y, t);
        float targetVolume = Mathf.Lerp(Volume.x, Volume.y, t);

        audioSource.pitch = Mathf.Lerp(audioSource.pitch, targetPitch, AudioSmooth * Time.deltaTime);
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, AudioSmooth * Time.deltaTime);
    }
}
