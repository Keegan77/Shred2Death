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
        abilityBannedStateMap = new Dictionary<Type, List<BehaviourState>>
        {
            {
                typeof(BoostAbilityState), new List<BehaviourState>()
                {
                    player.halfPipeState,
                    player.driftState,
                    player.nosediveState,
                    player.grindState,
                }
            },
            {
                typeof(DualieUltimateAbilityState), new List<BehaviourState>()
                {
                    player.halfPipeState,
                    player.driftState,
                    player.nosediveState,
                }
            }
        };
        abilityStyleCostMap = new Dictionary<Type, float>
        {
            {typeof(BoostAbilityState), 100f},
            {typeof(DualieUltimateAbilityState), 100f},
        };
    }

    public Dictionary<Type, List<BehaviourState>> abilityBannedStateMap;

    public Dictionary<Type, float> abilityStyleCostMap;
}
