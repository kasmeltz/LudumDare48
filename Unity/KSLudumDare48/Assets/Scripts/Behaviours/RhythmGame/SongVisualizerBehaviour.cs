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

        protected List<KeyboardKeyBehaviour> ToDestroy { get; set; }

        public Sprite[] KeyboardSprites { get; set; }

        protected LevelManagerBehaviour LevelManager { get; set; }

        #endregion

        #region Event Handlers

        private void LevelManager_SongStarted(object sender, System.EventArgs e)
        {
            foreach(var keyboardKey in KeyboardKeys)
            {
                if (keyboardKey == null)
                {
                    continue;
                }

                Destroy(keyboardKey);
            }

            KeyboardKeys
                .Clear();

            var button = Instantiate(Prefab);
            button.Image.sprite = KeyboardSprites[0];

            button
                .transform
                .SetParent(ScrollingPanel);

            button.Image.rectTransform.anchoredPosition = new Vector2(1200, 0);

            KeyboardKeys
                .Add(button);
        }

        #endregion

        #region Unity 

        protected override void Awake()
        {
            base
                .Awake();

            KeyboardKeys = new List<KeyboardKeyBehaviour>();
            ToDestroy = new List<KeyboardKeyBehaviour>();

            KeyboardSprites = Resources
                .LoadAll<Sprite>("Images/UI/keyboardButtons");

            LevelManager = FindObjectOfType<LevelManagerBehaviour>();

            LevelManager.SongStarted += LevelManager_SongStarted;
        }
        protected void Update()
        {
            ToDestroy
                .Clear();

            var moveSpeed = 400 + (LevelManager.LevelNumber * 25);

            foreach (var keyboardKey in KeyboardKeys)
            {
                var pos = keyboardKey.Image.rectTransform.anchoredPosition;

                pos.x -= moveSpeed * Time.deltaTime;

                keyboardKey.Image.rectTransform.anchoredPosition = pos;

                if (pos.x <= -1200)
                {
                    ToDestroy
                        .Add(keyboardKey);
                }
            }

            foreach(var keyboardKey in ToDestroy)
            {
                KeyboardKeys
                    .Remove(keyboardKey);

                Destroy(keyboardKey);
            }
        }

        #endregion
    }
}