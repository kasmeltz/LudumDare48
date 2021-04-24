namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class MusicalChord
    {
        #region Constructors

        public MusicalChord(MusicalNote[] notes)
        {
            Notes = notes;
        }

        #endregion

        #region Members

        public MusicalNote[] Notes { get; protected set; }

        #endregion
    }
}