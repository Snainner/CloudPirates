using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class DebugTools : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _dragText;

    public void SetDragText(string text)
    {
        _dragText.text = text;
    }

    public static TextMeshProUGUI AddDebugText(string text, RectTransform uiPanel)
    {
        TextMeshProUGUI textObj = new TextMeshProUGUI();
        textObj.text = text;
        textObj.fontSize = 18;
        textObj.color = Color.black;
        textObj.transform.SetParent(uiPanel, false);

        return textObj;
    }
}
