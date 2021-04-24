namespace KasJam.LD48.Unity.Behaviours.RhythmGame
{
    using KasJam.LD48.Unity.Behaviours.Music;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("LD48/SongVisualizer")]
    public class SongVisualizerBehaviour : BehaviourBase
    {
        #region Members

        public KeyCode[] KeyCodes;

        public RectTransform ScrollingPanel;

        public CorrectKeyBehaviour Correct;

        public CorrectKeyBehaviour Incorrect;

        public KeyboardKeyBehaviour Prefab;

        public RoundOverPanelBehaviour RoundOverPanel;

        public Text ScoreText;

        protected List<KeyboardKeyBehaviour> KeyboardKeys { get; set; }

        protected List<KeyboardKeyBehaviour> ToDestroy { get; set; }

        protected Sprite[] KeyboardSprites { get; set; }

        protected float SpawnX { get; set; }

        protected LevelManagerBehaviour LevelManager { get; set; }

        protected SongPlayerBehaviour SongPlayer { get; set; }

        protected int TotalSpawns { get; set; }

        protected int GoodKeyPresses { get; set; }

        protected int Score { get; set; }

        protected bool IsCheckingForOver { get; set; }

        #endregion

        #region Event Handlers

        private void LevelManager_SongStarted(object sender, System.EventArgs e)
        {
            Reset();
        }

        private void SongPlayer_SongFinished(object sender, System.EventArgs e)
        {
            if (LevelManager.IsFanfare)
            {
                return;
            }

            IsCheckingForOver = true;
        }

        private void SongPlayer_SongEventPlayed(object sender, SongEventEventArgs e)
        {
            if (LevelManager.IsFanfare)
            {
                return;
            }

            if (e.SongEvent.OccursAt >= LevelManager.CurrentSong.TotalTime * 0.9f)
            {
                return;
            }

            if (Random.value >= 0.25f)
            {
                SpawnKey(e.SongEvent);
            }
        }


        #endregion

        #region Protected Members

        protected void CheckForOver()
        {
            if (!KeyboardKeys
                .Any(o => !o.HasBeenActivated))
            {
                IsCheckingForOver = false;

                bool wasSuccessful = (float)GoodKeyPresses / (float)TotalSpawns >= 0.8f;

                RoundOverPanel
                    .Open(wasSuccessful, GoodKeyPresses, TotalSpawns);

                if (wasSuccessful)
                {
                    DoAfter(3, () =>
                    {
                        RoundOverPanel
                            .Close();

                        LevelManager
                            .AdvanceLevel();
                    });
                }
            }
        }

        protected private void Reset()
        {
            foreach (var keyboardKey in KeyboardKeys)
            {
                if (keyboardKey == null || keyboardKey.gameObject == null)
                {
                    continue;
                }

#if UNITY_EDITOR
                DestroyImmediate(keyboardKey.gameObject);
#else
                Destroy(keyboardKey.gameObject);
#endif
            }

            KeyboardKeys
                .Clear();
            
            IsCheckingForOver = false;
            SpawnX = 1200;
            TotalSpawns = 0;
        }

        protected void ActivateButton(KeyboardKeyBehaviour button, bool wasSuccessful)
        {
            if (button.HasBeenActivated)
            {
                return;
            }

            button.HasBeenActivated = true;
            button.Image.sprite = KeyboardSprites[button.SpriteIndex + 1];

            if (wasSuccessful)
            {
                Correct
                    .gameObject
                    .SetActive(true);

                GoodKeyPresses++;

                Score += LevelManager.LevelNumber * 10;

                ScoreText.text = $"Score: {Score}";
            }
            else
            {
                Incorrect
                    .gameObject
                    .SetActive(true);
            }
        }

        protected void SpawnKey(SongEvent songEvent)
        {
            var button = Instantiate(Prefab);

            button
                .transform
                .SetParent(ScrollingPanel);

            button.Image.rectTransform.anchoredPosition = new Vector2(SpawnX, 0);

            KeyboardKeys
                .Add(button);

            float moveOver = 100 + ((50 - LevelManager.LevelNumber) * 2);
            SpawnX += moveOver;

            int i = Random.Range(0, 8);
            int spriteIndex = i * 2;
            button.Image.sprite = KeyboardSprites[spriteIndex];
            button.SpriteIndex = spriteIndex;
            button.HasBeenActivated = false;
            button.KeyCode = KeyCodes[i];

            TotalSpawns++;
        }

        #endregion

        #region Unity 

        protected override void Awake()
        {
            base
                .Awake();

            Score = 0;

            KeyboardKeys = new List<KeyboardKeyBehaviour>();
            ToDestroy = new List<KeyboardKeyBehaviour>();

            KeyboardSprites = Resources
                .LoadAll<Sprite>("Images/UI/keyboardButtons");

            LevelManager = FindObjectOfType<LevelManagerBehaviour>();
            LevelManager.SongStarted += LevelManager_SongStarted;

            SongPlayer = FindObjectOfType<SongPlayerBehaviour>();
            SongPlayer.SongEventPlayed += SongPlayer_SongEventPlayed;
            SongPlayer.SongFinished += SongPlayer_SongFinished;

            Reset();
        }

        protected void HandleKeyPress(KeyCode keyCode)
        {
            var key = KeyboardKeys
                .Where(o =>
                    !o.HasBeenActivated &&
                    o.Image.rectTransform.anchoredPosition.x >= 0 &&
                    o.Image.rectTransform.anchoredPosition.x <= 200)
                .OrderBy(o => o.Image.rectTransform.anchoredPosition.x)
                .FirstOrDefault();

            if (key == null)
            {
                return;
            }

            if (key.KeyCode != keyCode)
            {
                return;
            }

            if (key.Image.rectTransform.anchoredPosition.x <= 100)
            {
                ActivateButton(key, true);
            }
        }

        protected bool WasKeyPressed(KeyCode keyCode)
        {
            if (Input.GetKeyDown(keyCode))
            {
                return true;
           }
            else
            {
                return false;
            }
        }

        protected void CheckInput()
        {
            if (WasKeyPressed(KeyCode.A))
            {
                HandleKeyPress(KeyCode.A);
            }
            else if (WasKeyPressed(KeyCode.W))
            {
                HandleKeyPress(KeyCode.W);
            }
            else if (WasKeyPressed(KeyCode.S))
            {
                HandleKeyPress(KeyCode.S);
            }
            else if (WasKeyPressed(KeyCode.D))
            {
                HandleKeyPress(KeyCode.D);
            }
            else if (WasKeyPressed(KeyCode.LeftArrow))
            {
                HandleKeyPress(KeyCode.LeftArrow);
            }
            else if (WasKeyPressed(KeyCode.RightArrow))
            {
                HandleKeyPress(KeyCode.RightArrow);
            }
            else if (WasKeyPressed(KeyCode.UpArrow))
            {
                HandleKeyPress(KeyCode.UpArrow);
            }
            else if (WasKeyPressed(KeyCode.DownArrow))
            {
                HandleKeyPress(KeyCode.DownArrow);
            }
        }

        protected void Update()
        {         
            ToDestroy
                .Clear();

            CheckInput();

            float moveSpeed = 400 + (LevelManager.LevelNumber * 25);
            moveSpeed *= Time.deltaTime;
            //moveSpeed /= 8.0f;

            SpawnX -= moveSpeed;
            if (SpawnX <= 1200)
            {
                SpawnX = 1200;
            }

            foreach (var keyboardKey in KeyboardKeys)
            {
                bool isPositiveX = false;

                var pos = keyboardKey.Image.rectTransform.anchoredPosition;

                if (pos.x > 0)
                {
                    isPositiveX = true;
                }

                pos.x -= moveSpeed;

                if (pos.x <= 0 && isPositiveX && !keyboardKey.HasBeenActivated)
                {
                    ActivateButton(keyboardKey, false);
                }

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

            if (IsCheckingForOver)
            {
                CheckForOver();
            }
        }

#endregion
    }
}