using System;
using UnityEngine;

[Serializable]
public struct Stat
{
    public float BaseValue;
    public float Bonus;
    public float Value => BaseValue * (1f + Bonus);

    public Stat(float baseValue)
    {
        BaseValue = baseValue;
        Bonus = 0f;
    }
}
[Serializable]
public struct HealthSettings
{
    public int Health;
    public float Regen;
}
[Serializable]
public struct PlayerSettings
{
    public HealthSettings Health;
    public Stat Damage;
    public PlaneSettings PlaneSettings;
    public CameraSettings CameraSettings;
}

[Serializable]
public struct PlaneSettings
{
    public float MaxThrust;
    public float ThrottleIncrement;
    public float RollModifier;
    public float Responsiveness;
}

[Serializable]
public struct CameraSettings
{
    [Range(0.7f, 1f)]
    public float CamSpring;
    public float Distance;
    public Vector3 Offset;
    public float FoVMultiplier;
    [Header("Camera Roll")]
    //public bool RollEnabled;
    [Range(0f, 90f)]
    public float MaxRotation;
    public float RotationSharpness;
    public bool ZeroRollBeyondLimit;
    [Header("Camera Shake")]
    public bool ShakeEnabled;
    public float Frequency;
    [Range(0f, .5f)]
    public float Intensity;
}

[Serializable]
public struct GameSettings
{
    public float RequiredScoreMultiplier;
    public float BaseRequiredScore;
    public EnemySettings EnemySettings;

}

public struct EnemySettings
{
    public HealthSettings Health;
    public float Damage;
    public int RayDistance;
    [Header("Movement")]
    public float MaxSpeed;
    public float Acceleration;
    public float CatchUpBonus;
    [Header("Turning")]
    public float TurnRate;
    public float MaxBank;
    public float BankSmooth;
}