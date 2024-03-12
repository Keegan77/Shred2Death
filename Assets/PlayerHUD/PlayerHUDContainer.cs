using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Acts as an interface to the playerHUD widget to make accessing its information easier.
/// </summary>
public class PlayerHUDContainer : MonoBehaviour
{
    public MultiFillBar styleMeter;
    public FillBar healthBar;
    public FillBar ammoBar;
    public EnemyText enemyText;
}
