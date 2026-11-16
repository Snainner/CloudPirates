using UnityEngine;
public class GameEvent { }

public static class Events 
{
    public static GameEvent CaptureEvent = new CaptureEvent();
    public static GameEvent PlayerSightedEvent = new PlayerSightedEvent();
    public static GameEvent LevelUpEvent = new LevelUpEvent();
    public static GameEvent ApplyUpgradeEvent = new ApplyUpgradeEvent();
}

//Triggered each time a voxel gets captured
public class CaptureEvent : GameEvent
{
    public int Captures; 
}

//Each time score gets added 
public class ScoreEvent : GameEvent
{
    public int Score; 
}

//Create upgrade data and pull up UI
public class LevelUpEvent : GameEvent
{
    public int Level;
    public int RequiredScore;
}

//Apply the upgrade
public class ApplyUpgradeEvent : GameEvent
{
    public int ChosenUpgrade;
}
public class UpgradeUIEvent : GameEvent
{
    public string[] UpgradePreviews;
}

public class ButtonsHeldEvent : GameEvent
{
    public bool Up;
    public bool Down;
}

public class PlayerSightedEvent : GameEvent
{
    //public float TimeOfSight;
    public bool PlayerSighted;
}

