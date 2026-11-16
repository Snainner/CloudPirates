using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradable 
{
    void Upgrade(float addModifier);
    UpgradeData[] UpgradableStats();

}
