namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class MusicalScale
    {
        #region Constructors

        public MusicalScale(string root, ScaleType mode)
        {
            Mode = mode;
            BuildScale(root);
        }

        #endregion

        #region Members

        public MusicalNote Root { get; protected set; }

        public ScaleType Mode { get; protected set; }

        public MusicalNote[] Notes { get; protected set; }

        #endregion

        #region Protected Methods

        protected void BuildScale(string root)
        {
            Root = new MusicalNote("C", 3, 1);

            Notes = new MusicalNote[8];

            Notes[0] = new MusicalNote("C", 3, 1);
            Notes[1] = new MusicalNote("D", 3, 1);
            Notes[2] = new MusicalNote("E", 3, 1);
            Notes[3] = new MusicalNote("F", 3, 1);
            Notes[4] = new MusicalNote("G", 3, 1);
            Notes[5] = new MusicalNote("A", 3, 1);
            Notes[6] = new MusicalNote("B", 3, 1);
        }

        #endregion
    }
}