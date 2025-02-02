using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.MetaMovement.Editor
{
    /// <summary>
    /// Custom editor for Avatar Bone Mirror component.
    /// </summary>
    [CustomEditor(typeof(FixedAvatarBoneMirror)), CanEditMultipleObjects]
    public class FixedAvatarBoneMirrorEditor : UnityEditor.Editor
    {
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            var mirroredObject = (FixedAvatarBoneMirror) target;
            if(mirroredObject.AvatarToMirror != null)
            {
                if (GUILayout.Button("Get Mirrored Bone Pairs "))
                {
                    mirroredObject.SetMirroredBonePair();
                }
            }
            else
            {
                var mirroredTransformPairs = serializedObject.FindProperty("mirroredBonePairs");
                mirroredTransformPairs.ClearArray();
            }
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            DrawDefaultInspector();
        }
    }
}
