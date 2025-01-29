// Copyright (c) Meta Platforms, Inc. and affiliates.
using System.Linq;
using Oculus.Movement.AnimationRigging;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Movement.UI
{
    public class CustomAnimToggleTest : MonoBehaviour
    {
        /// <summary>
        /// Mask to apply.
        /// </summary>
        [SerializeField]
        private AvatarMask _customMask;
        /// <summary>
        /// Retargeting constraints to fix based on animation state.
        /// </summary>
        [SerializeField]
        private RetargetingAnimationConstraint[] _retargetingConstraints;
        /// <summary>
        /// Animators to control.
        /// </summary>
        [SerializeField]
        private Animator[] _animators;
        /// <summary>
        /// True if animation is enabled, false is not.
        /// </summary>
        [SerializeField]
        private bool _customAnimEnabled = false;
        /// <summary>
        /// Text to update to based on animation state.
        /// </summary>
        [SerializeField]
        private TMPro.TextMeshPro _worldText;
        /// <summary>
        /// Animator parameter name.
        /// </summary>
        [SerializeField]
        private string _animParamName = "Wave";
        private string _animParamName2 = "Anger";

        private const string _ANIM_OFF_TEXT = "Anim off";
        private const string _ANIM_ON_TEXT = "Anim on";

        public AnimationTest animTest;
        public bool IsInitialProcessed = false;

        private RetargetingAnimationConstraint assignedRetargetConstraint = null;
        private Animator assignedAnimator = null;

         private void Awake()
        {
            Assert.IsNotNull(_customMask);
            // Assert.IsTrue(_retargetingConstraints != null && _retargetingConstraints.Length > 0);
            // Assert.IsTrue(_animators != null && _animators.Length > 0);
            Assert.IsNotNull(_worldText);
        }

        // private void Start()
        // {
        //     EnforceAnimState();
        // }

        private void Update()
        {
            if (assignedRetargetConstraint == null){
                if (animTest.retargetingConstraint != null) {
                    assignedRetargetConstraint = animTest.retargetingConstraint;
                    var constraintsList = _retargetingConstraints.ToList();
                    constraintsList.Add(assignedRetargetConstraint);
                    _retargetingConstraints = constraintsList.ToArray();
                }
            }

            if (assignedAnimator == null){
                if (animTest.animator != null) {
                    assignedAnimator = animTest.animator;
                    var animatorList = _animators.ToList();
                    animatorList.Add(assignedAnimator);
                    _animators = animatorList.ToArray();
                }
            }

            if (assignedRetargetConstraint != null && assignedAnimator != null){
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
            _worldText.text = _customAnimEnabled ?
                _ANIM_ON_TEXT : _ANIM_OFF_TEXT;
        }
    }
}
