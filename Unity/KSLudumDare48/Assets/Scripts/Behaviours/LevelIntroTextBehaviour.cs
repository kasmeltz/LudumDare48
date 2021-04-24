namespace KasJam.LD48.Unity.Behaviours
{
    using KasJam.LD48.Unity.Behaviours.Music;
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("LD48/LevelIntroText")]
    public class LevelIntroTextBehaviour : BehaviourBase
    {
        #region Members

        public Text Text;

        public Animator Animator;

        public LevelManagerBehaviour LevelManager;

        #endregion

        #region Event Handlers

        private void LevelManager_LevelStarted(object sender, System.EventArgs e)
        {
            UpdateUI();
        }

        #endregion

        #region Protected Methods

        protected void UpdateUI()
        {
            var note = LevelManager.CurrentNote;

            Text.text = $"{note.Name} {note.Octave}";

            int noteStep = MusicalScale
                .GetNoteAbsoluteIndex(note.Name, LevelManager.MaxOctave, note.Octave);

            int anchorY = -(noteStep * 18) - 30;
            var rt = Text
                .rectTransform;
            var pos = rt.anchoredPosition;
            pos.y = anchorY;
            rt.anchoredPosition = pos;

            float ratio = (noteStep / 60f);

            Text.color = Color
                .Lerp(LevelManager.StartColor, LevelManager.EndColor, ratio);

            Animator
                .SetTrigger("DoIntro");
        }

        #endregion

        #region Unity

        protected override void Awake()
        {
            base
                .Awake();

            LevelManager = FindObjectOfType<LevelManagerBehaviour>();

            LevelManager.LevelStarted += LevelManager_LevelStarted;

            Text.text = "";
        }

        #endregion
    }
}