namespace KasJam.LD48.Unity.Behaviours
{
    using UnityEngine;

    [AddComponentMenu("LD48/ExitOnKeyPress")]
    public class ExitOnKeyPressBehaviour : BehaviourBase
    {
        #region Members

        public KeyCode KeyCode;

        #endregion

        #region Unity

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode))
            {
                Application
                    .Quit();
            }
        }

        #endregion
    }
}