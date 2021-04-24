namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class SongComposer
    {
        #region Protected Methods

        protected MusicalNote CloneNote(MusicalNote original, int octaveChange)
        {
            return new MusicalNote(original.Name, original.Octave + octaveChange, original.ScaleDegree);
        }

        protected void AddNoteToSong(Song song, float time, MusicalNote note, int octaveChange)
        {
            var songEvent = new SongEvent(time, CloneNote(note, octaveChange));

            song
                .Events
                .Add(songEvent);
        }

        #endregion

        #region Public Methods

        public Song ComposeSong(string root, int octave, ScaleType mode, float minNoteLength)
        {
            var song = new Song(100);

            MusicalScale scale = new MusicalScale(root, mode, octave);

            float runningTime = 0;
            var notes = scale.AscendingNotes;
            int octaveChange = 0;
            for (int i = 0; i < 8; i++)
            {
                if (i >= 7)
                {
                    octaveChange = 1;
                }

                AddNoteToSong(song, runningTime, notes[i % 7], octaveChange);
                runningTime += 0.5f;
            }

            notes = scale.DescendingNotes;
            for (int i = 6; i >= 0; i--)
            {
                AddNoteToSong(song, runningTime, notes[i % 7], 0);
                runningTime += 0.5f;
            }

            return song;
        }
    
        #endregion
    }
}