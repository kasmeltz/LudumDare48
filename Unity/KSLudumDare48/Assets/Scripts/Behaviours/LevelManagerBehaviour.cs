namespace KasJam.LD48.Unity.Behaviours
{
    using KasJam.LD48.Unity.Behaviours.Music;
    using UnityEngine;

    [AddComponentMenu("LD48/LevelManager")]
    public class LevelManagerBehaviour : BehaviourBase
    {
        #region Members

        public SongPlayerBehaviour SongPlayer;

        protected MusicalNote CurrentNote { get; set; }

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
                .ComposeSong(CurrentNote.Name, CurrentNote.Octave, ScaleType.Major, 0.25f);

            SongPlayer
                .SetSong(song);

            SongPlayer
                .StartPlaying();

        }

        #endregion

        #region Unity

        protected override void Awake()
        {
            base
                .Awake();

            SongComposer composer = new SongComposer();

            SongPlayer.SongFinished += SongPlayer_SongFinished;

            CurrentNote = new MusicalNote("C", 4, NoteTimbre.Ah);

            MakeSong();
        }

        private void SongPlayer_SongFinished(object sender, System.EventArgs e)
        {
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