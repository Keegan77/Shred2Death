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
    static Trick Ollie        = new Trick("Ollie", 80, jumpTrick);
    static Trick Kickflip     = new Trick("Ollie", 80, DPadRIGHT);
    static Trick PopShuvIt    = new Trick("Ollie", 80, DPadLEFT);
    
    //Grind Tricks
    static Trick FiftyFifty   = new Trick("Ollie", 120, DPadRIGHT);
    static Trick FiveO        = new Trick("Ollie", 120, DPadLEFT);
    static Trick BoardSlide   = new Trick("Ollie", 120, LBumperTrick);
    
    //Air Tricks
    static Trick Backflip     = new Trick("Ollie", 150, DPadDOWN);
    static Trick NoseGrab     = new Trick("Ollie", 150, DPadUP);
    static Trick OneEighty    = new Trick("Ollie", 150, RBumperTrick);
    #endregion // these tricks are just for testing, they will be replaced with real tricks later
    
    
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
