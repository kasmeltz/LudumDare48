namespace KasJam.LD48.Unity.Behaviours.RhythmGame
{
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("LD48/SongVisualizer")]
    public class SongVisualizerBehaviour : BehaviourBase
    {
        #region Members

        public RectTransform ScrollingPanel;

        public KeyboardKeyBehaviour Prefab;

        protected List<KeyboardKeyBehaviour> KeyboardKeys { get; set; }

        public Sprite[] KeyboardSprites { get; set; }

        protected LevelManagerBehaviour LevelManager { get; set; }

        #endregion

        #region Event Handlers

        private void LevelManager_SongStarted(object sender, System.EventArgs e)
        {

        }

        #endregion

        #region Unity 

        protected override void Awake()
        {
            base
                .Awake();

            KeyboardSprites = Resources
                .LoadAll<Sprite>("Images/UI/keyboardButtons");

            LevelManager = FindObjectOfType<LevelManagerBehaviour>();

            LevelManager.SongStarted += LevelManager_SongStarted;
        }

        #endregion
    }
}