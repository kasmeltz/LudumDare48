namespace KasJam.LD48.Unity.Behaviours
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    [AddComponentMenu("LD48/SunlightPanel")]
    public class SunlightPanelBehaviour : BehaviourBase
    {
        #region Members

        public Image Beach;
        public Image Singer;

        protected LevelManagerBehaviour LevelManager { get; set; }

        protected Color32[] SunlightColors { get; set; }

        #endregion
        
        #region Event Handlers

        private void LevelManager_LevelStarted(object sender, System.EventArgs e)
        {
            int level = LevelManager.LevelNumber;
            var sunlightColor = SunlightColors[level];

            //byte r = (byte)((sunlightColor.r + 128) / 2);
            //byte g = (byte)((sunlightColor.g + 128) / 2);
            //byte b = (byte)((sunlightColor.b + 128) / 2);

            byte r = (byte)Mathf.Min(255, sunlightColor.r + 16);
            byte g = (byte)Mathf.Min(255, sunlightColor.g + 16); 
            byte b = (byte)Mathf.Min(255, sunlightColor.b + 16);

            var finalColor = new Color32(r, g, b, 255);

            Beach.color = finalColor;
            Singer.color = finalColor;
        }

        #endregion

        #region Protected Methods

        protected void LoadSunlightColors()
        {
            var sunlightTexture = Resources
                .Load<Texture2D>("Images/sunlight_color");

            SunlightColors = new Color32[60];

            var colors = sunlightTexture
                .GetPixels32();

            int step = -sunlightTexture.width / 30;
            int px = sunlightTexture.width - 1;

            for(int i = 0;i < 60;i++)
            {
                SunlightColors[i] = colors[px];
                px += step;

                if (px <= 16)
                {
                    px = 16;
                    step = -step;
                }
            }
        }

        #endregion

        #region Unity

        protected override void Awake()
        {
            base
                .Awake();

            LoadSunlightColors();

            LevelManager = FindObjectOfType<LevelManagerBehaviour>();

            LevelManager.LevelStarted += LevelManager_LevelStarted;

        }

        #endregion
    }
}