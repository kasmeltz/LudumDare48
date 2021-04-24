namespace KasJam.LD48.Unity.Behaviours
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [AddComponentMenu("LD48/ChangeScene")]
    public class ChangeSceneBehaviour : BehaviourBase
    {
        #region Public Methods

        public void ChangeScene(string sceneName)
        {
            SceneManager
                .LoadSceneAsync(sceneName);
        }
        #endregion
    }
}