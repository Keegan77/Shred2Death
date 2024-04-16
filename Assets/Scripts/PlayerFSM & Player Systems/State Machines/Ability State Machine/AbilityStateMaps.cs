using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityStateMaps
{
    private PlayerBase player;

    public AbilityStateMaps(PlayerBase player)
    {
        this.player = player;
        abilityBannedStateMap = new Dictionary<Type, List<Type>>
        {
            {
                typeof(BoostAbilityState), new List<Type>()
                {
                    player.halfPipeState.GetType(),
                    player.driftState.GetType(),
                    player.nosediveState.GetType(),
                    player.grindState.GetType(),
                    player.deathState.GetType(),
                }
            },
            {
                typeof(DualieUltimateAbilityState), new List<Type>()
                {
                    player.deathState.GetType(),
                }
            },
            {
                typeof(ShotgunUltimateAbilityState), new List<Type>()
                {
                    player.deathState.GetType(),
                    player.nosediveState.GetType(),
                }
            }
            
        };
        
        abilityStyleCostMap = new Dictionary<Type, float>
        {
            {typeof(BoostAbilityState), 25f},
            {typeof(DualieUltimateAbilityState), 100f},
            {typeof(ShotgunUltimateAbilityState), 0f},
        };
    }

    public Dictionary<Type, List<Type>> abilityBannedStateMap;

    public Dictionary<Type, List<Type>> selectiveAbilityStateMap; //todo: make this a thing. if an ability is in this
                                                                  //todo: map at all, it's list of states are the only ones
                                                                  //todo: that are not banned, as oppose to banned ability
                                                                  //todo: states, where the listed abilities are the banned
                                                                  //todo: abilities

    public Dictionary<Type, float> abilityStyleCostMap;
}
