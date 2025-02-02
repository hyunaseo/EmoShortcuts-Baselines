using System;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.MetaMovement
{
    /// <summary>
    /// Manages the mirroring of bones for an avatar, allowing for synchronized movement between source and mirrored bones.
    /// </summary>
    public class AvatarBoneMirror : MonoBehaviour
    {
        [System.Serializable]
        public class MirroredBonePair
        {
            [HideInInspector] public string Name;
            public Transform SourceBone;
            public Transform MirroredBone;
        }

        [SerializeField]
        protected Transform avatarToMirror;
        public Transform AvatarToMirror => avatarToMirror;
        
        [SerializeField]
        protected MirroredBonePair[] mirroredBonePairs;

        protected string[] animatedBonePairs;

        private void Start()
        {
            if (avatarToMirror == null)
            {
                gameObject.SetActive(false); // Disable the object if no avatar is set.
            }

            // animatedBonePairs = new string[] {"Spine", "Shoulder", "Arm", "Hand", "Palm", "Neck", "Head", "Eye", "UpLeg", "Toe"};
            animatedBonePairs = new string [] {"Armature", "LeftLeg", "RightLeg", "LeftFoot", "RightFoot"};
        }

        /// <summary>
        /// Sets the avatar to mirror and activates the GameObject.
        /// </summary>
        /// <param name="avatar">The GameObject representing the avatar to be mirrored.</param>
        public void SetAvatarToMirror(GameObject avatar)
        {
            avatarToMirror = avatar.transform;
            SetMirroredBonePair();
            gameObject.SetActive(true); // Reactivate the GameObject with the new avatar.
        }

        /// <summary>
        /// Creates a copy of the avatar, transfers its mesh, updates face tracking meshes, and sets the avatar to mirror.
        /// </summary>
        /// <param name="avatar">The GameObject representing the avatar to be mirrored.</param>
        public void TransferMeshAndSetAvatarToMirror(GameObject avatar)
        {
            var avatarCopy = Instantiate(avatar, transform);
            AvatarMeshHelper.TransferMesh(avatarCopy, gameObject);
            MetaMovementHelper.UpdateFaceTrackingMeshes(gameObject);
            SetAvatarToMirror(avatar);
            Destroy(avatarCopy); // Clean up the temporary avatar copy.
        }
        
        private void LateUpdate()
        {
            if(avatarToMirror == null)
            {
                return;
            }
            // Update this GameObject's transform to match the avatar's.
            transform.localPosition = avatarToMirror.localPosition;
            transform.localRotation = avatarToMirror.localRotation;
            
            // Mirror the bone positions and rotations.
            foreach (var transformPair in mirroredBonePairs)
            {
                transformPair.MirroredBone.localPosition = transformPair.SourceBone.localPosition;
                transformPair.MirroredBone.localRotation = transformPair.SourceBone.localRotation;
            }
        }
        
        /// <summary>
        /// Sets up the mirrored bone pairs by matching the source avatar's bones to corresponding bones in the mirrored object.
        /// </summary>
        public void SetMirroredBonePair()
        {
            if(avatarToMirror == null)
            {
                Debug.LogError("Avatar to mirror is not set.");
                return;
            }
            var mirrorArmature = AvatarToMirror.transform.Find("Armature");
            if(mirrorArmature == null)
            {
                Debug.LogError("Armature not found skipping bone pair setup.");
                return;
            }
            var childTransforms = new List<Transform>(mirrorArmature.GetComponentsInChildren<Transform>(true));
            var bonePairList = new List<MirroredBonePair>();

            foreach (var childTransform in childTransforms)
            {
                Transform mirroredTransform = null;

                if(childTransform.name == "Armature"){
                    mirroredTransform =
                        gameObject.transform.FindChildRecursive(childTransform.name); 
                }

                else{
                    mirroredTransform =
                        gameObject.transform.Find("Armature").transform.FindChildRecursive(childTransform.name);
                }

                if (mirroredTransform != null)
                {
                    bonePairList.Add(new MirroredBonePair
                    {
                        MirroredBone = mirroredTransform,
                        SourceBone = childTransform,
                        Name = childTransform.name
                    });

                    Debug.Log($"Hyuna: joint name is {childTransform.name}");
                }
                else
                {
                    Debug.LogError($"Missing a mirrored transform for: {transform.name}");
                }
                
            }
            mirroredBonePairs = bonePairList.ToArray();
        }

        public void InitializeMirroredBonePairs()
        {
            mirroredBonePairs = new MirroredBonePair[0];
            Debug.Log("Hyuna: MirroredBonePairs has been initialized as an empty array.");
        }

        public void AnimatedMirroredBonePairs()
        {
            if (mirroredBonePairs.Length == 0)
                return; 

            List<MirroredBonePair> filteredList = new List<MirroredBonePair>();

            foreach (var pair in mirroredBonePairs)
            {
                bool shouldRemove = true;
                foreach (var keyword in animatedBonePairs)
                {
                    if (pair.Name == keyword)
                    {
                        shouldRemove = false;            
                        break; 
                    }
                }

                if (!shouldRemove)
                {
                    filteredList.Add(pair);
                }
            }

            mirroredBonePairs = filteredList.ToArray();
        }
    }
}