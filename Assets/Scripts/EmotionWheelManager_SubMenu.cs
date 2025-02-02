using System.Collections;
using System.Collections.Generic;

using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.UI;

using ReadyPlayerMe.MetaMovement;

public class EmotionWheelManager_SubMenu : MonoBehaviour
{   
    [System.Serializable] 
    public class Emotion{
        public string name;
        [HideInInspector] public string colorCode;
        [HideInInspector] public UltimateRadialButtonInfo buttonInfo;
        public AnimationClip[] lowAnimations;
        public AnimationClip[] midAnimations;
        public AnimationClip[] highAnimations;
    }

    [System.Serializable]
    public class Intensity{
        public string name;
        [HideInInspector] public string colorCode;
        [HideInInspector] public UltimateRadialSubButtonInfo subButtonInfo;
    }

    [Header("Radial Menus")]
    public string radialMenuName = "EmotionWheel";
    UltimateRadialMenu radialMenu;
    UltimateRadialSubmenu subMenu;


    [Header("Emotion Lists")]
    public Emotion[] Emotions;
    public Intensity[] Intensities;

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


    void Start(){
        // Store the radial and sub menus.
        radialMenu = UltimateRadialMenu.ReturnComponent( radialMenuName );
        subMenu = UltimateRadialSubmenu.ReturnComponent( radialMenuName );

        // Loop through all the emotion types assigned...
        for( int i = 0; i < Emotions.Length; i++ )
        {
            // Store the button id for reference and register the object to the radial menu.
            emotionColors.TryGetValue(Emotions[i].name.ToLower(), out Emotions[i].colorCode);
            Emotions[i].buttonInfo.id = i;
            Emotions[i].buttonInfo.name = Emotions[i].name;
            Emotions[i].buttonInfo.key = Emotions[i].name;
            radialMenu.RegisterButton(UpdateAnimation, Emotions[i].buttonInfo);
        }

        SetRadialButtonColor();
    }

    void Update(){

    }

    public void UpdateEmotionType(int id){

        // Select this button exclusively on the radial menu.
        Emotions[id].buttonInfo.SelectButton(true);

        // Clear the sub menu.
        submenu.ClearMenu();

        // Loop through all the intensity options.
        for( int i = 0; i < Intensities.Length; i++ )
        {
            // Store the id of this option into the button info.
            Intensities[i].subButtonInfo.id = i;
            Intensities[i].subButtonInfo.name = Intensities[i].name;
            Intensities[i].subButtonInfo.key = Intensities[i].name;
            submenu.RegisterButton(UpdateIntensity, Intensities[i].buttonInfo);
        }
    }

    public void UpdateIntensity(int id){

    }

    void SetRadialButtonColor()
    {
        foreach (Transform buttonTransform in radialMenu.transform)
        {
            Image buttonImage = buttonTransform.GetComponent<Image>();
            if (buttonImage == null){
                continue;
            }
                
            Text buttonText = buttonTransform.GetComponentInChildren<Text>();
            if (buttonText == null){
                continue;
            }

            string textValue = buttonText.text.ToLower();
            Color newColor = Color.gray;

            foreach (var emotion in Emotions)
            {
                if (textValue.Contains(emotion.name.ToLower()))
                {
                    // Debug.Log($"Hyuna: {emotion.name}'s ColorCode is {emotion.colorCode}.");
                    ColorUtility.TryParseHtmlString(emotion.colorCode, out newColor);
                    break; 
                }
            }

            buttonImage.color = newColor;
        }
    }
}