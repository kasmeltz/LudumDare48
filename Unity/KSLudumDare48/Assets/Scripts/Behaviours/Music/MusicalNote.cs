namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class MusicalNote
    {
        #region Constructors

        public MusicalNote(string name, int octave, int scaleDegree)
        {
            Name = name;
            Octave = octave;
            ScaleDegree = scaleDegree;
        }

        #endregion

        #region Members

        public string Name { get; protected set; }

        public int Octave { get; protected set; }

        public int ScaleDegree{ get; protected set; }

        #endregion
    }
}