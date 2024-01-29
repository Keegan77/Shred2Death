using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class TrickMaps
{
    #region Trick Creation
    //Skating Tricks
    static Trick Ollie        = new Trick("Ollie", 100, InputRouting.Instance.input.PlayerTricks.JumpButtonTrick);
    static Trick Kickflip     = new Trick("Kickflip", 100, InputRouting.Instance.input.PlayerTricks.DPadTrick);
    static Trick PopShuvIt    = new Trick("PopShuvIt", 100, InputRouting.Instance.input.PlayerTricks.DPadTrick);
    
    //Grind Tricks
    static Trick FiftyFifty   = new Trick("FiftyFifty", 100, InputRouting.Instance.input.PlayerTricks.DPadTrick);
    static Trick FiveO        = new Trick("Five0", 100, InputRouting.Instance.input.PlayerTricks.DPadTrick);
    static Trick BoardSlide   = new Trick("BoardSlide", 100, InputRouting.Instance.input.PlayerTricks.LBumperTrick);
    
    //Air Tricks
    static Trick Backflip     = new Trick("Backflip", 100, InputRouting.Instance.input.PlayerTricks.DPadTrick);
    static Trick NoseGrab     = new Trick("Nosegrab", 100, InputRouting.Instance.input.PlayerTricks.DPadTrick);
    static Trick OneEighty    = new Trick("180", 100, InputRouting.Instance.input.PlayerTricks.RBumperTrick);
    #endregion
    
    
    public static readonly Dictionary<Type, Trick[]> StateMap = new Dictionary<Type, Trick[]>
    {
        {typeof(PlayerSkatingState), new []{Kickflip, PopShuvIt, Ollie}},
        {typeof(PlayerGrindState),   new []{FiftyFifty, FiveO, BoardSlide}},
        {typeof(PlayerAirborneState), new []{Backflip, NoseGrab, OneEighty}},
        // Add more states and associated tricks here...
    };
    
}

public class Trick
{
    public string animTriggerName { get; }
    public float score { get; }
    public InputAction trickAction { get; }

    public Trick(string animTriggerName, float score, InputAction trickAction)
    {
        this.animTriggerName = animTriggerName;
        this.score = score;
        this.trickAction = trickAction;
    }
}
