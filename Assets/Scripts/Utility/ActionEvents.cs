using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class ActionEvents
{
    #region Trick Events
    public static Action<Trick> OnTrickRequested;  // will be invoked when the player presses the trick input
    public static Action<Trick> OnTrickPerformed;  // invoked if while trick input was pressed, player was free for a trick
    public static Action<Trick> OnTrickCompletion; // invoked on trick animation completion
    public static Action<float> AddToStylePoints; //invoke when we want to add to style points without tricking
    #endregion
    #region Enemy Events
    public static Action OnEnemyKilled; //eventually should be populated with enemy data like how tricks are
                                        //this data should include the enemy's style value increase
                                        //& multiplier increase
    #endregion
    #region Gun Related Events
    public static Action<GunSwitchData> OnGunSwitch; //invoked when the player switches guns    
    #endregion
    #region Audio Events
    public static Action<AudioClip, float> PlayerSFXOneShot;
    public static Action<AudioClip> PlayLoopAudio;
    public static Action StopLoopAudio;
    #endregion
    #region Misc. Player Events
    public static Action<string> OnPlayBehaviourAnimation;
    public static Action<Type> OnBehaviourStateSwitch;
    public static Action LoadBowlMeshes; //pertains to player because only the player can interact with these
    public static Action<AbilityState> OnAbilityStateSwitch;
    public static Action IntermediaryAbilityStateEnter;
    public static Action MakePlayerLookForward;
    public static Action MakePlayerLookMouse;
    public static Action<InputAction> FreezeAndWaitForInput;
    #endregion
    #region UI Events
    public static Action<bool, float> FadeToBlack;
    #endregion
}

public struct GunSwitchData
{
    public GunData GunData;
    public SceneDataForGun SceneDataForGun;

    public GunSwitchData(GunData gunData, SceneDataForGun sceneDataForGun)
    {
        GunData = gunData;
        SceneDataForGun = sceneDataForGun;
    }
}
