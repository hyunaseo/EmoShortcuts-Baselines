using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Movement.AnimationRigging;
using ReadyPlayerMe.MetaMovement.Runtime;

public class RuntimeAvatarLoadChecker : MonoBehaviour
{
    // Movement Prefab Loader and Runtime Animator Controller should be assigned before start
    public MovementPrefabLoader movementPrefabLoader; 
    // public RuntimeAnimatorController animatorController;  


    // MyAvatar, RetargetingAnimationConstraint, Animator will be referred in Runtime Animation Toggle class 
    [HideInInspector] public GameObject loadedAvatar = null;
    [HideInInspector] public RetargetingAnimationConstraint loadedRetargetingAnimationConstraint = null;
    [HideInInspector] public Animator loadedAnimator = null;

    private bool IsAvatarLoaded = false;    
    private bool IsComponentAssigned = false;
    
    void Start()
    {
        if (movementPrefabLoader != null)
        {
            movementPrefabLoader.OnAvatarObjectLoaded.AddListener(OnAvatarLoaded);
        }
        else
        {
            Debug.LogError("MovementPrefabLoader is not assigned.");
        }
    }

    // void Update(){
    //     if (IsAvatarLoaded){
    //         if(!IsComponentAssigned){
    //             loadedAnimator = loadedAvatar.GetComponent<Animator>();
    //             if (loadedAnimator != null)
    //             {
    //                 loadedAnimator.runtimeAnimatorController = animatorController;
    //                 Debug.Log("RuntimeAvatarLoadChecker: animator is assigned to MyAvatar.");

    //                 var allConstraints = Resources.FindObjectsOfTypeAll<RetargetingAnimationConstraint>();
    //                 foreach (var constraint in allConstraints)
    //                 {
    //                     if (constraint.gameObject.name == "RetargetingConstraint")
    //                     {
    //                         loadedRetargetingAnimationConstraint = constraint;
    //                         break;
    //                     }
    //                 } 
    //                 Debug.Log("RuntimeAvatarLoadChecker: retargeting animation constraint is assigned to MyAvatar.");

    //                 IsComponentAssigned = true;
    //             }
    //             else{
    //                 Debug.LogWarning("MyAvatar doesn't have animator component.");
    //             }
    //         }
    //     }
    // }

    private void OnAvatarLoaded(GameObject loadedAvatar)
    {
        IsAvatarLoaded = true;
        this.loadedAvatar = loadedAvatar;
    }
}
