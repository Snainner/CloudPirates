using TMPro;
using PrimeTween;
using UnityEngine;
using UnityEngine.UnityConsent;
public static class Tweens 
{
    const float _defaultTime = .25f;
    const float _popTime = .75f;
    const float _popScaleMultiplier = 1.5f;
    const float _shakeStrength = 20;
    public static void TextPop(Transform text)
    {
        var normalScale = text.localScale;
        var popScale = text.localScale * _popScaleMultiplier;

        Shake(text);
        Tween.Scale(text, endValue: popScale,duration:_popTime ,ease: Ease.OutElastic, cycleMode: CycleMode.Yoyo, cycles: 1, useUnscaledTime: false).OnComplete(() => SimpleScale(text, normalScale));
    }

    static void SimpleScale(Transform text, Vector3 scale)
    {
        Tween.Scale(text, endValue: scale, duration: _defaultTime, ease: Ease.OutCubic, cycleMode: CycleMode.Yoyo, cycles: 1, useUnscaledTime: false);
    }

    public static void Shake(Transform text)
    {
        var currentRot = text.localRotation;

        Tween.ShakeLocalRotation(text, Vector3.one * 20, _popTime);
    }

}
