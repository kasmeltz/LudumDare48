namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class SongComposer
    {
        #region Protected Methods

        protected MusicalNote CloneNote(MusicalNote original, int octaveChange)
        {
            return new MusicalNote(original.Name, original.Octave + octaveChange, original.ScaleDegree);
        }

        #endregion

        #region Public Methods

        public Song ComposeSong(string root, int octave, ScaleType mode, float minNoteLength)
        {
            var song = new Song(100);

            MusicalScale scale = new MusicalScale(root, mode, octave);

            var notes = scale.AscendingNotes;

            int octaveChange = 0;
            for (int i = 0; i < 8; i++)
            {
                if (i >= 7)
                {
                    octaveChange = 1;
                }

                float time = i / 2f;
                var songEvent = new SongEvent(time, CloneNote(notes[i % 7], octaveChange));

                song
                    .Events
                    .Add(songEvent);
            }

            return song;
        }
    
        #endregion
    }
}