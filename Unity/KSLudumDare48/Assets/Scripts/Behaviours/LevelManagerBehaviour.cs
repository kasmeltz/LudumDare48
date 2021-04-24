namespace KasJam.LD48.Unity.Behaviours
{
    using KasJam.LD48.Unity.Behaviours.Music;
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

        #endregion

        #region Protected Methods
   
        protected void MakeSong()
        {
            SongComposer composer = new SongComposer();

            int rootIndex = Random
                .Range(0, MusicalScale.NoteOrder.Count);

            string root = MusicalScale.NoteOrder[rootIndex];

            int octave = Random
                .Range(2, 5);

            int modeIndex = Random
                .Range(1, 10);

            //var song = composer
            //.ComposeSong(root, octave, (ScaleType)modeIndex, 1);

            var song = composer
                .ComposeSong(CurrentNote.Name, CurrentNote.Octave, ScaleType.Major, 0.02f);

            SongPlayer
                .SetSong(song);

            SongPlayer
                .StartPlaying();

            UpdateUI();
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

            MakeSong();
        }

        private void SongPlayer_SongFinished(object sender, System.EventArgs e)
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

            DoAfter(1, () =>
            {
                MakeSong();
            });
        }

        /*
        protected void Update()
        {
            BeatCounter += Time.deltaTime;

            if (BeatCounter >= 1)
            {
                BeatCounter -= 1;

                PlayNote("C3");
            }
        }
        */

        #endregion
    }
}