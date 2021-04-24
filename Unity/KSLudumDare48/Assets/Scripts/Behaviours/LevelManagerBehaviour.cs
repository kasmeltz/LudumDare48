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

        public Image Background;

        public SongPlayerBehaviour SongPlayer;

        public Text LevelNameText;

        public int MaxOctave;

        public Color StartColor;

        public Color EndColor;

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

        public void AdvanceLevel()
        {
            LevelNumber++;

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

        protected void StartLevel()
        {
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

            int modeIndex = UnityEngine
                    .Random
                    .Range(1, 10);

            ShortestNote = 0.5f - (LevelNumber * 0.008f);

            CurrentSong = composer
                .ComposeSong(CurrentNote.Name, CurrentNote.Octave, (ScaleType)modeIndex, ShortestNote);

            CurrentSong.TotalTime += 1;

            SongPlayer
                .SetSong(CurrentSong);

            SongPlayer
                .StartPlaying();

            OnSongStarted();
        }

        protected void UpdateUI()
        {
            Background.sprite = BackgroundSprites[LevelNumber];
        }

        #endregion

        #region Unity

        protected override void Awake()
        {
            base
                .Awake();

            LevelNumber = 0;

            BackgroundSprites = Resources
                .LoadAll<Sprite>("Images/UI/beachsunset");

            SongComposer composer = new SongComposer();

            SongPlayer.SongFinished += SongPlayer_SongFinished;

            CurrentNote = new MusicalNote("C", 5, NoteTimbre.Ah);

            StartLevel();
        }

        private void SongPlayer_SongFinished(object sender, System.EventArgs e)
        {
            if (IsFanfare)
            {
                MakeSong();
                return;
            }
        }

        #endregion
    }
}