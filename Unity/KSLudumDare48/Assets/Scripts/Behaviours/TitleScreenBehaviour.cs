namespace KasJam.LD48.Unity.Behaviours
{
    using KasJam.LD48.Unity.Behaviours.Music;
    using UnityEngine;

    [AddComponentMenu("LD48/TitleScreen")]
    public class TitleScreenBehaviour : BehaviourBase
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
           
            float shortestNote = 0.25f;

            var song = composer
                .ComposeSong("C", 4, shortestNote);

            SongPlayer
                .SetSong(song);

            SongPlayer
                .StartPlaying();
        }

        #endregion
    }
}