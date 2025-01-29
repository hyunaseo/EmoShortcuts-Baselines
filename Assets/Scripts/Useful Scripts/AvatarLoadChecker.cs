using UnityEngine;
using ReadyPlayerMe.MetaMovement.Runtime;

public class AvatarLoadChecker : MonoBehaviour
{
    public MovementPrefabLoader movementPrefabLoader; 
    public bool IsAvatarLoaded = false;
    
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

    private void OnAvatarLoaded(GameObject loadedAvatar)
    {
        Debug.Log($"My avatar is loaded: {loadedAvatar.name}");
        IsAvatarLoaded = true;
    }
}
