namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class SongComposer
    {
        #region Protected Methods

        protected MusicalNote CloneNote(MusicalNote original, NoteTimbre timbre, int octaveChange)
        {
            return new MusicalNote(original.Name, original.Octave + octaveChange, timbre);
        }

        protected void AddNoteToSong(Song song, float time, int voiceNumber, float volume, MusicalNote note, NoteTimbre timbre, int octaveChange)
        {
            var songEvent = new SongEvent(time, voiceNumber, volume, CloneNote(note, timbre, octaveChange));
            var voice = song.Voices[voiceNumber];
            voice
                .Events
                .Add(songEvent);
        }

        protected void StopNote(Song song, float time, int voiceNumber)
        {
            var songEvent = new SongEvent(time, voiceNumber);
            var voice = song.Voices[voiceNumber];
            voice
                .Events
                .Add(songEvent);
        }

        protected Song CreateSong()
        {
            var song = new Song(100);

            for (int i = 0; i < 10; i++)
            {
                song
                    .Voices
                    .Add(new SongVoice());
            }

            return song;
        }

        #endregion

        #region Public Methods

        public Song ComposeFanfare(string root, int octave)
        {
            var song = CreateSong();

            MusicalScale scale = new MusicalScale(root, ScaleType.Major, octave);

            var notes = scale.AscendingNotes;

            AddNoteToSong(song, 0, 1, 0.75f, notes[0], NoteTimbre.Oo, 0);
            AddNoteToSong(song, 0, 2, 0.75f, notes[2], NoteTimbre.Oo, 0);
            AddNoteToSong(song, 0, 3, 0.75f, notes[4], NoteTimbre.Oo, 0);
            AddNoteToSong(song, 0, 4, 0.75f, notes[0], NoteTimbre.Oo, 1);

            song.TotalTime = 3;

            return song;
        }

        public Song ComposeSong(string root, int octave, ScaleType mode, float minNoteLength)
        {
            var song = CreateSong();

            MusicalScale scale = new MusicalScale(root, mode, octave);

            float runningTime = 0;
            var notes = scale.AscendingNotes;
            int octaveChange = 0;
            var notesInScaleCount = notes.Length;

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i <= notesInScaleCount; i++)
                {
                    if (i >= notesInScaleCount)
                    {
                        octaveChange = 1;
                    }

                    AddNoteToSong(song, runningTime, 0, 0.75f, notes[i % notesInScaleCount], NoteTimbre.Ee, octaveChange);
                    StopNote(song, runningTime + minNoteLength / 2, 0);
                    runningTime += minNoteLength;
                }

                notes = scale.DescendingNotes;
                for (int i = notesInScaleCount - 1; i >= 0; i--)
                {
                    AddNoteToSong(song, runningTime, 0, 0.75f, notes[i % notesInScaleCount], NoteTimbre.Ah, 0);
                    runningTime += minNoteLength;
                }

                runningTime += minNoteLength * 4;
            }

            song.TotalTime = runningTime;

            return song;
        }
    
        #endregion
    }
}