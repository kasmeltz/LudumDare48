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

        public float BaseMoveSpeed;

        public float MoveSpeedPerLevel;

        public float SpawnTimer;

        protected List<KeyboardKeyBehaviour> KeyboardKeys { get; set; }

        protected List<KeyboardKeyBehaviour> ToDestroy { get; set; }

        protected Sprite[] KeyboardSprites { get; set; }

        protected float SpawnX { get; set; }

        protected LevelManagerBehaviour LevelManager { get; set; }

        protected SongPlayerBehaviour SongPlayer { get; set; }

        protected int BadKeyPresses { get; set; }

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

            if (SpawnTimer > 0)
            {
                return;
            }

            if (e.SongEvent.OccursAt >= LevelManager.CurrentSong.TotalTime * 0.9f)
            {
                return;
            }

            SpawnKey(e.SongEvent);
            SpawnTimer = 0.25f;
        }


        #endregion

        #region Protected Members

        protected void CheckForOver()
        {
            if (!KeyboardKeys
                .Any(o => !o.HasBeenActivated))
            {
                IsCheckingForOver = false;

                DoAfter(1, () =>
                {
                    int totalKeyPresses = GoodKeyPresses + BadKeyPresses;
                    bool wasSuccessful = (float)GoodKeyPresses / (float)totalKeyPresses >= 0.75f;

                    RoundOverPanel
                        .Open(wasSuccessful, GoodKeyPresses, totalKeyPresses);

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
                });
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
            SpawnTimer = 0;
            BadKeyPresses = 0;
            GoodKeyPresses = 0;
        }

        protected void ActivateButton(KeyboardKeyBehaviour button, bool wasSuccessful)
        {
            if (button.HasBeenActivated)
            {
                return;
            }

            button.HasBeenActivated = true;
            button.Image.sprite = KeyboardSprites[button.SpriteIndex + 1];

            CorrectKeyBehaviour key = null;

            if (wasSuccessful)
            {
                key = Correct;

                GoodKeyPresses++;

                Score += (LevelManager.LevelNumber + 1) * 10;

                ScoreText.text = $"Score: {Score}";
            }
            else
            {
                BadKeyPresses++;

                key = Incorrect;
            }

            key
                .Animator
                .ResetTrigger("ShowKey");

            key
                .Animator
                .SetTrigger("ShowKey");
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

            int maxNumber = 2;

            if (LevelManager.LevelNumber >= 45)
            {
                maxNumber = 8;
            
            }
            else if (LevelManager.LevelNumber >= 35)
            {
                maxNumber = 7;

            }
            else if (LevelManager.LevelNumber >= 25)
            {
                maxNumber = 6;

            }
            else if (LevelManager.LevelNumber >= 15)
            {
                maxNumber = 5;

            }
            else if (LevelManager.LevelNumber >= 8)
            {
                maxNumber = 4;

            }
            else if (LevelManager.LevelNumber >= 4)
            {
                maxNumber = 3;

            }

            int i = Random.Range(0, maxNumber);
            int spriteIndex = i * 2;
            button.Image.sprite = KeyboardSprites[spriteIndex];
            button.SpriteIndex = spriteIndex;
            button.HasBeenActivated = false;
            button.KeyCode = KeyCodes[i];
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
                    o.Image.rectTransform.anchoredPosition.x >= -80 &&
                    o.Image.rectTransform.anchoredPosition.x <= 80)
                .OrderBy(o => o.Image.rectTransform.anchoredPosition.x)
                .FirstOrDefault();

            if (key == null)
            {
                BadKeyPresses++;

                return;
            }

            if (key.KeyCode != keyCode)
            {
                BadKeyPresses++;

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

            if (SpawnTimer > 0)
            {
                SpawnTimer -= Time.deltaTime;
                if (SpawnTimer <= 0)
                {
                    SpawnTimer = 0;
                }
            }

            CheckInput();

            float moveSpeed = BaseMoveSpeed + (LevelManager.LevelNumber * MoveSpeedPerLevel);
            moveSpeed *= Time.deltaTime;

            SpawnX -= moveSpeed;
            if (SpawnX <= 1200)
            {
                SpawnX = 1200;
            }           

            foreach (var keyboardKey in KeyboardKeys)
            {
                var pos = keyboardKey.Image.rectTransform.anchoredPosition;

                pos.x -= moveSpeed;

                if (pos.x <= -80 && !keyboardKey.HasBeenActivated)
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