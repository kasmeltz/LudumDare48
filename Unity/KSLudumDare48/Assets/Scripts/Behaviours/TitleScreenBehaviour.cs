namespace KasJam.LD48.Unity.Behaviours
{
    using KasJam.LD48.Unity.Behaviours.Music;
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("LD48/TitleScreen")]
    public class TitleScreenBehaviour : BehaviourBase
    {
        #region Members

        public Image TitlePanel;

        public Image StoryPanel;

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

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (TitlePanel.gameObject.activeInHierarchy)
                {
                    TitlePanel
                        .gameObject
                        .SetActive(false);

                    StoryPanel
                        .gameObject
                        .SetActive(true);
                }
                else
                {
                    TitlePanel
                        .gameObject
                        .SetActive(true);

                    StoryPanel
                        .gameObject
                        .SetActive(false);
                }
            }
        }

        #endregion
    }
}