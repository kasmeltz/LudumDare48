namespace KasJam.LD48.Unity.Behaviours.Music
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public class SongStyle
    {
        #region Constructors

        public SongStyle(SongComposer composer, Song song, MusicalScale musicalScale, float minimumNoteLength)
        {
            Composer = composer;
            Song = song;
            
            RunningTime = 0;

            MusicalScale = musicalScale;
            MinimumNoteLength = minimumNoteLength;

            ChordsTicksToHold = new int[4];
            BassMelodicStyles = new BassMelodicStyle[4];
            FirstDegrees = new int[4];
            BassTimbres = new NoteTimbre[4];
            ChordTimbres = new NoteTimbre[4];

            for (int i = 0; i < 4; i++)
            {
                ChordsTicksToHold[i] = -1;
                BassMelodicStyles[i] = BassMelodicStyle.None;
                FirstDegrees[i] = -1;
                BassTimbres[i] = NoteTimbre.None;
                ChordTimbres[i] = NoteTimbre.None;
            }

            LoadChords();
        }

        #endregion

        #region Members

        public SongComposer Composer { get; protected set; }

        public Song Song { get; protected set; }

        public MusicalScale MusicalScale { get; protected set; }

        public float MinimumNoteLength { get; protected set; }

        public int[] ChordsTicksToHold { get; protected set; }

        public NoteTimbre[] BassTimbres { get; protected set; }

        public NoteTimbre[] ChordTimbres { get; protected set; }

        public BassMelodicStyle[] BassMelodicStyles { get; protected set; }

        public int[] FirstDegrees { get; protected set; }

        public int CurrentChunkIndex { get; protected set; }

        public int TicksRemaining { get; protected set; }

        public float RunningTime { get; protected set; }

        protected Dictionary<ChordType, List<MusicalChord>> AllChords { get; set; }

        #endregion

        #region Public Methods

        public void Compose()
        {
            // MAKE A SONG 32 BARS LONG

            // MAKE FOUR EIGHT BAR CHUNKS
            for (int chunkIndex = 0; chunkIndex < 4; chunkIndex++)
            {
                DoChunk(chunkIndex);
            }

            Song
                .SortEvents();

            Song.TotalTime = RunningTime;


            /*
            float runningTime = 0;

            for (int j = 0; j < 2; j++)
            {
                var notes = scale.AscendingNotes;
                int octaveChange = 0;
                var notesInScaleCount = notes.Length;

                for (int i = 0; i <= notesInScaleCount; i++)
                {
                    if (i >= notesInScaleCount)
                    {
                        octaveChange = 1;
                    }

                    AddNoteToSong(song, runningTime, 0, 0.75f, notes[i % notesInScaleCount], NoteTimbre.Ee, octaveChange);
                    //StopNote(song, runningTime + minNoteLength / 2, 0);
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

            // TODO - CHANGE FOR TESTING
            //song.TotalTime = runningTime / 2;
            song.TotalTime = runningTime;

            return song;
            */
        }

        public void DoChunk(int index)
        {
            var sb = new StringBuilder();

            CurrentChunkIndex = index;

            sb
                .AppendLine("==========================================================================");

            sb
                .AppendLine($"CHUNK => {CurrentChunkIndex}");


            // DECIDE WHAT STYLE(S) TO USE FOR THIS CHUNK
            ChooseChunkStyle();

            sb
                .AppendLine($"ChordsTicksToHold => {ChordsTicksToHold[CurrentChunkIndex]}");

            sb
                .AppendLine($"ChordTimbre => {ChordTimbres[CurrentChunkIndex]}");

            sb
                .AppendLine($"BassMelodicStyle => {BassMelodicStyles[CurrentChunkIndex]}");

            sb
                .AppendLine($"BassTimbres => {BassTimbres[CurrentChunkIndex]}");

            ChooseFirstDegree();
                
            var currentDegree = FirstDegrees[CurrentChunkIndex];
            var chordTimbre = ChordTimbres[CurrentChunkIndex];
            var bassTimbre = BassTimbres[CurrentChunkIndex];

            sb
                .AppendLine($"FirstDegree => {currentDegree}");

            // WRITE THE CHORD PROGRESSION
            ResetChunkTimers();

            do
            {
                AddChordToSong(RunningTime, 0.5f, currentDegree, chordTimbre, ChordType.TriadForDegree);
                
                RunningTime += Song.NoteLengthMin * ChordsTicksToHold[CurrentChunkIndex];
                TicksRemaining -= ChordsTicksToHold[CurrentChunkIndex];

            } while (TicksRemaining > 0);

            Song
                .SortEvents();

            /*
            // WRITE THE BASS LINE
            ResetChunkTimers();

            do
            {
                Composer
                    .AddNoteToSong(Song, RunningTime, 0, 0.5f, MusicalScale.AscendingNotes[currentDegree], bassTimbre, 0);

                RunningTime += Song.NoteLengthMin * ChordsTicksToHold[CurrentChunkIndex];

                TicksRemaining -= ChordsTicksToHold[CurrentChunkIndex];

            } while (TicksRemaining > 0);

            Song
                .SortEvents();            
            */

            Debug
                .Log(sb
                    .ToString());
        }

        #endregion

        #region Protected Methods

        protected void AddChordToSong(float time, float volume, int degree, NoteTimbre timbre, ChordType chordType)
        {
            var chord = AllChords[chordType][degree];

            // CHORDS START WITH VOICE 1
            int voice = 1;
            for (int i = 0; i > chord.Notes.Length; i++)
            {
                Composer
                    .AddNoteToSong(Song, time, voice, volume, chord.Notes[i], timbre, 0);

                voice++;
            }
        }

        protected void ResetChunkTimers()
        {
            // 8 BARS WITH 4 BEATS AND EACH BEAT HAS 4 SUBDIVISIONS
            TicksRemaining = 8 * 4 * 4;

            // SET CURRENT TIME
            RunningTime = TicksRemaining * CurrentChunkIndex * Song.NoteLengthMin;
        }

        protected void ChooseFirstDegree()
        {
            ProbabilityChooser<int> firstDegreeChooser = new ProbabilityChooser<int>();
            for (int i = 0; i < MusicalScale.AscendingNotes.Length; i++)
            {
                firstDegreeChooser
                    .AddItem(i, 1);
            }

            AddPreviousChoice(firstDegreeChooser, FirstDegrees, CurrentChunkIndex, 5f);

            FirstDegrees[CurrentChunkIndex] = firstDegreeChooser
                .ChooseItem();
        }

        protected void ChooseChunkStyle()
        {
            // SELECT HOW MANY MINIMUM NOTE LENGTHS
            // TO PLAY EACH NOTE FOR
            ProbabilityChooser<int> chordTicksChooser = new ProbabilityChooser<int>();

            chordTicksChooser
                .AddItem(16, 0.05f);
            chordTicksChooser
                .AddItem(8, 0.4f);
            chordTicksChooser
                .AddItem(4, 0.1f);
            chordTicksChooser
                .AddItem(2, 0.05f);
            chordTicksChooser
                .AddItem(1, 0.01f);

            AddPreviousChoice(chordTicksChooser, ChordsTicksToHold, CurrentChunkIndex, 0.5f);

            ChordsTicksToHold[CurrentChunkIndex] = chordTicksChooser
                .ChooseItem();

            // SELECT THE TIMBRE FOR THE CHORD VOICES
            ProbabilityChooser<NoteTimbre> chordTimbreChooser = new ProbabilityChooser<NoteTimbre>();

            chordTimbreChooser
               .AddItem(NoteTimbre.Eh, 0.1f);

            chordTimbreChooser
                .AddItem(NoteTimbre.Ee, 0.1f);

            chordTimbreChooser
                .AddItem(NoteTimbre.Ah, 0.1f);

            chordTimbreChooser
                .AddItem(NoteTimbre.Oh, 0.3f);

            chordTimbreChooser
                .AddItem(NoteTimbre.Oo, 0.05f);

            AddPreviousChoice(chordTimbreChooser, BassTimbres, CurrentChunkIndex, 0.6f);

            ChordTimbres[CurrentChunkIndex] = chordTimbreChooser
                .ChooseItem();

            // SELECT THE TIMBRE FOR THE BASS VOICE
            ProbabilityChooser<NoteTimbre> bassTimbreChooser = new ProbabilityChooser<NoteTimbre>();

            bassTimbreChooser
               .AddItem(NoteTimbre.Eh, 0.1f);

            bassTimbreChooser
                .AddItem(NoteTimbre.Ee, 0.1f);

            bassTimbreChooser
                .AddItem(NoteTimbre.Ah, 0.4f);

            bassTimbreChooser
                .AddItem(NoteTimbre.Oh, 0.05f);

            bassTimbreChooser
                .AddItem(NoteTimbre.Oo, 0.05f);

            AddPreviousChoice(bassTimbreChooser, BassTimbres, CurrentChunkIndex, 0.5f);

            BassTimbres[CurrentChunkIndex] = bassTimbreChooser
                .ChooseItem();

            // SELECT THE BASS MELODIC STYLE
            ProbabilityChooser<BassMelodicStyle> bassMelodicStyleChooser = new ProbabilityChooser<BassMelodicStyle>();

            bassMelodicStyleChooser
               .AddItem(BassMelodicStyle.SingleNote, 1f);

            bassMelodicStyleChooser
               .AddItem(BassMelodicStyle.AlternatingNotes, 2f);

            bassMelodicStyleChooser
               .AddItem(BassMelodicStyle.WalkingNotes, 1f);

            AddPreviousChoice(bassMelodicStyleChooser, BassMelodicStyles, CurrentChunkIndex, 4f);

            BassMelodicStyles[CurrentChunkIndex] = bassMelodicStyleChooser
                .ChooseItem();
        }

        protected void AddPreviousChoice<T>(ProbabilityChooser<T> chooser, T[] chunkArray, int chunkIndex, float probability)
        {
            if (chunkIndex == 0)
            {
                return;
            }

            chooser
                .AddItem(chunkArray[chunkIndex - 1], probability);
        }

        protected void LoadChords()
        {
            AllChords = new Dictionary<ChordType, List<MusicalChord>>();

            switch (MusicalScale.Mode)
            {
                case ScaleType.Major:
                case ScaleType.NaturalMinor:
                case ScaleType.MelodicMinor:
                case ScaleType.HarmonicMinor:
                case ScaleType.Dorian:                
                    LoadRegularChords();
                    break;
            }
        }

        protected void LoadRegularChords()
        {
            var triads = new List<MusicalChord>();
            for (int i = 0; i < 6; i++)
            {
                int octave2 = 0;
                int octave3 = 0;

                int scaleDegree1 = i;
                int scaleDegree2 = i + 2;
                if (scaleDegree2 >= 7)
                {
                    octave2 = 1;
                    scaleDegree2 -= 7;

                }
                int scaleDegree3 = i + 4;
                if (scaleDegree3 >= 7)
                {
                    octave3 = 1;
                    scaleDegree3 -= 7;
                }

                var notes = new MusicalNote[] 
                {
                    MusicalScale
                        .AscendingNotes[scaleDegree1],
                    MusicalScale
                        .AscendingNotes[scaleDegree2]
                        .Clone(octave2),
                    MusicalScale
                    .AscendingNotes[scaleDegree3]
                        .Clone(octave3)
                };

                triads
                    .Add(new MusicalChord(notes));
            }

            AllChords[ChordType.TriadForDegree] = triads;

            var tetrads = new List<MusicalChord>();
            for (int i = 0; i < 6; i++)
            {
                int octave2 = 0;
                int octave3 = 0;
                int octave4 = 0;

                int scaleDegree1 = i;
                int scaleDegree2 = i + 2;
                if (scaleDegree2 >= 7)
                {
                    octave2 = 1;
                    scaleDegree2 -= 7;

                }
                int scaleDegree3 = i + 4;
                if (scaleDegree3 >= 7)
                {
                    octave3 = 1;
                    scaleDegree3 -= 7;
                }
                int scaleDegree4 = i + 6;
                if (scaleDegree4 >= 7)
                {
                    octave4 = 1;
                    scaleDegree4 -= 7;
                }

                var notes = new MusicalNote[]
                {
                    MusicalScale
                        .AscendingNotes[scaleDegree1],
                    MusicalScale
                        .AscendingNotes[scaleDegree2]
                        .Clone(octave2),
                    MusicalScale
                        .AscendingNotes[scaleDegree3]
                        .Clone(octave3),
                    MusicalScale
                        .AscendingNotes[scaleDegree4]
                        .Clone(octave4)
                };

                tetrads
                    .Add(new MusicalChord(notes));
            }

            AllChords[ChordType.TetradForDegree] = tetrads;
        }

        #endregion
    }
}