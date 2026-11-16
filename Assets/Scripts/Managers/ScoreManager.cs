using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    float _xpNeededMultiplier = 1.1f;
    public int Score => _currentXp;
    public int CurrentLevel { get; private set; }
    
    int _currentXp;
    int _requiredXp;
    
    public const int BaseXp = 200;


    private void Awake()
    {
        CurrentLevel = 1;
        _requiredXp = Mathf.CeilToInt(LevelMultiplier(BaseXp, _xpNeededMultiplier, CurrentLevel + 1));
    }
    //public bool LevelUp => _score % 100 == 0 && _score != 0 ? true : false;
    //ScoreEvent _scoreEvent = new ScoreEvent();

    private void OnEnable()
    {
        EventManager.AddListener<CaptureEvent>(e => OnCapture());
        
    }
    private void OnDisable()
    {
        EventManager.RemoveListener<CaptureEvent>(e => OnCapture());
    }
    void OnCapture()
    {
        if(!UIManager.Instance.IsUpgradeUIActive)
            _currentXp++;
        //_scorePerc = ((float)_score / (float)_maxScore) * 100;

        
        EventManager.Broadcast(new ScoreEvent { Score = _currentXp});
        if(_currentXp == _requiredXp || UIManager.Instance.IsUpgradeUIActive)
        {
            CurrentLevel++;
            _requiredXp = Mathf.CeilToInt(LevelMultiplier(BaseXp, _xpNeededMultiplier, CurrentLevel + 1));
            
            EventManager.Broadcast(new LevelUpEvent{ Level = CurrentLevel, RequiredScore = _requiredXp});
        }

        //Debug.Log($"Score Added {_score}");
    }

    public static float LevelMultiplier(int baseXP, float xpIncrease, int nextLevel)
    {
        return baseXP * xpIncrease * (nextLevel - 1);
    }


}
