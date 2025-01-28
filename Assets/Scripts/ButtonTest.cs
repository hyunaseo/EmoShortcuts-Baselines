using System.Collections;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour
{
    [Header("Radial Menu Configuration")]
    public UltimateRadialMenu radialMenu;
    public List<UltimateRadialButtonInfo> buttonInfos = new List<UltimateRadialButtonInfo>();
    
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
    public AnimationClip JoyAnimationClip;
    public AnimationClip FearAnimationClip;
    public AnimationClip SadAnimationClip;
    public AnimationClip SurpriseAnimationClip;
    public AnimationClip NeutralAnimationClip;

    private Dictionary<string, AnimationClip> emotionAnimations;
    private AnimationClip currentAnimation;
    
    [Header("Emotion States")]
    private Dictionary<string, bool> emotionStates = new Dictionary<string, bool>();
    private bool isAnimationOn;

    [Header("Avatar GameObjects")]
    public GameObject mirroredAvatar;
    public GameObject OriginalAvatarFace;
    public GameObject OriginalAvatarBody;

    [Header("Animation System")]
    private Animator animator;
    private PlayableGraph playableGraph;

    void Start()
    {
        foreach (var buttonInfo in buttonInfos)
        {
            radialMenu.RegisterButton(TurnOnAnimation, buttonInfo);
        }

        ChangeColor();

        emotionAnimations = new Dictionary<string, AnimationClip>()
        {
            { "anger", AngerAnimationClip },
            { "disgust", DisgustAnimationClip },
            { "joy", JoyAnimationClip },
            { "fear", FearAnimationClip },
            { "sad", SadAnimationClip },
            { "surprise", SurpriseAnimationClip },
            { "neutral", NeutralAnimationClip }
        };
        
        foreach (var emotion in emotionColors.Keys)
        {
            emotionStates[emotion] = false;
        }


        if (mirroredAvatar != null)
        {
            animator = mirroredAvatar.GetComponent<Animator>();
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

    void TurnOnAnimation ( string key )
	{
        key = key.ToLower();
        emotionStates.TryGetValue(key, out isAnimationOn);

        if (!isAnimationOn){ //isAnimationOn == false;
            // This is where you can check your item dictionary.
		    Debug.Log( $"Hyuna 1. TurnOnAnimation called for: {key}" );

            if (emotionAnimations.TryGetValue(key, out currentAnimation))
            {
                Debug.Log($"Hyuna 2. Current animation set to: {currentAnimation.name}");
            }
            else
            {
                Debug.LogWarning($"Hyuna 2. No animation found for key: {key}");
                currentAnimation = null;
            }

            OriginalAvatarFace.SetActive(false);
            OriginalAvatarBody.SetActive(false);

            // mirroredAvatar.SetActive(true);

            PlayAnimationClip(currentAnimation);
        }

        else{ // isAnimationOn == true;
          Debug.Log( $"Hyuna 1. TurnOFFAnimation called for: {key}" );

            emotionStates[key] = false;

            OriginalAvatarFace.SetActive(true);
            OriginalAvatarBody.SetActive(true);

            // mirroredAvatar.SetActive(false);

            playableGraph.Destroy();
        }    
	}

    void PlayAnimationClip(AnimationClip newClip)
    {
        if (playableGraph.IsValid())
        {
            if (playableGraph.IsValid())
            {
                Playable rootPlayable = playableGraph.GetRootPlayable(0);

                if (rootPlayable.IsPlayableOfType<AnimationClipPlayable>())
                {
                    var currentPlayable = (AnimationClipPlayable)rootPlayable;
                    Debug.Log($"Hyuna 3. Transitioning from animation: {currentPlayable.GetAnimationClip()?.name ?? "None"} to {newClip.name}");
                }
                else
                {
                    Debug.Log($"Hyuna 3. Transitioning from a non-clip playable to {newClip.name}");
                }

                StartCoroutine(TransitionToNewAnimation(newClip, 1.0f)); // Smooth transition over 1 second
            }
        }
        else
        {
            Debug.Log($"Hyuna 3. No previous animation. Playing animation: {newClip.name}");
            // If no existing graph, play animation directly
            playableGraph = PlayableGraph.Create("AnimationGraph");
            var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
            var animationPlayable = AnimationClipPlayable.Create(playableGraph, newClip);

            playableOutput.SetSourcePlayable(animationPlayable);
            playableGraph.Play();
        }
    }

    IEnumerator TransitionToNewAnimation(AnimationClip newClip, float transitionDuration)
    {
        var currentPlayable = (AnimationClipPlayable)playableGraph.GetRootPlayable(0);

        // Create a mixer to blend between the current and new animation
        var mixer = AnimationMixerPlayable.Create(playableGraph, 2);
        playableGraph.Connect(currentPlayable, 0, mixer, 0);
        
        var newPlayable = AnimationClipPlayable.Create(playableGraph, newClip);
        playableGraph.Connect(newPlayable, 0, mixer, 1);

        // Set the mixer as the playable output
        var output = playableGraph.GetOutput(0);
        output.SetSourcePlayable(mixer);

        // Blend from current animation to new animation smoothly
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            float blend = elapsedTime / transitionDuration;
            mixer.SetInputWeight(0, 1.0f - blend);
            mixer.SetInputWeight(1, blend);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the new animation is fully active
        mixer.SetInputWeight(0, 0);
        mixer.SetInputWeight(1, 1);

        // Cleanup previous animation
        currentPlayable.Destroy();
    }

    void ChangeColor()
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
}