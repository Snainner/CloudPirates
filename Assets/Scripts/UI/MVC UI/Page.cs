using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof(AudioSource), typeof(CanvasGroup))]
[DisallowMultipleComponent]
public class Page : MonoBehaviour
{
    AudioSource _audioSource;
    RectTransform _rectTransform;
    CanvasGroup _canvasGroup;

    [SerializeField]
    float _animationSpeed = 1f;
    public bool ExitOnNewPage = false;
    [SerializeField]
    AudioClip _entryClip;
    [SerializeField]
    AudioClip _exitClip;

    Coroutine _audioRoutine;

    UnityEvent _prePushAction;
    UnityEvent _postPushAction;
    UnityEvent _prePopAction;
    UnityEvent _postPopAction;


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _audioSource = GetComponent<AudioSource>();

        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        _audioSource.spatialBlend = 0f;
    }
    public void Enter(bool playAudio)
    {
        _prePushAction?.Invoke();
        if(playAudio)
            PlayEntryClip(playAudio);

    }
    public void Exit()
    {
        _prePopAction?.Invoke();
    }
    void PlayEntryClip(bool playAudio)
    {
        if (playAudio && _entryClip != null && _audioSource != null)
        {
            if (_audioRoutine != null)
            {
                StopCoroutine(_audioRoutine);
            }
            _audioRoutine = StartCoroutine(PlayClip(_entryClip));
        }
    }
    void PlayExitClip(bool playAudio)
    {
        if (playAudio && _exitClip != null && _audioSource != null)
        {
            if (_audioRoutine != null)
            {
                StopCoroutine(_audioRoutine);
            }
            _audioRoutine = StartCoroutine(PlayClip(_exitClip));
        }
    }

    IEnumerator PlayClip(AudioClip clip)
    {
        _audioSource.enabled = true;
        WaitForSeconds wait = new WaitForSeconds(clip.length);
        _audioSource.PlayOneShot(clip);
        yield return wait;

        //Disable the audiosource again to prevent extra cpu overhead (even if small)
        _audioSource.enabled = false;
    }
}
