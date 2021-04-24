namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class SongComposer
    {
        #region Protected Methods

        protected MusicalNote CloneNote(MusicalNote original, NoteTimbre timbre, int octaveChange)
        {
            return new MusicalNote(original.Name, original.Octave + octaveChange, timbre);
        }

        protected void AddNoteToSong(Song song, float time, MusicalNote note, NoteTimbre timbre, int octaveChange)
        {
            var songEvent = new SongEvent(time, CloneNote(note, timbre, octaveChange));

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
            var notesInScaleCount = notes.Length;

            for (int i = 0; i <= notesInScaleCount; i++)
            {
                if (i >= notesInScaleCount)
                {
                    octaveChange = 1;
                }

                AddNoteToSong(song, runningTime, notes[i % notesInScaleCount], NoteTimbre.Ah, octaveChange);
                runningTime += 0.5f;
            }

            notes = scale.DescendingNotes;
            for (int i = notesInScaleCount - 1; i >= 0; i--)
            {
                AddNoteToSong(song, runningTime, notes[i % notesInScaleCount], NoteTimbre.Ah, 0);
                runningTime += 0.5f;
            }

            return song;
        }
    
        #endregion
    }
}