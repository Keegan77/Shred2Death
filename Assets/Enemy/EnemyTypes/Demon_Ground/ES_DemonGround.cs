using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_DemonGround : Enemy_State
{
    protected E_Demon_Ground eGround;

    #region STATEMACHINE
    private void Awake ()
    {
        eGround = transform.parent.GetComponent<E_Demon_Ground>();
    }
    #endregion

        #region NAVIGATION
    public void EndPath ()
    {
        eGround.agentPath.ClearCorners ();
    }
    #endregion
}
