using System.Collections;
using System.Collections.Generic;

using Oculus.Movement.UI;

using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.UI;

using ReadyPlayerMe.MetaMovement;

public class RetargetEmotionWheel : MonoBehaviour
{
    [Header("Radial Menu Configuration")]
    public UltimateRadialMenu radialMenu;
    public List<UltimateRadialButtonInfo> buttonInfos = new List<UltimateRadialButtonInfo>();
    private UltimateRadialButtonInfo angerButtonInfo = null;
    
    [Header("Emotion Colors")]
    private Dictionary<string, string> emotionColors = new Dictionary<string, string>()
    {
        { "anger", "#CB7C87" },      
        { "disgust", "#3CBB9E" },    
        { "joy", "#E7A54E" },        
    };

    [Header("Emotion Animations")]
    public AnimationClip AngerAnimationClip;
    public AnimationClip DisgustAnimationClip;
    public AnimationClip JoyAnimationClip;

    private Dictionary<string, AnimationClip> emotionAnimations;
    private AnimationClip currentAnimation;

    [Header("Emotion States")]
    public RuntimeAnimationController runtimeAnimatorController;
    private Dictionary<string, bool> emotionStates = new Dictionary<string, bool>();

    void Start(){
        foreach (var buttonInfo in buttonInfos)
        {
            radialMenu.RegisterButton(ToggleEmotionState, buttonInfo);
            if(angerButtonInfo == null) angerButtonInfo = buttonInfo;
        }

        SetRadialButtonColor();
       
        foreach (var emotion in emotionColors.Keys)
        {
            emotionStates[emotion] = false;
        }

        emotionAnimations = new Dictionary<string, AnimationClip>()
        {
            { "anger", AngerAnimationClip },
            { "disgust", DisgustAnimationClip },
            { "joy", JoyAnimationClip }
        };

        StartCoroutine(CallTuronOnButtonAfterDelay(10f));
        StartCoroutine(CallTuronOffButtonAfterDelay(20f));
    }

    // TEST CODE 
    
    IEnumerator CallTuronOnButtonAfterDelay(float delay)
    {
        float elapsedTime = 0f;
        while (elapsedTime < delay)
        {
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
            Debug.Log($"Elapsed Time: {elapsedTime} seconds");
        }

        TurnOnButton();
    }

    void TurnOnButton()
    {
        string tempKey = "joy";
        Debug.Log("Hyuna: Turn on Joy Animation.");
        
        ResetAllEmotions();
        emotionStates[tempKey] = true;

        emotionAnimations.TryGetValue(tempKey, out currentAnimation);
        TrackingToEmotion(tempKey, currentAnimation);
    }

    IEnumerator CallTuronOffButtonAfterDelay(float delay)
    {
        float elapsedTime = 0f;
        while (elapsedTime < delay)
        {
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
            Debug.Log($"Elapsed Time: {elapsedTime} seconds");
        }

        TurnOffButton();
    }

    void TurnOffButton(){
        string tempKey = "joy";
        Debug.Log("Hyuna: Turn off Joy Animation");

        Debug.Log($"ToggleEmotionState: CASE #3: {tempKey} is turned off.");
        ResetAllEmotions();
        EmotionToTracking(tempKey);
    }
    

    void ToggleEmotionState(string key)
    {
        key = key.ToLower();

        string previouslyActiveKey = null;

        foreach (var state in emotionStates)
        {
            if (state.Value)
            {
                previouslyActiveKey = state.Key;
                break;
            }
        }

        // CASE #3: EMOTION A -> EMOTION A (TURN OFF EMOTION A)
        if (emotionStates[key])
        {
            Debug.Log($"ToggleEmotionState: CASE #3: {key} is turned off.");
            ResetAllEmotions();
            EmotionToTracking(key);
        }

        else
        {
            // CASE #2: EMOTION A -> EMOTION B
            if (previouslyActiveKey != null)
            {
                Debug.Log($"ToggleEmotionState: CASE #2: {previouslyActiveKey} was turned off because {key} was clicked.");
                ResetAllEmotions();
                emotionStates[key] = true;

                EmotionToEmotion(previouslyActiveKey, key);
            }

            // CASE #1: TRACKING -> EMOTION 
            else
            {
                Debug.Log($"ToggleEmotionState: CASE #1: {key} is turned on.");
                ResetAllEmotions();
                emotionStates[key] = true;

                emotionAnimations.TryGetValue(key, out currentAnimation);
                TrackingToEmotion(key, currentAnimation);
            }
        }
    }

    private void ResetAllEmotions()
    {
        List<string> keys = new List<string>(emotionStates.Keys);

        foreach (var key in keys)
        {
            emotionStates[key] = false;
        }
        Debug.Log("Hyuna: ResetAllEmotions.");
    }

    void SetRadialButtonColor()
    {
        foreach (Transform buttonTransform in radialMenu.transform)
        {
            Image buttonImage = buttonTransform.GetComponent<Image>();
            if (buttonImage == null)
                continue;

            Text buttonText = buttonTransform.GetComponentInChildren<Text>();
            if (buttonText == null)
                continue;

            string textValue = buttonText.text.ToLower();
            Color newColor = Color.gray;

            foreach (var emotion in emotionColors.Keys)
            {
                if (textValue.Contains(emotion))
                {
                    ColorUtility.TryParseHtmlString(emotionColors[emotion], out newColor);
                    break; 
                }
            }

            buttonImage.color = newColor;
        }
    }

    // CASE #1: TRACKING -> EMOTION. Mirrored Avatar Does Not Have Playable Graph.
    void TrackingToEmotion(string key, AnimationClip newClip)
    {
        Debug.Log($"Hyuna: TrackingToEmotion: CASE #1: Turn on {key}'s animation.");
        runtimeAnimatorController._animParamName = key;
        runtimeAnimatorController.SwapAnimState();
    }

    // CASE #2: EMOTION A -> EMOTION B
    void EmotionToEmotion(string fromKey, string toKey)
    {
        Debug.Log($"1. EmotionToEmotion: CASE #2: Smooth transitioning from {fromKey} to {toKey}.");
        runtimeAnimatorController._animParamName = toKey;
        runtimeAnimatorController.SwapAnimState();
    }

    // CASE #3: EMOTION A -> EMOTION A (TURN OFF EMOTION A)
    void EmotionToTracking(string key){
        Debug.Log($"Hyuna: EmotionToTracking: CASE #3: Turn off {key}'s animation.");
        runtimeAnimatorController.SwapAnimState();
    }
}