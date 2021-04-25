namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class SongComposer
    {
        #region Protected Methods           

        protected Song CreateSong(float minNoteLengt)
        {
            var song = new Song(minNoteLengt);

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

        public void AddNoteToSong(Song song, float time, int voiceNumber, float volume, MusicalNote note, NoteTimbre timbre, int octaveChange)
        {
            var songEvent = new SongEvent(time, voiceNumber, volume, note
                .Clone(timbre, octaveChange));

            var voice = song.Voices[voiceNumber];
            voice
                .Events
                .Add(songEvent);
        }

        public void StopNote(Song song, float time, int voiceNumber)
        {
            var songEvent = new SongEvent(time, voiceNumber);
            var voice = song.Voices[voiceNumber];
            voice
                .Events
                .Add(songEvent);
        }

        public Song ComposeFanfare(string root, int octave)
        {
            var song = CreateSong(0.25f);

            MusicalScale scale = new MusicalScale(root, ScaleType.Major, octave);

            var notes = scale.AscendingNotes;

            float runningTime = 0;

            AddNoteToSong(song, runningTime, 1, 0.5f, notes[5], NoteTimbre.Oh, -1);
            AddNoteToSong(song, runningTime, 2, 0.5f, notes[4], NoteTimbre.Oh, 0);
            AddNoteToSong(song, runningTime, 3, 0.5f, notes[0], NoteTimbre.Oh, 0);
            AddNoteToSong(song, runningTime, 4, 0.75f, notes[3], NoteTimbre.Ee, 0);

            runningTime += 0.75f;

            AddNoteToSong(song, runningTime, 1, 0.5f, notes[1], NoteTimbre.Ee, 0);
            AddNoteToSong(song, runningTime, 2, 0.5f, notes[0], NoteTimbre.Ee, 0);
            AddNoteToSong(song, runningTime, 3, 0.5f, notes[5], NoteTimbre.Ee, 0);
            AddNoteToSong(song, runningTime, 4, 0.75f, notes[4], NoteTimbre.Ee, 0);

            runningTime += 0.75f;

            AddNoteToSong(song, runningTime, 1, 0.5f, notes[4], NoteTimbre.Oh, -1);
            AddNoteToSong(song, runningTime, 2, 0.5f, notes[6], NoteTimbre.Oh, -1);
            AddNoteToSong(song, runningTime, 3, 0.5f, notes[3], NoteTimbre.Oh, 0);
            AddNoteToSong(song, runningTime, 4, 0.75f, notes[1], NoteTimbre.Ee, 0);

            runningTime += 0.75f;

            AddNoteToSong(song, runningTime, 1, 0.5f, notes[0], NoteTimbre.Ee, 0);
            AddNoteToSong(song, runningTime, 2, 0.5f, notes[2], NoteTimbre.Ee, 0);
            AddNoteToSong(song, runningTime, 3, 0.5f, notes[4], NoteTimbre.Ee, 0);
            AddNoteToSong(song, runningTime, 4, 0.75f, notes[2], NoteTimbre.Ee, 0);

            runningTime += 0.75f;

            song.TotalTime = runningTime + 2f;

            return song;
        }

        public Song ComposeSong(string root, int octave, ScaleType mode, float minNoteLength)
        {
            var song = CreateSong(minNoteLength);

            MusicalScale scale = new MusicalScale(root, mode, octave);
            
            SongStyle songStyle = new SongStyle(this, song, scale, minNoteLength);

            songStyle
                .Compose();

            return song;            
        }

        public Song ComposeSong(string root, int octave, float minNoteLength)
        {
            ProbabilityChooser<ScaleType> chooser = new ProbabilityChooser<ScaleType>();

            chooser
                .AddItem(ScaleType.Major, 1f);

            chooser
                .AddItem(ScaleType.NaturalMinor, 1f);

            chooser
                .AddItem(ScaleType.Dorian, 1f);

            var mode = chooser
                .ChooseItem();

            return ComposeSong(root, octave, mode, minNoteLength);
        }

        #endregion
    }
}