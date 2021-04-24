namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class SongComposer
    {
        #region Protected Methods

        protected MusicalNote CloneNote(MusicalNote original, int octave)
        {
            return new MusicalNote(original.Name, octave, original.ScaleDegree);
        }

        #endregion

        #region Public Methods

        public Song ComposeSong(string root, ScaleType mode, float minNoteLength)
        {
            var song = new Song(100);

            MusicalScale scale = new MusicalScale(root, mode);

            var notes = scale.Notes;

            var songEvent = new SongEvent(0, CloneNote(notes[0], 3));
           
            song
                .Events
                .Add(songEvent);

            songEvent = new SongEvent(1, CloneNote(notes[1], 3));

            song
                .Events
                .Add(songEvent);

            songEvent = new SongEvent(2, CloneNote(notes[2], 3));

            song
                .Events
                .Add(songEvent);

            songEvent = new SongEvent(3, CloneNote(notes[3], 3));

            song
                .Events
                .Add(songEvent);

            songEvent = new SongEvent(4, CloneNote(notes[4], 3));

            song
                .Events
                .Add(songEvent);

            songEvent = new SongEvent(5, CloneNote(notes[5], 3));

            song
                .Events
                .Add(songEvent);

            songEvent = new SongEvent(6, CloneNote(notes[6], 3));

            song
                .Events
                .Add(songEvent);

            songEvent = new SongEvent(7, CloneNote(notes[0], 4));

            song
                .Events
                .Add(songEvent);

            return song;
        }

        #endregion
    }
}