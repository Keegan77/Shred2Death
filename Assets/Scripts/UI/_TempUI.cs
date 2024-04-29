using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class _TempUI : MonoBehaviour
{
    [SerializeField] TrickComboHandler trickComboHandler;
    [SerializeField] TextMeshProUGUI stylePointsText;
    [SerializeField] TextMeshProUGUI styleLevelText;
    [SerializeField] TextMeshProUGUI multiplierText;
    [SerializeField] TextMeshProUGUI timeSinceLastTrickText;
    [SerializeField] TextMeshProUGUI timeSinceMultiplierIncreaseText;
    
    private void Update()
    {
        stylePointsText.text = "Style Points: " + trickComboHandler.GetStylePoints();
        styleLevelText.text = "Style Level: " + trickComboHandler.GetStyleLevel();
        multiplierText.text = "Multiplier: " + trickComboHandler.GetMultiplier();
        timeSinceLastTrickText.text = "Time Since Last Trick: " + trickComboHandler.GetTimeSinceLastTrick().ToString("F2");;
        timeSinceMultiplierIncreaseText.text = "Time Since Multiplier Increase: " + trickComboHandler.GetTimeSinceMultiplierIncrease().ToString("F2");;
    }
}
