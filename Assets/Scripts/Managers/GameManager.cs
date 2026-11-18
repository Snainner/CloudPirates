using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerManager Player { get; private set; }
   //public AdvancedPlaneController PlaneController { get; private set; }
    public ScoreManager ScoreManager{ get; private set; }

    [SerializeField]
    GameObject _enemy;
    public bool InGame = false;
    
    private void Awake()
    {
        _enemy.SetActive(false);
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(Instance);
            return;
        }

        Player = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
        if (Player == null)
            Debug.Log("<color=red><b>[GameManager]</b> Unable to find player by tag</color>");
        //PlaneController = Player.Plane;

        ScoreManager = GetComponent<ScoreManager>();
        if(ScoreManager == null)
        {
            Debug.Log("GameManager: Score manager not found on manager component");
        }

    }

    public void StartEnemyTimer()
    {
        StartCoroutine(ActivateEnemyTimer());   
    }
    

    public IEnumerator ActivateEnemyTimer()
    {
        yield return new WaitForSeconds(30f);
        _enemy.SetActive(true);
    }

}
