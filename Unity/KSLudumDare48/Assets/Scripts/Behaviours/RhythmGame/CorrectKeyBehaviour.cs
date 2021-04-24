namespace KasJam.LD48.Unity.Behaviours.RhythmGame
{
    using UnityEngine;

    [AddComponentMenu("LD48/CorrectKey")]
    public class CorrectKeyBehaviour : BehaviourBase
    {
        #region Members

        public Animator Animator;

        #endregion

        #region Animation Callbacks

        public override void AnimationFinished(int value)
        {
            //gameObject
                //.SetActive(false);

        }

        #endregion
    }
}