using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethods
{
    public static void SetFill(this Slider slider, float normalizedValue)
    {
        slider.value = Mathf.Clamp01(normalizedValue);
    }
}
