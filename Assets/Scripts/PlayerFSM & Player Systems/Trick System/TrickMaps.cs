using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class TrickMaps
{
    private static InputAction jumpTrick = InputRouting.Instance.input.PlayerTricks.JumpButtonTrick;
    private static InputAction LBumperTrick = InputRouting.Instance.input.PlayerTricks.LBumperTrick;
    private static InputAction RBumperTrick = InputRouting.Instance.input.PlayerTricks.RBumperTrick;
    
    private static InputAction DPadUP = InputRouting.Instance.input.PlayerTricks.DpadUP;
    private static InputAction DPadDOWN = InputRouting.Instance.input.PlayerTricks.DpadDOWN;
    private static InputAction DPadLEFT = InputRouting.Instance.input.PlayerTricks.DpadLEFT;
    private static InputAction DPadRIGHT = InputRouting.Instance.input.PlayerTricks.DpadRIGHT;
    
    #region Trick Creation
    //Skating Tricks
    static Trick Ollie        = new Trick("Idle", 5, 3, .2f, jumpTrick, CustomTrickMethods.OllieFunc, canBeInterrupted:true); //less ammo bc it's the basic trick & you're always jumping around
    
    static Trick PopShuvIt    = new Trick("PopShoveIt", 10, 5, .2f, DPadLEFT, CustomTrickMethods.PopShuvItCustomFunction);
    
    
    //Air Tricks
    static Trick Backflip     = new Trick("Kickflip", 20, 5, .2f, DPadLEFT, CustomTrickMethods.PopShuvItCustomFunction);
    static Trick Kickflip     = new Trick("Kickflip", 10, 6, .2f, DPadRIGHT, CustomTrickMethods.PopShuvItCustomFunction);
    static Trick Heelflip     = new Trick("Hellflip", 10, 6, .2f, DPadDOWN, CustomTrickMethods.PopShuvItCustomFunction);
    #endregion // these tricks are just for testing, they will be replaced with real tricks later
    
    
    public static readonly Dictionary<Type, Trick[]> StateMap = new Dictionary<Type, Trick[]>
    {
        {typeof(PlayerSkatingState), new []{Ollie}},
        {typeof(PlayerAirborneState), new []{Kickflip, PopShuvIt, Heelflip}},
        // Add more states and associated tricks here...
    };
    
}

public class Trick
{
    public string animTriggerName { get; }
    public float stylePoints { get; }
    
    public float ammoBonus { get; }
    
    public float multiplierIncrease { get; }
    
    public InputAction trickAction { get; }
    
    public bool canBeInterrupted { get; }
    
    public CustomTrickMethods.TrickMethod customMethod { get; }

    public Trick(string animTriggerName,
                 float stylePoints,
                 int ammoBonus,
                 float multiplierIncrease,
                 InputAction trickAction,
                 CustomTrickMethods.TrickMethod customMethod = null,
                 bool canBeInterrupted = false)
    {
        this.animTriggerName = animTriggerName;
        this.stylePoints = stylePoints;
        this.trickAction = trickAction;
        this.customMethod = customMethod;
        this.ammoBonus = ammoBonus;
        this.multiplierIncrease = multiplierIncrease;
        this.canBeInterrupted = canBeInterrupted;
    }
}
