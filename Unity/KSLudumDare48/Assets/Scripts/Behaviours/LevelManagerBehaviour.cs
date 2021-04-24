namespace KasJam.LD48.Unity.Behaviours
{
    using KasJam.LD48.Unity.Behaviours.Music;
    using UnityEngine;

    [AddComponentMenu("LD48/LevelManager")]
    public class LevelManagerBehaviour : BehaviourBase
    {
        #region Members

        public SongPlayerBehaviour SongPlayer;

        #endregion

        #region Protected Methods

        protected void MakeSong()
        {
            SongComposer composer = new SongComposer();

            var song = composer
                .ComposeSong("D#", 2, ScaleType.Blues, 1);

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

            MakeSong();
        }

        private void SongPlayer_SongFinished(object sender, System.EventArgs e)
        {
            DoAfter(2, () =>
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