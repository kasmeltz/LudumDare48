namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class MusicalScale
    {
        #region Constructors

        public MusicalScale(MusicalNote root, ScaleType mode)
        {
            Root = root;
            Mode = mode;
            BuildScale();
        }

        #endregion

        #region Members

        public MusicalNote Root { get; protected set; }

        public ScaleType Mode { get; protected set; }

        public MusicalNote[] Notes { get; protected set; }

        #endregion

        #region Protected Methods

        protected void BuildScale()
        {
            Notes = new MusicalNote[8];


        }

        #endregion
    }
}