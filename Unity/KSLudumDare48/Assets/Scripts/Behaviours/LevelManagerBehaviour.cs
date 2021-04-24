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

        #region Unity

        protected override void Awake()
        {
            base
                .Awake();

            SongComposer composer = new SongComposer();

            var song = composer
                .ComposeSong("C", ScaleType.Major, 1);

            SongPlayer
                .SetSong(song);

            SongPlayer
                .StartPlaying();
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