using System;

public static class ActionEvents
{
    public static Action OnBehaviourStateSwitch;

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
    
    
    
}
