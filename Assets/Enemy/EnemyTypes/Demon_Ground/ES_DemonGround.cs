using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_DemonGround : Enemy_State
{
    protected E_Demon_Ground e;

    #region STATEMACHINE
    private void Awake ()
    {
        e = transform.parent.GetComponent<E_Demon_Ground>();

        Debug.Log ($"ESDG debug: {e}");
    }
    #endregion

        #region NAVIGATION
    public void EndPath ()
    {
        e.agentPath.ClearCorners ();
    }
    #endregion
}
