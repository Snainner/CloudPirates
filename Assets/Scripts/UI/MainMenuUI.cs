using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    //[SerializeField]
    //GameObject _rootPanel;
    [SerializeField]
    AdvancedPlaneController _player;
    [SerializeField]
    GameObject _CreditsPanel;
    [SerializeField]
    Button _playButton;
    [SerializeField]
    Button _creditsButton;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        

        _playButton.onClick.AddListener(PlayBtn);
        _creditsButton.onClick.AddListener(CreditBtn);
    }
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
        _playButton.onClick.RemoveListener(PlayBtn);
        _creditsButton.onClick.RemoveListener(CreditBtn);

    }

    void PlayBtn()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartEnemyTimer();

    }

    void CreditBtn()
    {
        gameObject.SetActive(false);
        _CreditsPanel.SetActive(true);
    }
}
