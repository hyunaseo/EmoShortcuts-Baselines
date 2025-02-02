using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RaycastMenuInteractor : MonoBehaviour
{
    public UltimateRadialMenu radialMenu;
    public Transform leftFingerTip, leftFingerBase, rightFingerTip, rightFingerBase;


    private void Update ()
    {
        // If any of the components are unassigned, warn the user and return.
        if( radialMenu == null)
        {
            Debug.LogError( "raial menu is null" );
            return;
        }

        if(leftFingerTip == null)
        {
            Debug.LogError( "leftFingerTip is null" );
            return;
        }

        if(leftFingerBase == null)
        {
            Debug.LogError( "leftFingerBase is null" );
            return;
        }

        if(rightFingerBase == null)
        {
            Debug.LogError( "rightFingerBase is null" );
            return;
        }

        if(rightFingerTip == null)
        {
            Debug.LogError( "rightFingerTip is null" );
            return;
        }

        // Send in the finger tip and base positions to be calculated on the menu.
        radialMenu.inputManager.SendRaycastInput( leftFingerTip.position, leftFingerBase.position);
        radialMenu.inputManager.SendRaycastInput( rightFingerTip.position, rightFingerBase.position);
    }
}
