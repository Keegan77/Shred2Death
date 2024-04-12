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
                }
            },
            {
                typeof(DualieUltimateAbilityState), new List<Type>()
                {
                    player.halfPipeState.GetType(),
                    player.GetType(),
                    player.nosediveState.GetType(),
                }
            }
        };
        abilityStyleCostMap = new Dictionary<Type, float>
        {
            {typeof(BoostAbilityState), 0f},
            {typeof(DualieUltimateAbilityState), 100f},
        };
    }

    public Dictionary<Type, List<Type>> abilityBannedStateMap;

    public Dictionary<Type, float> abilityStyleCostMap;
}
