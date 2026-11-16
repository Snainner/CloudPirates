using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{
    [SerializeField]
    GameObject _mainMenu;
    [SerializeField]
    GameObject _ScoreText;
    [SerializeField]
    RectTransform _creditsRect;
    [SerializeField]
    GameObject _creditGo;
    [SerializeField]
    float _scoreTextTime = 1f;
    [SerializeField]
    float _speed = 10f;
    [SerializeField]
    float _incrementAmount = 50f;
    [SerializeField]
    float _incrementTime= .2f;

    Vector3 _initialTransform;
    Coroutine _scrollText;

    bool _finished = false;
    private void OnEnable()
    {
        _creditGo.SetActive(false);
        StartCoroutine(ScoreTextTimer());
    }
    [Button]
    void ResetCredits()
    {
        _finished = false;
        _ScoreText.SetActive(true);
        _creditsRect.SetPositionAndRotation(_initialTransform, Quaternion.identity);
        //StopAllCoroutines();
        //_scrollText= StartCoroutine(ScrollText(_incrementTime, _incrementAmount));
    }
    private void Update()
    {
        //_scrollRect.transform.position += _speed * Vector3.up * Time.deltaTime;
    }
    IEnumerator ScoreTextTimer()
    {
        yield return new WaitForSeconds(_scoreTextTime);
        _ScoreText.SetActive(false);
        yield return null;
        _scrollText = StartCoroutine(ScrollText(_incrementTime, _incrementAmount));
    }
    IEnumerator ScrollText(float incrementTime, float incrementAmount)
    {
        _creditGo.SetActive(true);
        _initialTransform = _creditsRect.position;

        float currentPos = 0f;
        while(currentPos < _creditsRect.rect.height)
        {
            _creditsRect.transform.position += incrementAmount * Vector3.up;
            currentPos += incrementAmount;
            yield return new WaitForSeconds(incrementTime);

        }
        yield return new WaitForSeconds(2f);
        _mainMenu.SetActive(true);
        ResetCredits();
        gameObject.SetActive(false);
    }
}
