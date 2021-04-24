namespace KasJam.LD48.Unity.Behaviours.Music
{
    using System;
    using System.Collections.Generic;

    public class MusicalScale
    {
        #region Constructors

        public MusicalScale(string root, ScaleType mode, int octave)
        {
            Mode = mode;
            BuildScale(root, octave);
        }

        #endregion

        #region Members

        public MusicalNote Root { get; protected set; }

        public ScaleType Mode { get; protected set; }

        public MusicalNote[] AscendingNotes { get; protected set; }
        public MusicalNote[] DescendingNotes { get; protected set; }

        public static List<string> NoteOrder = new List<string> { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        public static Dictionary<ScaleType, List<int>[]> NoteOffsets = new Dictionary<ScaleType, List<int>[]>
        {
            [ScaleType.Major] = new List<int>[]
            {
                new List<int> { 0, 2, 4, 5, 7, 9, 11 },
                new List<int> { 0, 2, 4, 5, 7, 9, 11 }
            },
            [ScaleType.NaturalMinor] = new List<int>[]
            {
                new List<int> { 0, 2, 3, 5, 7, 8, 10 },
                new List<int> { 0, 2, 3, 5, 7, 8, 10 }
            },
            [ScaleType.HarmonicMinor] = new List<int>[]
            {
                new List<int> { 0, 2, 3, 5, 7, 8, 11 },
                new List<int> { 0, 2, 3, 5, 7, 8, 11 }
            },
            [ScaleType.MelodicMinor] = new List<int>[]
            {
                new List<int> { 0, 2, 3, 5, 7, 9, 11 },
                new List<int> { 0, 2, 3, 5, 7, 8, 10 }
            },
            [ScaleType.Dorian] = new List<int>[]
            {
                new List<int> { 0, 2, 3, 5, 7, 9, 10 },
                new List<int> { 0, 2, 3, 5, 7, 9, 10 }
            },
            [ScaleType.Blues] = new List<int>[]
            {
                new List<int> { 0, 3, 5, 6, 7, 10 },
                new List<int> { 0, 3, 5, 6, 7, 10 }
            },
            [ScaleType.PentatonicMajor] = new List<int>[]
            {
                new List<int> { 0, 2, 4, 7, 9 },
                new List<int> { 0, 2, 4, 7, 9 }
            },
            [ScaleType.PentatonicMinor] = new List<int>[]
            {
                new List<int> { 0, 3, 5, 7, 10 },
                new List<int> { 0, 3, 5, 7, 10 }
            },
            [ScaleType.PentatonicJapanese] = new List<int>[]
            {
                new List<int> { 0, 1, 5, 7, 8 },
                new List<int> { 0, 1, 5, 7, 8 }
            }
        };


        #endregion

        #region Static Methods

        public static int GetNoteAbsoluteIndex(string name, int octaveMax, int octave)
        {
            int noteStep = 0;
            int octaveDifference = (octaveMax - octave);
            noteStep += octaveDifference * 12;
            noteStep -= NoteOrder
                .IndexOf(name);

            return noteStep;
        }

        #endregion

        #region Protected Methods

        protected void BuildScale(string root, int octave)
        {
            Root = new MusicalNote(root, octave, NoteTimbre.Ah);

            int rootOctave = octave;
            int rootNoteIndex = NoteOrder
                .IndexOf(root);

            if (rootNoteIndex < 0)
            {
                throw new ArgumentException($"{root} isn't a proper scale root you dumbass!");
            }

            switch (Mode)
            {
                case ScaleType.Major:
                case ScaleType.NaturalMinor:
                case ScaleType.HarmonicMinor:
                case ScaleType.MelodicMinor:
                case ScaleType.Dorian:
                    AscendingNotes = new MusicalNote[7];
                    DescendingNotes = new MusicalNote[7];
                    break;
                case ScaleType.Blues:
                    AscendingNotes = new MusicalNote[6];
                    DescendingNotes = new MusicalNote[6];
                    break;
                case ScaleType.PentatonicMajor:
                case ScaleType.PentatonicMinor:
                case ScaleType.PentatonicJapanese:
                    AscendingNotes = new MusicalNote[5];
                    DescendingNotes = new MusicalNote[5];
                break;

            }

            var noteOffsets = NoteOffsets[Mode];
            var ascending = noteOffsets[0];
            var descending = noteOffsets[1];

            for (int i = 0; i < ascending.Count; i++)
            {
                int actualOctave = rootOctave;
                int noteIndex = rootNoteIndex + ascending[i];

                if (noteIndex >= NoteOrder.Count)
                {
                    noteIndex -= NoteOrder.Count;
                    actualOctave++;
                }

                AscendingNotes[i] = new MusicalNote(NoteOrder[noteIndex], actualOctave, NoteTimbre.Ah);

                actualOctave = rootOctave;
                noteIndex = rootNoteIndex + descending[i];

                if (noteIndex >= NoteOrder.Count)
                {
                    noteIndex -= NoteOrder.Count;
                    actualOctave++;
                }

                DescendingNotes[i] = new MusicalNote(NoteOrder[noteIndex], actualOctave, NoteTimbre.Ah);
            }
        }
            
        #endregion
    }
}