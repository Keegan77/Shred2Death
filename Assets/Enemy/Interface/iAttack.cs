using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// iAttack is attached to specific enemy attack states that have firing patterns.
/// States that are dedicated to attacking (ex: ESG_Attack) will implement
/// this interface to provide themselves with firing logic.
/// 
/// Muzzlepoints and bulletInfo
/// </summary>
public interface iAttack
{
    #region Parameters
    // Using abstracted getters and setters lets us declare these publicly
    // while also allowing us to customize per bullet pattern which behavior we use.
    abstract GameObject muzzlePoint { get; set; }
    abstract Enemy_BulletPattern bulletInfo { get; set; }

    #endregion

    #region abstract functions
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    abstract IEnumerator PlayShot ();
    #endregion

    #region Concrete Functions

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>


    #endregion
}
