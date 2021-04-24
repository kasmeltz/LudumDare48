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

        protected MusicalNote CurrentNote { get; set; }

        protected int LevelNumber { get; set; }

        public Song CurrentSong { get; protected set; }

        public bool IsFanfare { get; protected set; }

        #endregion

        #region Events

        public event EventHandler SongStarted;

        protected void OnSongStarted()
        {
            SongStarted?
                .Invoke(this, EventArgs.Empty);
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
        }

        protected void MakeSong()
        {
            SongComposer composer = new SongComposer();

            IsFanfare = false;

            int modeIndex = UnityEngine
                    .Random
                    .Range(1, 10);

            float shortestNote = UnityEngine
                .Random
                .Range(0.25f, 1f);

            CurrentSong = composer
                .ComposeSong(CurrentNote.Name, CurrentNote.Octave, (ScaleType)modeIndex, shortestNote);

            SongPlayer
                .SetSong(CurrentSong);

            SongPlayer
                .StartPlaying();

            OnSongStarted();
        }

        protected void UpdateUI()
        {
            LevelNameText.text = $"{CurrentNote.Name} {CurrentNote.Octave}";

            int noteStep = MusicalScale
                .GetNoteAbsoluteIndex(CurrentNote.Name, MaxOctave, CurrentNote.Octave);

            int anchorY = -(noteStep * 18) - 30;
            var rt = LevelNameText
                .GetComponent<RectTransform>();
            var pos = rt.anchoredPosition;
            pos.y = anchorY;
            rt.anchoredPosition = pos;

            float ratio = (noteStep / 60f);

            LevelNameText.color = Color
                .Lerp(StartColor, EndColor, ratio);

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

            DoAfter(1, () =>
            {
                StartLevel();
            });
        }

        #endregion
    }
}