using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Movement.AnimationRigging;
using ReadyPlayerMe.MetaMovement.Runtime;

public class AnimationTest : MonoBehaviour
{
    public MovementPrefabLoader movementPrefabLoader; 
    public bool IsAvatarLoaded = false;
    public GameObject MyAvatar = null;
    public RuntimeAnimatorController animatorController;  

    public bool IsAnimControllerLoaded = false;
    public RetargetingAnimationConstraint retargetingConstraint = null;
    public Animator animator = null;
    
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

    void Update(){
        if (IsAvatarLoaded){
            if(!IsAnimControllerLoaded){
                animator = MyAvatar.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = animatorController;
                    Debug.Log("Animator Controller is assigned to MyAvatar.");

                    var allConstraints = Resources.FindObjectsOfTypeAll<RetargetingAnimationConstraint>();
                    foreach (var constraint in allConstraints)
                    {
                        if (constraint.gameObject.name == "RetargetingConstraint")
                        {
                            retargetingConstraint = constraint;
                            break;
                        }
                    } 
                    IsAnimControllerLoaded = true;
                }
                else{
                    Debug.LogWarning("MyAvatar doesn't have animator component.");
                }
            }
        }
    }

    private void OnAvatarLoaded(GameObject loadedAvatar)
    {
        Debug.Log($"My avatar is loaded: {loadedAvatar.name}");
        IsAvatarLoaded = true;
        MyAvatar = loadedAvatar;
    }
}
