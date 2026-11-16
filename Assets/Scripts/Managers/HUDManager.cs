using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    Health _playerHealth;
    //[SerializeField]
    //Slider _scoreText;
    //[SerializeField]
    //TextMeshProUGUI _healthText;

    [SerializeField]
    Slider _healthBar;
    [SerializeField]
    Slider _xpBar;
    int _requiredScore;
    float _healthBarMax;
    float _xpBarMax;
    int _maxScore;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitSlider();
        if (_playerHealth == null)
        {
            Debug.Log("UIManager: No player health found");
        }
    }
    private void OnEnable()
    {
        _playerHealth = GameManager.Instance.Player.GetComponent<Health>();

        _playerHealth.OnHealthChange += SetHealth;


        EventManager.AddListener<ScoreEvent>(e => SetCaptureScore(e.Score));
        EventManager.AddListener<LevelUpEvent>(OnLevelUpEvent);
    }
    private void OnDisable()
    {
        _playerHealth.OnHealthChange -= SetHealth;
        EventManager.RemoveListener<ScoreEvent>(e => SetCaptureScore(e.Score));
        EventManager.RemoveListener<LevelUpEvent>(OnLevelUpEvent);
    }

    void OnLevelUpEvent(LevelUpEvent e)
    {
        _xpBar.value = 0;
        _requiredScore = e.RequiredScore;
    }
    void SetCaptureScore(int score)
    {
        SetXpBar(score, _requiredScore);
        //SetScoreText(scorePerc);
    }
    void InitSlider()
    {
        _healthBarMax = _healthBar.maxValue;
        _healthBar.value = _healthBarMax;
        _xpBarMax = _xpBar.maxValue;
        _xpBar.value = 0;
        _requiredScore = ScoreManager.BaseXp;

    }
    void SetHealth(int currentHealth, int maxHP)
    {
        //_healthText.text = "Health:" + currentHealth.ToString();
        _healthBar.value = (ConvertToSliderValue(currentHealth, maxHP, _healthBarMax));
    }
    void SetXpBar(int score, int requiredScore)
    {
        float xpSlideValue = ConvertToSliderValue(score, requiredScore, _xpBarMax);
        //Debug.Log($"SlidValue: {xpSlideValue} Req {requiredScore}Current {score}");
        _xpBar.value = (ConvertToSliderValue(score, requiredScore, _xpBarMax));
    }
    [Button]
    void PopText(Transform text)
    {
        Tweens.TextPop(text);
    }
    float ConvertToSliderValue(int current, int max, float sliderMax)
    {
        if (current <= 0 || max <= 0)
            return 0;
            
        return Mathf.Ceil(current * sliderMax + max -1) / max;
    }
}
