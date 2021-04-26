namespace KasJam.LD48.Unity.Behaviours
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("LD48/BodyPartCallOut")]
    public class BodyPartCallOutBehaviour : BehaviourBase
    {
        #region Members

        public Image Callout;
        
        public Text BodyPartName;

        protected Sprite[] CalloutSprites { get; set; }

        protected LevelManagerBehaviour LevelManager { get; set; }

        protected static List<string> BodyPartNames = new List<string>
        {
            "Crown",
            "Brain",
            "Forehead",
            "Skull",
            "Left Eyebrow",
            "Right Eyebrow",
            "Left Eye",
            "Right Eye",
            "Nose",
            "Left Ear",
            "Right Ear",
            "Teeth",
            "Uvula",
            "Chin",
            "Left Shoulder",
            "Right Shoulder",
            "Chest",
            "Left Lung",
            "Right Lung",
            "Heart",
            "Left Bicep",
            "Right Bicep",
            "Elbow",
            "Left Forearm",
            "Right Forearm",
            "Thumb",
            "Index Finger",
            "Middle Finger",
            "Ring Finger",
            "Pinky Finger",           
            "Knuckles",
            "Abs",
            "Left Kidney",
            "Right Kidney",
            "Stomach",
            "Hips",
            "Groin",
            "Left Thigh",
            "Right Thigh",
            "Hamstrings",
            "Knees",
            "Left Calf",
            "Right Calf",
            "Left Ankle",
            "Right Ankle",
            "Left Foot",
            "Right Foot",
            "Root Chakra",
            "Sacral Chakra",
            "Solar Plexus Chakra",
            "Heart Chakra",
            "Throat Chakra",
            "3rd Eye Chakra",
            "Crown Chakra",
            "Soul Star Chakra",
            "Spirit Star Chakra",
            "Universal Chakra",
            "Galactic Chakra",
            "Divnine Gateway Chakra"
        };

        #endregion      

        #region Event Handlers

        private void LevelManager_LevelStarted(object sender, System.EventArgs e)
        {
            int level = LevelManager.LevelNumber;

            if (level >= CalloutSprites.Length)
            {
                return;
            }

            var sprite = CalloutSprites[level];
            Callout.sprite = sprite;
            BodyPartName.text = BodyPartNames[level];
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Unity

        protected override void Awake()
        {
            base
                .Awake();

            CalloutSprites = Resources
                .LoadAll<Sprite>("Images/UI/singerbodyparts");

            LevelManager = FindObjectOfType<LevelManagerBehaviour>();
            LevelManager.LevelStarted += LevelManager_LevelStarted;
        }

        #endregion
    }
}