namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class MusicalNote
    {
        #region Constructors

        public MusicalNote(string name, int octave, NoteTimbre timbre)
        {
            Name = name;
            Octave = octave;
            Timbre = timbre;
        }

        #endregion

        #region Members

        public string Name { get; protected set; }

        public int Octave { get; protected set; }

        public NoteTimbre Timbre { get; protected set; }

        #endregion

        #region Public Methods

        public MusicalNote Clone(NoteTimbre timbre, int octaveChange)
        {
            return new MusicalNote(Name, Octave + octaveChange, timbre);
        }

        public MusicalNote Clone(NoteTimbre timbre)
        {
            return new MusicalNote(Name, Octave, timbre);
        }

        public MusicalNote Clone(int octaveChange)
        {
            return new MusicalNote(Name, Octave + octaveChange, Timbre);
        }

        #endregion
    }
}