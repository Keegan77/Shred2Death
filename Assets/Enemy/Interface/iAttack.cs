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
    abstract GameObject muzzlePoints { get; set; }
    abstract Enemy_BulletPattern bulletInfo { get; set; }

    #endregion

    #region abstract functions
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayShot ();
    #endregion

    #region Concrete Functions

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>


    #endregion
}
