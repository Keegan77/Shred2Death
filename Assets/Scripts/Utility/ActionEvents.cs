using System;
using UnityEngine;

public static class ActionEvents
{
    public static Action<Type> OnBehaviourStateSwitch;

    #region Trick Events
    public static Action<Trick> OnTrickRequested;  // will be invoked when the player presses the trick input
    public static Action<Trick> OnTrickPerformed;  // invoked if while trick input was pressed, player was free for a trick
    public static Action<Trick> OnTrickCompletion; // invoked on trick animation completion
    
    #endregion

    public static Action<float> AddToStylePoints; //invoke when we want to add to style points without tricking
    #region Enemy Events
    public static Action OnEnemyKilled; //eventually should be populated with enemy data like how tricks are
                                        //this data should include the enemy's style value increase
                                        //& multiplier increase
    #endregion

    #region Gun Related Events
    // create a struct so i can return a variable with GunData and then also SceneDataForGun

    
    public static Action<GunSwitchData> OnGunSwitch; //invoked when the player switches guns    
    
    #endregion

    public static Action<string> OnPlayBehaviourAnimation;

    public static Action<AudioClip, float> PlayerSFXOneShot;

    public static Action<AudioClip> PlayLoopAudio;
    
    public static Action StopLoopAudio;

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
