using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using UnityEngine;


public abstract class Upgradable : MonoBehaviour 
{
    UpgradeData[] cache;
    protected abstract UpgradeData[] BuildUpgrades();
    public IReadOnlyList<UpgradeData> Upgrades => cache ??= BuildUpgrades();

}
public abstract class UpgradeData
{
    //To keep track of all upgrades the component might have
    public string Id;
    //The name of the variable being changed
    public string Name;
    public abstract void Roll();
    public abstract string Preview { get; }
    public abstract void Apply(Component target);
}

public class UpgradeData<TComp> : UpgradeData where TComp : Component 
{
    readonly Func<TComp, float> getter;
    readonly Action<TComp, float> setter;
    readonly float minPercent, maxPercent;
    float rolled;   // e.g. 12.5 means +12.5%

    public UpgradeData(string id, string name,
        Func<TComp, float> getter, Action<TComp, float> setter,
        float minPercent, float maxPercent)
    {
        Id = id; Name = name;
        this.getter = getter; this.setter = setter;
        this.minPercent = minPercent; this.maxPercent = maxPercent;
    }

    public float Rolled => rolled;

    public override void Roll() => rolled = UnityEngine.Random.Range(minPercent, maxPercent);

    public override string Preview => $"{Name} +{rolled:0.#}%";

    public override void Apply(Component target)
    {
        var comp = (TComp)target;
        setter(comp, getter(comp) + Rolled / 100f);
    }
}

public static class UpgradeFactory
{
    public static UpgradeData Create<TComp>(string id, string name, Expression<Func<TComp, float>> field, 
        float minPercent = UpgradeConstants.MinUpgradePerc, float maxPercent= UpgradeConstants.MaxUpgradePerc) where TComp : Component
    {
        var member = (MemberExpression)field.Body;
        var param = field.Parameters[0];
        var value = Expression.Parameter(typeof(float), "value");

        var getter = field.Compile();
        var setter = Expression.Lambda<Action<TComp, float>>(Expression.Assign(member, value), param, value).Compile();

        return new UpgradeData<TComp>(id, name, getter,setter, minPercent, maxPercent);
    }

    public static List<(UpgradeData, Component)> CreateUpgradePool(Upgradable[] upgradableComponents )
    {
        List<(UpgradeData, Component)> pool = new List<(UpgradeData, Component)>();
        
        foreach(var component in upgradableComponents)
        {
            foreach(var data in component.Upgrades)
            {
                pool.Add((data, (Component)component));
            }
        }
        
        if (pool.Count == 0)
            Debug.Log("UpgradeFactory: No upgrades found");

        return pool;
    }

    public static List<(UpgradeData, Component)> RollUpgrades(List<(UpgradeData, Component)> pool, int amount = 3)
    {
        if(pool.Count == 0 || pool == null || pool.Count < amount)
        {
            Debug.Log("UpgradeFactory: Upgrade pool empty");
            return null;
        }
        List<(UpgradeData, Component)> pickedUpgrades = new List<(UpgradeData, Component)>();
        
        int[] pickedNumbers = new int[amount];
        System.Array.Fill(pickedNumbers, -1); // Prevent index 0 from appearing as a duplicate
        for (int i = 0; i < amount ; i++)
        {
            int index = UnityEngine.Random.Range(0, pool.Count);
            if(pickedNumbers.Contains(index)) //Check if upgrade was already picked
            {
                i--; //Retry the 
                continue;
            }
            pickedNumbers.SetValue(index, i);
            var pick = pool[index];

            //pool.RemoveAt(index); //Prevent duplicate upgrades in the same roll
            pick.Item1.Roll();
            pickedUpgrades.Add(pick);
        }

        return pickedUpgrades;
    }
}
