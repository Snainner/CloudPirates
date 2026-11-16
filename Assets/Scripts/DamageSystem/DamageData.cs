using System;
using UnityEngine;

public readonly struct DamageData
{
    public readonly int Amount;
    public readonly Component Source;

    public DamageData(int amount, Component source)
    {
        Amount = amount;
        Source = source;
    }
}
