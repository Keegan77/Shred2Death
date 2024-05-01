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
    public static Trick Ollie        = new Trick("Ollie", 5, 3, .2f, jumpTrick, 0, CustomTrickMethods.OllieFunc, canBeInterrupted:true, useOnRelease:true); //less ammo bc it's the basic trick & you're always jumping around
    static Trick PopShuvIt    = new Trick("PopShoveIt", 10, 5, .2f, DPadLEFT, 1.2f, CustomTrickMethods.GeneralTrickFunc);
    
    static Trick Backflip     = new Trick("Kickflip", 20, 5, .2f, DPadUP, .875f,CustomTrickMethods.GeneralTrickFunc);
    static Trick Kickflip     = new Trick("Kickflip", 10, 6, .2f, DPadRIGHT, .875f,CustomTrickMethods.GeneralTrickFunc);
    static Trick Heelflip     = new Trick("Heelflip", 10, 6, .2f, DPadDOWN, .875f,CustomTrickMethods.GeneralTrickFunc);
    #endregion 
    
    
    public static readonly Dictionary<Type, Trick[]> StateMap = new Dictionary<Type, Trick[]>
    {
        {typeof(PlayerSkatingState), new []{Ollie}},
        {typeof(PlayerAirborneState), new []{Kickflip, PopShuvIt, Heelflip}},
        {typeof(PlayerHalfpipeState), new []{Kickflip, PopShuvIt, Heelflip}},
        // Add more states and associated tricks here...
    };
    
}

public class Trick
{
    public string animTriggerName { get; }
    
    public float animTime { get; }
    
    public float stylePoints { get; }
    
    public float ammoBonus { get; }
    
    public float multiplierIncrease { get; }
    
    public InputAction trickAction { get; }
    
    public bool canBeInterrupted { get; }
    
    public bool skipAnim { get; }
    public bool useOnRelease { get; }
    
    public CustomTrickMethods.TrickMethod customMethod { get; }

    public Trick(string animTriggerName,
                 float stylePoints,
                 int ammoBonus,
                 float multiplierIncrease,
                 InputAction trickAction,
                 float animTime,
                 CustomTrickMethods.TrickMethod customMethod = null,
                 bool canBeInterrupted = false,
                 bool skipAnim = false,
                 bool useOnRelease = false)
    {
        this.animTriggerName = animTriggerName;
        this.stylePoints = stylePoints;
        this.trickAction = trickAction;
        this.customMethod = customMethod;
        this.ammoBonus = ammoBonus;
        this.multiplierIncrease = multiplierIncrease;
        this.animTime = animTime;
        this.skipAnim = skipAnim;
        this.canBeInterrupted = canBeInterrupted;
        this.useOnRelease = useOnRelease;
    }
}
