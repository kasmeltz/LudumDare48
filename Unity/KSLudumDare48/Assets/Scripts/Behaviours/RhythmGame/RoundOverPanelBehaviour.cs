namespace KasJam.LD48.Unity.Behaviours.RhythmGame
{
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("LD48/RoundOverPanel")]
    public class RoundOverPanelBehaviour : BehaviourBase
    {
        #region Members

        public Text SuccessText;
        public Text FailureText;
        public Button FailureButton;

//        public Text GoodKeyPressesText;
        //public Text TotalSpawnsText;
        public Text CorrectRatioText;

        #endregion

        #region Public Methods

        public void Open(bool wasSuccessful, int goodKeyPresses, int totalSpawns)
        {
            gameObject
                .SetActive(true);

            SuccessText
                .gameObject
                .SetActive(wasSuccessful);

            FailureText
                .gameObject
                .SetActive(!wasSuccessful);

            FailureButton
                .gameObject
                .SetActive(!wasSuccessful);

          //  GoodKeyPressesText.text = goodKeyPresses
                //.ToString();

            //TotalSpawnsText.text = totalSpawns
                //.ToString();

            float ratio = ((float)goodKeyPresses / (float)totalSpawns) * 1000;
            int ratioInt = Mathf
                .RoundToInt(ratio);
            ratio = ratioInt /  1000f;
            CorrectRatioText.text = $"{ratio:P}";
        }

        public void Close()
        {
            gameObject
                .SetActive(false);
        }
        
        #endregion
    }
}