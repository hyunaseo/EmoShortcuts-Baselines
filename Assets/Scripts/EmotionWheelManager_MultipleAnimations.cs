using System.Collections;
using System.Collections.Generic;

using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.UI;

using ReadyPlayerMe.MetaMovement;

public class EmotionWheelManager_MultipleAnimations : MonoBehaviour
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
        { "fear", "#7575B5" },       
        { "sad", "#37A3BA" },        
        { "surprise", "#E2886E" },   
        { "neutral", "#7F7F7F" }     
    };

    [Header("Emotion Animations")]
    public AnimationClip AngerAnimationClip;
    public AnimationClip DisgustAnimationClip;
    public AnimationClip JoyAnimationClip_Low;
    public AnimationClip JoyAnimationClip_Mid;
    public AnimationClip JoyAnimationClip_High;
    public AnimationClip FearAnimationClip;
    public AnimationClip SadAnimationClip;
    public AnimationClip SurpriseAnimationClip;
    public AnimationClip NeutralAnimationClip;

    private Dictionary<string, List<AnimationClip>> emotionAnimations;
    private AnimationClip currentAnimation;

    [Header("Emotion States")]
    private Dictionary<string, bool> emotionStates = new Dictionary<string, bool>();

    [Header("Avatar GameObjects")]
    public GameObject MirroredAvatar;
    private AvatarBoneMirror avatarBoneMirror;

    [Header("Animation System")]
    private Animator animator;
    private PlayableGraph playableGraph;

    private int joyCount; 

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

        emotionAnimations = new Dictionary<string, List<AnimationClip>>()
        {
            { "anger", new List<AnimationClip> { AngerAnimationClip } },
            { "disgust", new List<AnimationClip> { DisgustAnimationClip } },
            { "joy", new List<AnimationClip> { JoyAnimationClip_Low, JoyAnimationClip_Mid, JoyAnimationClip_High } },
            { "fear", new List<AnimationClip> { FearAnimationClip } },
            { "sad", new List<AnimationClip> { SadAnimationClip } },
            { "surprise", new List<AnimationClip> { SurpriseAnimationClip } },
            { "neutral", new List<AnimationClip> { NeutralAnimationClip } }
        };


        if (MirroredAvatar != null)
        {
            animator = MirroredAvatar.GetComponent<Animator>();
            avatarBoneMirror = MirroredAvatar.GetComponent<AvatarBoneMirror>();
            avatarBoneMirror.enabled = true;
        }
        else
        {
            Debug.LogError("MirroredAvatar GameObject is not assigned!");
            return;
        }
        
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject!");
            return;
        }

        // TEST CODE 
        // StartCoroutine(CallTuronOnButtonAfterDelay(10f));
        // StartCoroutine(CallTuronOffButtonAfterDelay(20f));
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
        string tempKey = "anger";
        Debug.Log("Turn on Anger Animation");
        
        ResetAllEmotions();
        emotionStates[tempKey] = true;

        // emotionAnimations.TryGetValue(tempKey, out currentAnimation);
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
        string tempKey = "anger";
        Debug.Log("Turn off Anger Animation");

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


                if (emotionAnimations.TryGetValue(key, out List<AnimationClip> animationList))
                {   
                    if (key == "joy"){
                        currentAnimation = animationList[joyCount%3];
                        joyCount += 1;
                    }
                    else{
                        currentAnimation = animationList[0];
                    }
                }
               
                TrackingToEmotion(key, currentAnimation);
            }
        }

        if (MirroredAvatar != null)
        {
            animator = MirroredAvatar.GetComponent<Animator>();
            // mirroredAvatar.SetActive(false);
        }
        else
        {
            Debug.LogError("MirroredAvatar GameObject is not assigned!");
            return;
        }
        
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject!");
            return;
        }
    }

    private void ResetAllEmotions()
    {
        List<string> keys = new List<string>(emotionStates.Keys);

        foreach (var key in keys)
        {
            emotionStates[key] = false;
        }
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
        Debug.Log($"TrackingToEmotion: CASE #1: Turn on {key}'s animation.");
        playableGraph = PlayableGraph.Create("AnimationGraph");
        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
        var animationPlayable = AnimationClipPlayable.Create(playableGraph, newClip);

        playableOutput.SetSourcePlayable(animationPlayable);
        avatarBoneMirror.enabled = false;

        playableGraph.Play();
    }

    // CASE #2: EMOTION A -> EMOTION B
    void EmotionToEmotion(string fromKey, string toKey)
    {
        Debug.Log($"1. EmotionToEmotion: CASE #2: Smooth transitioning from {fromKey} to {toKey}.");

        if (emotionAnimations.TryGetValue(fromKey, out var fromClips) &&
            emotionAnimations.TryGetValue(toKey, out var toClips))
        {
            // Check if the graph is already running
            if (playableGraph.IsValid())
            {
                var currentPlayable = (Playable)playableGraph.GetRootPlayable(0);
                float currentTime = (float)currentPlayable.GetTime();
                playableGraph.Destroy();

                // Create a new PlayableGraph
                playableGraph = PlayableGraph.Create("AnimationTransitionGraph");
                var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);

                // Create Playables for both animations
                var fromPlayable = AnimationClipPlayable.Create(playableGraph, fromClips[0]);
                fromPlayable.SetTime(currentTime);
                var toPlayable = AnimationClipPlayable.Create(playableGraph, toClips[0]);

                // Use a MixerPlayable to blend animations
                var mixerPlayable = AnimationMixerPlayable.Create(playableGraph, 2);

                // Connect playables to mixer
                playableGraph.Connect(fromPlayable, 0, mixerPlayable, 0);
                playableGraph.Connect(toPlayable, 0, mixerPlayable, 1);

                // Set input weights for transition (smooth transition)
                mixerPlayable.SetInputWeight(0, 1f);
                mixerPlayable.SetInputWeight(1, 0f);

                playableOutput.SetSourcePlayable(mixerPlayable);
                playableGraph.Play();

                StartCoroutine(TransitionAnimation(mixerPlayable));
            }
        }
        else
        {
            Debug.LogError($"Error: Invalid animation clips for transition from {fromKey} to {toKey}");
        }
    }

    private IEnumerator TransitionAnimation(AnimationMixerPlayable mixerPlayable)
    {
        float transitionDuration = 1.0f;  // Set your desired transition duration here
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float weight = elapsedTime / transitionDuration;
            mixerPlayable.SetInputWeight(0, 1f - weight);
            mixerPlayable.SetInputWeight(1, weight);
            yield return null;
        }

        // Ensure final state is fully on the new animation
        mixerPlayable.SetInputWeight(0, 0f);
        mixerPlayable.SetInputWeight(1, 1f);
    }

    // CASE #3: EMOTION A -> EMOTION A (TURN OFF EMOTION A)
    void EmotionToTracking(string key){
        Debug.Log($"EmotionToTracking: CASE #3: Turn off {key}'s animation.");
        playableGraph.Destroy();
        avatarBoneMirror.enabled = true;
    }
}