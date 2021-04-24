namespace KasJam.LD48.Unity.Behaviours.RhythmGame
{
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("LD48/KeyboardKey")]
    public class KeyboardKeyBehaviour : BehaviourBase
    {
        #region Members        

        public Image Image;

        public int SpriteIndex { get; set; }

        public bool HasBeenActivated { get; set; }

        public KeyCode KeyCode { get; set; }

        #endregion

        #region Unity 

        #endregion
    }
}