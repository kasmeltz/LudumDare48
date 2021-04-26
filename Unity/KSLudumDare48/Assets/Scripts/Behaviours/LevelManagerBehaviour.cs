namespace KasJam.LD48.Unity.Behaviours
{
    using KasJam.LD48.Unity.Behaviours.Music;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("LD48/LevelManager")]
    public class LevelManagerBehaviour : BehaviourBase
    {
        #region Members

        public Image WinGamePanel;

        public Image Background;

        public SongPlayerBehaviour SongPlayer;

        public Text LevelNameText;

        public int MaxOctave;

        public Color StartColor;

        public Color EndColor;

        public Animator Singer;

        public GameObject RoundOverPanel;

        public Sprite[] BackgroundSprites { get; set; }

        public MusicalNote CurrentNote { get; set; }

        public int LevelNumber { get; protected set; }

        public Song CurrentSong { get; protected set; }

        public bool IsFanfare { get; protected set; }

        public float ShortestNote { get; protected set; }

        #endregion

        #region Events

        public event EventHandler SongStarted;

        protected void OnSongStarted()
        {
            SongStarted?
                .Invoke(this, EventArgs.Empty);
        }

        public event EventHandler LevelStarted;

        protected void OnLevelStarted()
        {
            LevelStarted?
                .Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Public Methods

        public void WinGame()
        {
            WinGamePanel
                .gameObject
                .SetActive(true);
        }

        public void AdvanceLevel()
        {
            LevelNumber++;

            if (LevelNumber >= 59)
            {
                WinGame();
                return;
            }

            var octave = CurrentNote.Octave;
            var index = MusicalScale.NoteOrder.IndexOf(CurrentNote.Name);
            index--;
            if (index < 0)
            {
                index += 12;
                octave--;
            }
            var newPitch = MusicalScale.NoteOrder[index];
            CurrentNote = new MusicalNote(newPitch, octave, NoteTimbre.Ah);

            StartLevel();
        }

        #endregion

        #region Protected Methods

        protected void SetSingerState(bool isSinging)
        {
            Singer
                .SetBool("IsSinging", isSinging);

            Singer
                .SetBool("IsResting", !isSinging);
        }

        public void StartLevel()
        {
            RoundOverPanel
                .SetActive(false);

            SongComposer composer = new SongComposer();

            IsFanfare = true;

            var song = composer
                .ComposeFanfare(CurrentNote.Name, CurrentNote.Octave);

            SongPlayer
                .SetSong(song);

            SongPlayer
                .StartPlaying();

            UpdateUI();

            OnLevelStarted();
        }

        protected void MakeSong()
        {
            SongComposer composer = new SongComposer();

            IsFanfare = false;

            // TODO - CHANGE FOR TESTING
            // ShortestNote = 0.0625f;
            ShortestNote = 0.5f - (LevelNumber * 0.0035f);

            CurrentSong = composer
                .ComposeSong(CurrentNote.Name, CurrentNote.Octave, ShortestNote);

            CurrentSong.TotalTime += 1;

            SongPlayer
                .SetSong(CurrentSong);

            SongPlayer
                .StartPlaying();

            SetSingerState(true);

            OnSongStarted();
        }

        protected void UpdateUI()
        {
            if (LevelNumber < BackgroundSprites.Length)
            {
                Background.sprite = BackgroundSprites[LevelNumber];
            }
        }

        #endregion

        #region Unity

        protected override void Awake()
        {
            base
                .Awake();


            /*
            var chooser = new ProbabilityChooser<int>();
            chooser.AddItem(0, 0.1f);
            chooser.AddItem(1, 0.2f);
            chooser.AddItem(2, 0.3f);
            chooser.AddItem(3, 0.4f);

            int[] chosens = new int[4];

            for(int i = 0;i < 10000;i++)
            {
                int chosen = chooser
                    .ChooseItem();

                chosens[chosen]++;
            }


            Debug
                .Log($"0 had probability of 0.1 and was chosen {chosens[0]} times");


            Debug
                .Log($"1 had probability of 0.2 and was chosen {chosens[1]} times");


            Debug
                .Log($"2 had probability of 0.3 and was chosen {chosens[2]} times");


            Debug
                .Log($"3 had probability of 0.4 and was chosen {chosens[3]} times");
            */


            LevelNumber = 20;

            BackgroundSprites = Resources
                .LoadAll<Sprite>("Images/UI/beachsunset");

            SongComposer composer = new SongComposer();

            SongPlayer.SongFinished += SongPlayer_SongFinished;

            CurrentNote = new MusicalNote("C", 4, NoteTimbre.Ah);

            SetSingerState(false);

            StartLevel();
        }

        private void SongPlayer_SongFinished(object sender, System.EventArgs e)
        {
            if (IsFanfare)
            {
                MakeSong();
                return;
            }

            SetSingerState(false);
        }

        #endregion
    }
}