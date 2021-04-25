namespace KasJam.LD48.Unity.Behaviours.Music
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            MelodicStyles = new MelodicStyle[4];
            FirstDegrees = new int[4];
            BassTimbres = new NoteTimbre[4];
            ChordTimbres = new NoteTimbre[4];
            MelodyTimbres = new NoteTimbre[4];
            ChordTypes = new ChordType[4];

            for (int i = 0; i < 4; i++)
            {
                ChordsTicksToHold[i] = -1;
                BassMelodicStyles[i] = BassMelodicStyle.None;
                MelodicStyles[i] = MelodicStyle.None;
                FirstDegrees[i] = -1;
                BassTimbres[i] = NoteTimbre.None;
                ChordTimbres[i] = NoteTimbre.None;
                MelodyTimbres[i] = NoteTimbre.None;
                ChordTypes[i] = ChordType.None;
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

        public NoteTimbre[] MelodyTimbres { get; protected set; }

        public ChordType[] ChordTypes { get; protected set; }

        public BassMelodicStyle[] BassMelodicStyles { get; protected set; }

        public MelodicStyle[] MelodicStyles { get; protected set; }

        public int[] FirstDegrees { get; protected set; }

        public int CurrentChunkIndex { get; protected set; }

        public int TicksRemaining { get; protected set; }

        public float RunningTime { get; protected set; }

        protected Dictionary<ChordType, List<MusicalChord>> AllChords { get; set; }

        protected Dictionary<MelodicStyle, ProbabilityChooser<int>> RestLengthChooserPerStyle { get; set; }

        protected Dictionary<MelodicStyle, ProbabilityChooser<int>> NoteLengthChooserPerStyle { get; set; }

        protected Dictionary<MelodicStyle, ProbabilityChooser<bool>> StayOnOffBeatChooser { get; set; }

        protected Dictionary<BeatStrength, ProbabilityChooser<bool>> MelodyNoteIsInChordChoosers { get; set; }

        protected Dictionary<BeatStrength, ProbabilityChooser<bool>> MelodyNoteIsInScaleChoosers { get; set; }
        
        protected ProbabilityChooser<int> MelodicIntervalChooser { get; set; }

        public const int BASS_VOICE = 0;
        public const int CHORD_VOICE = 1;
        public const int MELODY_VOICE = 8;
        public const float TICKS_PER_CHUNK = 8 * 4 * 4;

        #endregion

        #region Public Methods

        public void Compose()
        {
            // MAKE A SONG 32 BARS LONG

            // Create choosers that will be used throughout process
            CreateChoosers();

            // MAKE FOUR EIGHT BAR CHUNKS
            for (int chunkIndex = 0; chunkIndex < 4; chunkIndex++)
            {
                DoChunk(chunkIndex);
            }

            Song
                .SortEvents();

            Song.TotalTime = RunningTime;
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
                .AppendLine($"ChordType => {ChordTypes[CurrentChunkIndex]}");

            sb
                .AppendLine($"BassMelodicStyle => {BassMelodicStyles[CurrentChunkIndex]}");

            sb
                .AppendLine($"BassTimbres => {BassTimbres[CurrentChunkIndex]}");

            sb
               .AppendLine($"MelodicStyle => {MelodicStyles[CurrentChunkIndex]}");

            sb
               .AppendLine($"MelodyTimbres => {MelodyTimbres[CurrentChunkIndex]}");


            ChooseFirstDegree();
                
            sb
                .AppendLine($"FirstDegree => {FirstDegrees[CurrentChunkIndex]}");

            ResetChunkTimers();

            WriteChordProgression();

            WriteBaseLine();

            WriteMelody();

            Song
                .SortEvents();

            Debug
                .Log(sb
                    .ToString());
        }

        #endregion

        #region Protected Methods

        protected void WriteChordProgression()
        {
            var currentDegree = FirstDegrees[CurrentChunkIndex];
            var chordTimbre = ChordTimbres[CurrentChunkIndex];
            var chordType = ChordTypes[CurrentChunkIndex];
            var chordTicksToHold = ChordsTicksToHold[CurrentChunkIndex];

            do
            {
                AddChordToSong(RunningTime, 0.5f, currentDegree, chordTimbre, chordType);

                RunningTime += Song.NoteLengthMin * chordTicksToHold;
                TicksRemaining -= chordTicksToHold;

                currentDegree = ChooseNextDegree(currentDegree);

            } while (TicksRemaining > 0);

            Song
                .SortEvents();
        }

        protected void WriteBaseLine()
        {
            var chordTicksToHold = ChordsTicksToHold[CurrentChunkIndex];
            var bassTimbre = BassTimbres[CurrentChunkIndex];
            var bassmelodicStyle = BassMelodicStyles[CurrentChunkIndex];

            var walkingBassLineApproachChooser = new ProbabilityChooser<int>();

            walkingBassLineApproachChooser.AddItem(-2, 1);
            walkingBassLineApproachChooser.AddItem(-1, 1);

            var walkingBassLineExitChooser = new ProbabilityChooser<int>();

            walkingBassLineExitChooser.AddItem(2, 1);
            walkingBassLineExitChooser.AddItem(1, 1);

            // GET THE CHORD EVENTS
            var chordEvents = Song.Voices[1].Events;

            // WRITE THE BASS LINE
            int octaveChange;
            int baseNoteTicksToHold = chordTicksToHold;

            switch (bassmelodicStyle)
            {
                case BassMelodicStyle.SingleNote:
                    foreach (var chordEvent in chordEvents)
                    {
                        octaveChange = -1;
                        if (chordEvent.Note.Octave <= 1)
                        {
                            octaveChange = 0;
                        }

                        // BASS IS VOICE 0
                        Composer
                            .AddNoteToSong(Song, chordEvent.OccursAt, BASS_VOICE, 0.75f, chordEvent.Note, bassTimbre, octaveChange);
                    }

                    break;

                case BassMelodicStyle.AlternatingNotes:
                    baseNoteTicksToHold = chordTicksToHold / 2;

                    foreach (var chordEvent in chordEvents)
                    {
                        octaveChange = -1;
                        if (chordEvent.Note.Octave <= 1)
                        {
                            octaveChange = 0;
                        }

                        Composer
                            .AddNoteToSong(Song, chordEvent.OccursAt, BASS_VOICE, 0.75f, chordEvent.Note, bassTimbre, octaveChange);

                        var otherEvent = Song.Voices[3].Events.FirstOrDefault(o => o.OccursAt == chordEvent.OccursAt);

                        float secondNoteOccurs = chordEvent.OccursAt + baseNoteTicksToHold * MinimumNoteLength;

                        Composer
                            .AddNoteToSong(Song, secondNoteOccurs, BASS_VOICE, 0.75f, otherEvent.Note, bassTimbre, octaveChange);
                    }
                    break;

                case BassMelodicStyle.WalkingNotes:
                    baseNoteTicksToHold = chordTicksToHold / 4;

                    foreach (var chordEvent in chordEvents)
                    {
                        var otherEvent = Song.Voices[3].Events.FirstOrDefault(o => o.OccursAt == chordEvent.OccursAt);

                        octaveChange = -1;
                        if (chordEvent.Note.Octave <= 1)
                        {
                            octaveChange = 0;
                        }

                        float time = chordEvent.OccursAt;

                        Composer
                            .AddNoteToSong(Song, time, BASS_VOICE, 0.75f, chordEvent.Note, bassTimbre, octaveChange);

                        time += baseNoteTicksToHold * MinimumNoteLength;

                        int secondNoteDegree = MusicalScale
                            .GetDegree(otherEvent.Note.Name);

                        int nextDegree = GetScaleDegree(secondNoteDegree, walkingBassLineApproachChooser
                            .ChooseItem());

                        if (nextDegree > secondNoteDegree)
                        {
                            octaveChange = -1;
                        }

                        var nextNote = MusicalScale.AscendingNotes[nextDegree];

                        Composer
                            .AddNoteToSong(Song, time, BASS_VOICE, 0.75f, nextNote, bassTimbre, octaveChange);

                        time += baseNoteTicksToHold * MinimumNoteLength;

                        octaveChange = -1;
                        if (otherEvent.Note.Octave <= 1)
                        {
                            octaveChange = 0;
                        }

                        Composer
                            .AddNoteToSong(Song, time, BASS_VOICE, 0.75f, otherEvent.Note, bassTimbre, octaveChange);

                        nextDegree = GetScaleDegree(secondNoteDegree, walkingBassLineExitChooser
                            .ChooseItem());

                        if (nextDegree < secondNoteDegree)
                        {
                            octaveChange = -1;
                        }

                        nextNote = MusicalScale.AscendingNotes[nextDegree];

                        time += baseNoteTicksToHold * MinimumNoteLength;

                        Composer
                            .AddNoteToSong(Song, time, BASS_VOICE, 0.75f, nextNote, bassTimbre, octaveChange);
                    }
                    break;
            }

            Song
                .SortEvents();
        }

        protected MusicalChord GetChordForTime(float time)
        {
            var chordEvent = Song
                  .Voices[CHORD_VOICE]
                  .Events
                  .Where(o => o.OccursAt == time)
                  .FirstOrDefault();

            if (chordEvent == null)
            {
                return null;
            }

            var notes = new List<MusicalNote>();

            notes
                .Add(chordEvent.Note);

            for (int i = CHORD_VOICE + 1; i < MELODY_VOICE; i++)
            {
                chordEvent = Song
                  .Voices[i]
                  .Events
                  .Where(o => o.OccursAt == time)
                  .FirstOrDefault();

                if (chordEvent != null)
                {
                    notes
                        .Add(chordEvent.Note);
                }
            }

            return new MusicalChord(notes
                .ToArray());
        }

        protected BeatStrength GetBeatStrength(float time)
        {
            int ticks = Mathf.RoundToInt(time / MinimumNoteLength);

            if (ticks % 16 == 0)
            {
                return BeatStrength.Strongest;
            }
            else if (ticks % 8 == 0)
            {
                return BeatStrength.Strong;
            }
            else if (ticks % 4 == 0)
            {
                return BeatStrength.Weak;
            }
            else if (ticks % 2 == 0)
            {
                return BeatStrength.Off8th;
            }

            return BeatStrength.Off16th;
        }

        protected void WriteMelody()
        {
            var melodicStyle = MelodicStyles[CurrentChunkIndex];
            var melodicTimbre = MelodyTimbres[CurrentChunkIndex];

            int consecutiveTicksWithoutRest = 0;
            float melodyTime = CurrentChunkIndex * TICKS_PER_CHUNK * MinimumNoteLength;
            MusicalNote currentNote = null;
            float currentNoteEndTime = 0;
            MusicalChord currentChord = null;

            for (int tick = 0; tick < 8 * 4 * 4; tick++)
            {
                if (melodyTime < currentNoteEndTime)
                {
                    melodyTime += MinimumNoteLength;
                    continue;
                }

                // SHOULD WE REST?
                var restChooser = new ProbabilityChooser<bool>();

                restChooser
                    .AddItem(true, consecutiveTicksWithoutRest);

                restChooser
                    .AddItem(false, 32);

                var shouldRest = restChooser
                    .ChooseItem();

                if (shouldRest)
                {
                    consecutiveTicksWithoutRest = 0;

                    var restLengthChooser = RestLengthChooserPerStyle[melodicStyle];

                    int ticksToRest = restLengthChooser
                        .ChooseItem();

                    currentNoteEndTime = melodyTime + ticksToRest * MinimumNoteLength;

                    currentNote = null;

                    melodyTime += MinimumNoteLength;

                    continue;
                }

                var chord = GetChordForTime(melodyTime);

                if (chord != null)
                {
                    currentChord = chord;
                }

                var beatStrength = GetBeatStrength(melodyTime);

                int noteTicks = 0;
                bool stayOnOffBeat = StayOnOffBeatChooser[melodicStyle]
                    .ChooseItem();

                if (beatStrength == BeatStrength.Off16th && !stayOnOffBeat)
                {
                    noteTicks = 1;
                }
                else if (beatStrength == BeatStrength.Off8th && !stayOnOffBeat)
                {
                    noteTicks = 2;
                }
                else
                {
                    var noteLengthChooser = NoteLengthChooserPerStyle[melodicStyle];

                    noteTicks = noteLengthChooser
                        .ChooseItem();
                }

                var melodyNoteInChordChooser = MelodyNoteIsInChordChoosers[beatStrength];
                bool noteShouldBeInChord = melodyNoteInChordChooser
                    .ChooseItem();

                var melodyNoteInScaleChooser = MelodyNoteIsInScaleChoosers[beatStrength];
                bool noteShouldBeInScale = melodyNoteInScaleChooser
                    .ChooseItem();

                int melodicInterval = MelodicIntervalChooser
                    .ChooseItem();

                int currentDegree = -1;

                if (currentNote != null)
                {
                    currentDegree = MusicalScale
                        .GetDegree(currentNote.Name);
                }

                int nextDegree = 0;

                if (noteShouldBeInChord)
                {
                    if (currentNote != null) {
                        int closestDegree = 0;
                        int minDistance = int.MaxValue;

                        foreach (var note in currentChord.Notes)
                        {
                            int ndg = MusicalScale
                                .GetDegree(note.Name);

                            int d = Mathf
                                .Abs(currentDegree - ndg);

                            if (d < minDistance)
                            {
                                minDistance = d;
                                closestDegree = ndg;
                            }
                        }

                        nextDegree = closestDegree;
                    }
                    else
                    {
                        var noteIndex = UnityEngine
                            .Random
                            .Range(0, currentChord.Notes.Length);

                        var note = currentChord.Notes[noteIndex];

                        nextDegree = MusicalScale
                            .GetDegree(note.Name);
                    }
                }
                else
                {
                    if (currentNote != null)
                    {
                        nextDegree = GetScaleDegree(currentDegree, melodicInterval);
                    }
                    else
                    {
                        nextDegree = GetScaleDegree(0, melodicInterval);
                    }
                }

                int octaveChange = 0;

                if (melodicInterval > 7)
                {
                    octaveChange++;
                }

                if (melodicInterval < -7)
                {
                    octaveChange--;
                }

                if (currentNote != null)
                { 
                    if (melodicInterval > 0 && nextDegree < currentDegree)
                    {
                        octaveChange++;
                    }

                    if (melodicInterval < 0 && nextDegree > currentDegree)
                    {
                        octaveChange--;
                    }
                }

                currentNote = MusicalScale
                    .AscendingNotes[nextDegree];

                if (currentNote.Octave - octaveChange <= 0)
                {
                    octaveChange = 0;
                }

                if (currentNote.Octave + octaveChange >= 7)
                {
                    octaveChange = 0;
                }

                Composer
                    .AddNoteToSong(Song, melodyTime, MELODY_VOICE, 1f, currentNote, melodicTimbre, octaveChange);

                consecutiveTicksWithoutRest++;

                currentNoteEndTime = melodyTime + noteTicks * MinimumNoteLength;

                melodyTime += MinimumNoteLength;
            }
        }

        protected int ChooseNextDegree(int degree)
        {
            ProbabilityChooser<int> nextDegreeChooser = new ProbabilityChooser<int>();

            int d = GetScaleDegree(degree, 3);

            nextDegreeChooser
                .AddItem(d, 3);

            d = GetScaleDegree(degree, 1);

            nextDegreeChooser
                .AddItem(d, 1);

            d = GetScaleDegree(degree, -1);

            nextDegreeChooser
                .AddItem(d, 1);

            nextDegreeChooser
                .AddItem(99, 0.5f);

            int nextDegree = nextDegreeChooser
                .ChooseItem();

            if (nextDegree == 99)
            {
                return UnityEngine
                    .Random
                    .Range(0, 7);
            }
            else
            {
                return nextDegree;
            }
        }

        protected void AddChordToSong(float time, float volume, int degree, NoteTimbre timbre, ChordType chordType)
        {
            var chord = AllChords[chordType][degree];

            // CHORDS START WITH VOICE 1
            int voice = CHORD_VOICE;
            for (int i = 0; i < chord.Notes.Length; i++)
            {
                float v = volume;
                if (i == 0)
                {
                    v = 0;
                }

                Composer
                    .AddNoteToSong(Song, time, voice, v, chord.Notes[i], timbre, 0);

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

            AddPreviousChoice(chordTicksChooser, ChordsTicksToHold, CurrentChunkIndex, 0.5f);

            ChordsTicksToHold[CurrentChunkIndex] = chordTicksChooser
                .ChooseItem();

            // SELECT THE TIMBRE FOR THE CHORD VOICES
            ProbabilityChooser<NoteTimbre> chordTimbreChooser = new ProbabilityChooser<NoteTimbre>();

            //chordTimbreChooser
               //.AddItem(NoteTimbre.Eh, 0.1f);

            chordTimbreChooser
                .AddItem(NoteTimbre.Ee, 0.1f);

            chordTimbreChooser
                .AddItem(NoteTimbre.Ah, 0.1f);

            chordTimbreChooser
                .AddItem(NoteTimbre.Oh, 0.3f);

            //chordTimbreChooser
                //.AddItem(NoteTimbre.Oo, 0.05f);

            AddPreviousChoice(chordTimbreChooser, ChordTimbres, CurrentChunkIndex, 0.6f);

            ChordTimbres[CurrentChunkIndex] = chordTimbreChooser
                .ChooseItem();

            // SELECT THE CHORD STYLE
            ProbabilityChooser<ChordType> chordTypeChooser = new ProbabilityChooser<ChordType>();

            chordTypeChooser
                .AddItem(ChordType.TriadForDegree, 1);

            chordTypeChooser
                .AddItem(ChordType.TetradForDegree, 1);

            AddPreviousChoice(chordTypeChooser, ChordTypes, CurrentChunkIndex, 0.6f);

            ChordTypes[CurrentChunkIndex] = chordTypeChooser
                .ChooseItem();

            // SELECT THE TIMBRE FOR THE BASS VOICE
            ProbabilityChooser<NoteTimbre> bassTimbreChooser = new ProbabilityChooser<NoteTimbre>();

            //bassTimbreChooser
               //.AddItem(NoteTimbre.Eh, 0.1f);

            bassTimbreChooser
                .AddItem(NoteTimbre.Ee, 0.1f);

            bassTimbreChooser
                .AddItem(NoteTimbre.Ah, 0.4f);

            bassTimbreChooser
                .AddItem(NoteTimbre.Oh, 0.05f);

            //bassTimbreChooser
                //.AddItem(NoteTimbre.Oo, 0.05f);

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

            // SELECT THE MELODIC STYLE
            ProbabilityChooser<MelodicStyle> melodicStyleChooser = new ProbabilityChooser<MelodicStyle>();

            melodicStyleChooser
               .AddItem(MelodicStyle.UpTempo, 1f);

            melodicStyleChooser
               .AddItem(MelodicStyle.Ballad, 1f);

            melodicStyleChooser
               .AddItem(MelodicStyle.Medium, 1f);

            AddPreviousChoice(melodicStyleChooser, MelodicStyles, CurrentChunkIndex, 12f);

            MelodicStyles[CurrentChunkIndex] = melodicStyleChooser
                .ChooseItem();

            // SELECT THE TIMBRE FOR THE MELODY
            ProbabilityChooser<NoteTimbre> melodyTimbreChooser = new ProbabilityChooser<NoteTimbre>();

            //melodyTimbreChooser
            //.AddItem(NoteTimbre.Eh, 0.1f);

            melodyTimbreChooser
                .AddItem(NoteTimbre.Ee, 0.1f);

            melodyTimbreChooser
                .AddItem(NoteTimbre.Ah, 0.1f);

            melodyTimbreChooser
                .AddItem(NoteTimbre.Oh, 0.3f);

            //melodyTimbreChooser
            //.AddItem(NoteTimbre.Oo, 0.05f);

            AddPreviousChoice(melodyTimbreChooser, MelodyTimbres, CurrentChunkIndex, 1.5f);

            MelodyTimbres[CurrentChunkIndex] = melodyTimbreChooser
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
            for (int i = 0; i < 7; i++)
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
            for (int i = 0; i < 7; i++)
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

        protected int GetScaleDegree(int degree, int change)
        {
            int newDegree = degree + change;

            if (newDegree >= MusicalScale.AscendingNotes.Length)
            {
                newDegree -= MusicalScale.AscendingNotes.Length;
            }
            
            if (newDegree < 0)
            {
                newDegree += MusicalScale.AscendingNotes.Length;
            }

            return newDegree;
        }

        protected void CreateChoosers()
        {            
            NoteLengthChooserPerStyle = new Dictionary<MelodicStyle, ProbabilityChooser<int>>();
            var chooser = new ProbabilityChooser<int>();

            chooser
                .AddItem(1, 0.1f);

            chooser
                .AddItem(2, 0.25f);

            chooser
                .AddItem(4, 5f);

            chooser
                .AddItem(8, 1f);

            chooser
                .AddItem(12, 0.1f);

            chooser
                .AddItem(16, 1f);

            NoteLengthChooserPerStyle[MelodicStyle.Ballad] = chooser;

            chooser = new ProbabilityChooser<int>();

            chooser
                .AddItem(1, 0.25f);

            chooser
                .AddItem(2, 0.25f);

            chooser
                .AddItem(4, 5f);

            chooser
                .AddItem(8, 0.5f);

            chooser
                .AddItem(12, 0.1f);

            chooser
                .AddItem(16, 0.5f);

            NoteLengthChooserPerStyle[MelodicStyle.Medium] = chooser;

            chooser = new ProbabilityChooser<int>();

            chooser
                .AddItem(1, 0.5f);

            chooser
                .AddItem(2, 1f);

            chooser
                .AddItem(4, 3f);

            chooser
                .AddItem(8, 0.5f);

            chooser
                .AddItem(12, 0.1f);

            chooser
                .AddItem(16, 0.25f);

            NoteLengthChooserPerStyle[MelodicStyle.UpTempo] = chooser;

            RestLengthChooserPerStyle = new Dictionary<MelodicStyle, ProbabilityChooser<int>>();

            chooser = new ProbabilityChooser<int>();

            chooser
                .AddItem(1, 0.05f);

            chooser
                .AddItem(2, 0.1f);

            chooser
                .AddItem(4, 2f);

            chooser
                .AddItem(8, 5f);

            chooser
                .AddItem(12, 2f);

            chooser
                .AddItem(16, 0.05f);

            RestLengthChooserPerStyle[MelodicStyle.Ballad] = chooser;

            chooser = new ProbabilityChooser<int>();

            chooser
                .AddItem(1, 0.1f);

            chooser
                .AddItem(2, 0.5f);

            chooser
                .AddItem(4, 2f);

            chooser
                .AddItem(8, 2f);

            chooser
                .AddItem(12, 0.5f);

            chooser
                .AddItem(16, 0.5f);

            RestLengthChooserPerStyle[MelodicStyle.Medium] = chooser;

            chooser = new ProbabilityChooser<int>();

            chooser
                .AddItem(1, 0.25f);

            chooser
                .AddItem(2, 1f);

            chooser
                .AddItem(4, 1f);

            chooser
                .AddItem(8, 1f);

            chooser
                .AddItem(12, 1f);

            chooser
                .AddItem(16, 1f);

            RestLengthChooserPerStyle[MelodicStyle.UpTempo] = chooser;

            MelodyNoteIsInChordChoosers = new Dictionary<BeatStrength, ProbabilityChooser<bool>>();

            var boolChooser = new ProbabilityChooser<bool>();

            boolChooser
                .AddItem(true, 16);

            boolChooser
                .AddItem(false, 1);

            MelodyNoteIsInChordChoosers[BeatStrength.Strongest] = boolChooser;

            boolChooser = new ProbabilityChooser<bool>();

            boolChooser
                .AddItem(true, 8);

            boolChooser
                .AddItem(false, 1);

            MelodyNoteIsInChordChoosers[BeatStrength.Strong] = boolChooser;

            boolChooser
                .AddItem(true, 2);

            boolChooser
                .AddItem(false, 1);

            MelodyNoteIsInChordChoosers[BeatStrength.Weak] = boolChooser;

            boolChooser
                .AddItem(true, 1);

            boolChooser
                .AddItem(false, 2);

            MelodyNoteIsInChordChoosers[BeatStrength.Off8th] = boolChooser;

            boolChooser
                .AddItem(true, 1);

            boolChooser
                .AddItem(false, 4);

            MelodyNoteIsInChordChoosers[BeatStrength.Off16th] = boolChooser;

            MelodyNoteIsInScaleChoosers = new Dictionary<BeatStrength, ProbabilityChooser<bool>>();

            boolChooser = new ProbabilityChooser<bool>();

            boolChooser
                .AddItem(true, 64);

            boolChooser
                .AddItem(false, 1);

            MelodyNoteIsInScaleChoosers[BeatStrength.Strongest] = boolChooser;

            boolChooser = new ProbabilityChooser<bool>();

            boolChooser
                .AddItem(true, 32);

            boolChooser
                .AddItem(false, 1);

            MelodyNoteIsInScaleChoosers[BeatStrength.Strong] = boolChooser;

            boolChooser
                .AddItem(true, 16);

            boolChooser
                .AddItem(false, 1);

            MelodyNoteIsInScaleChoosers[BeatStrength.Weak] = boolChooser;

            boolChooser
                .AddItem(true, 4);

            boolChooser
                .AddItem(false, 1);

            MelodyNoteIsInScaleChoosers[BeatStrength.Off8th] = boolChooser;

            boolChooser
                .AddItem(true, 4);

            boolChooser
                .AddItem(false, 1);

            MelodyNoteIsInScaleChoosers[BeatStrength.Off16th] = boolChooser;

            MelodicIntervalChooser = new ProbabilityChooser<int>();

            MelodicIntervalChooser
                .AddItem(0, 4);

            MelodicIntervalChooser
                .AddItem(1, 8);

            MelodicIntervalChooser
                .AddItem(2, 8);

            MelodicIntervalChooser
                .AddItem(3, 0.5f);

            MelodicIntervalChooser
                .AddItem(4, 4);

            MelodicIntervalChooser
                .AddItem(5, 0.25f);

            MelodicIntervalChooser
                .AddItem(6, 0.25f);

            MelodicIntervalChooser
                .AddItem(7, 0.5f);

            MelodicIntervalChooser
                .AddItem(8, 0.1f);

            MelodicIntervalChooser
                .AddItem(-1, 8);

            MelodicIntervalChooser
                .AddItem(-2, 2);

            MelodicIntervalChooser
                .AddItem(-3, 8);

            MelodicIntervalChooser
                .AddItem(-4, 2);

            MelodicIntervalChooser
                .AddItem(-5, 0.25f);

            MelodicIntervalChooser
                .AddItem(-6, 0.1f);

            MelodicIntervalChooser
                .AddItem(-7, 0.1f);

            StayOnOffBeatChooser = new Dictionary<MelodicStyle, ProbabilityChooser<bool>>();
            boolChooser = new ProbabilityChooser<bool>();

            boolChooser
                .AddItem(true, 1f);

            boolChooser
                .AddItem(false, 10f);

            StayOnOffBeatChooser[MelodicStyle.Ballad] = boolChooser;

            boolChooser = new ProbabilityChooser<bool>();

            boolChooser
                .AddItem(true, 4f);

            boolChooser
                .AddItem(false, 10f);

            StayOnOffBeatChooser[MelodicStyle.Medium] = boolChooser;

            boolChooser
               .AddItem(true, 1f);

            boolChooser
                .AddItem(false, 2f);

            StayOnOffBeatChooser[MelodicStyle.UpTempo] = boolChooser;
        }

        #endregion
    }
}