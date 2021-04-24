namespace KasJam.LD48.Unity.Behaviours
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [AddComponentMenu("LD48/ChangeSceneOnKeyPress")]
    public class ChangeSceneOnKeyPressBehaviour : BehaviourBase
    {
        #region Members

        public KeyCode KeyCode;

        public string SceneName;

        #endregion

        #region Unity

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode))
            {
                SceneManager
                    .LoadSceneAsync(SceneName);
            }
        }

        #endregion
    }
}