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
        if( radialMenu == null || leftFingerTip == null || leftFingerBase == null || rightFingerTip == null || rightFingerBase == null)
        {
            Debug.LogError( "" );
            return;
        }

        // Send in the finger tip and base positions to be calculated on the menu.
        radialMenu.inputManager.SendRaycastInput( leftFingerTip.position, leftFingerBase.position);
        radialMenu.inputManager.SendRaycastInput( rightFingerTip.position, rightFingerBase.position);
    }
}
