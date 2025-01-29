// Copyright (c) Meta Platforms, Inc. and affiliates.
using System.Linq;
using Oculus.Movement.AnimationRigging;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Movement.UI
{
    public class RuntimeAnimationController : MonoBehaviour
    {
        [SerializeField] private AvatarMask _customMask; // Mask to apply.

        public RuntimeAvatarLoadChecker runtimeAvatarLoadChecker;

        
        [SerializeField] private RetargetingAnimationConstraint[] _retargetingConstraints; // Retargeting constraints to fix based on animation state.
        [SerializeField] private Animator[] _animators; // Animators to control.
        [SerializeField] private bool _customAnimEnabled = false; // True if animation is enabled, false is not.
        [SerializeField] private string _animParamName = "Wave"; // Animator parameter name.

        private GameObject myAvatar; 

        public bool IsInitialProcessed = false;

        private RetargetingAnimationConstraint myRetargetingAnimationConstraint = null;
        private Animator myAnimator = null;

         private void Awake()
        {
            Assert.IsNotNull(_customMask);
        }

        private void Update()
        {
            if (runtimeAvatarLoadChecker.loadedAnimator != null){
                myAvatar = runtimeAvatarLoadChecker.loadedAvatar;
            }

            if (runtimeAvatarLoadChecker.loadedRetargetingAnimationConstraint != null && myRetargetingAnimationConstraint == null){
                myRetargetingAnimationConstraint = runtimeAvatarLoadChecker.loadedRetargetingAnimationConstraint;
                var constraintsList = _retargetingConstraints.ToList();
                constraintsList.Add(myRetargetingAnimationConstraint);
                _retargetingConstraints = constraintsList.ToArray();
            }

            if(runtimeAvatarLoadChecker.loadedAnimator !=null && myAnimator == null){
                myAnimator = runtimeAvatarLoadChecker.loadedAnimator;
                var animatorList = _animators.ToList();
                animatorList.Add(myAnimator);
                _animators = animatorList.ToArray();
            }
            

            if (myRetargetingAnimationConstraint != null && myAnimator != null){
                if (!IsInitialProcessed){
                    EnforceAnimState();
                    IsInitialProcessed = true;
                }

                // since the animation rig set up might reboot due to calibration
                // keep setting parameter to the proper value.
                foreach (var animator in _animators)
                {
                    animator.SetBool(_animParamName, _customAnimEnabled);
                }
            }
        }

        public void SwapAnimState()
        {
            _customAnimEnabled = !_customAnimEnabled;
            EnforceAnimState();
        }

        private void EnforceAnimState()
        {
            foreach (var retargetConstraint in _retargetingConstraints)
            {
                retargetConstraint.data.AvatarMaskComp =
                    _customAnimEnabled ? _customMask : null;
                retargetConstraint.data.UpdateDataArraysWithAdjustments();
            }
            foreach (var animator in _animators)
            {
                animator.SetBool(_animParamName, _customAnimEnabled);
            }
        }
    }
}
