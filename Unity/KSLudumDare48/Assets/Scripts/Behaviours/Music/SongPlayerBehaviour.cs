namespace KasJam.LD48.Unity.Behaviours.Music
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("LD48/SongPlayer")]
    public class SongPlayerBehaviour : BehaviourBase
    {
        #region Members

        public AudioSource[] AudioSources;

        public Song Song { get; protected set; }

        public float BeatCounter { get; protected set; }

        protected Dictionary<string, AudioClip> NoteClips { get; set; }

        protected bool IsPlaying { get; set; }

        public static Dictionary<string, float> NoteFrequencies = new Dictionary<string, float>
        {
            ["C1"] = 33f,
            ["C2"] = 65.41f,
            ["G2"] = 98f,
            ["C3"] = 130.81f,
            ["F3"] = 175f,
            ["C4"] = 262,
            ["F4"] = 349,
            ["A#4"] = 466,
        };

        public List<MusicalNote> AvailableNotes = new List<MusicalNote>
        {
            new MusicalNote("C", 1, NoteTimbre.Ah),
            new MusicalNote("C", 2, NoteTimbre.Ah),
            new MusicalNote("G", 2, NoteTimbre.Ah),
            new MusicalNote("C", 3, NoteTimbre.Ah),
            new MusicalNote("F", 3, NoteTimbre.Ah),
            new MusicalNote("C", 4, NoteTimbre.Ah),
            new MusicalNote("F", 4, NoteTimbre.Ah),
            new MusicalNote("A#", 4, NoteTimbre.Ah)
        };

        #endregion

        #region Events

        public event EventHandler SongFinished;

        protected void OnSongFinished()
        {
            SongFinished?
                .Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<SongEventEventArgs> SongEventPlayed;

        protected void OnSongEventPlayed(SongEvent songEvent)
        {
            SongEventPlayed?
                .Invoke(this, new SongEventEventArgs { SongEvent = songEvent });
        }

        #endregion

        #region Public Methods

        public void SetSong(Song song)
        {
            StopPlaying();

            Song = song;

            BeatCounter = 0;

            foreach(var voice in song.Voices)
            {
                voice.CurrentEventIndex = 0;
            }

            IsPlaying = false;
        }

        public void StartPlaying()
        {
            IsPlaying = true;
        }

        public void StopPlaying()
        {
            foreach (AudioSource audioSource in AudioSources)
            {
                audioSource.clip = null;
                audioSource
                    .Stop();
            }

            IsPlaying = false;
        }


        #endregion

        #region Protected Methods

        protected void LoadNotes()
        {
            NoteClips = new Dictionary<string, AudioClip>();

            NoteClips["C1Eh"] = Resources
                .Load<AudioClip>("Sounds/Notes/C1Eh");

            NoteClips["C1Ee"] = Resources
                .Load<AudioClip>("Sounds/Notes/C1Ee");

            NoteClips["C1Ah"] = Resources
                .Load<AudioClip>("Sounds/Notes/C1Ah");

            NoteClips["C1Oh"] = Resources
                .Load<AudioClip>("Sounds/Notes/C1Oh");

            NoteClips["C1Oo"] = Resources
                .Load<AudioClip>("Sounds/Notes/C1Oo");

            NoteClips["C2Eh"] = Resources
                .Load<AudioClip>("Sounds/Notes/C2Eh");

            NoteClips["C2Ee"] = Resources
                .Load<AudioClip>("Sounds/Notes/C2Ee");

            NoteClips["C2Ah"] = Resources
                .Load<AudioClip>("Sounds/Notes/C2Ah");

            NoteClips["C2Oh"] = Resources
                .Load<AudioClip>("Sounds/Notes/C2Oh");

            NoteClips["C2Oo"] = Resources
                .Load<AudioClip>("Sounds/Notes/C2Oo");

            NoteClips["G2Eh"] = Resources
                .Load<AudioClip>("Sounds/Notes/G2Eh");

            NoteClips["G2Ee"] = Resources
                .Load<AudioClip>("Sounds/Notes/G2Ee");

            NoteClips["G2Ah"] = Resources
                .Load<AudioClip>("Sounds/Notes/G2Ah");

            NoteClips["G2Oh"] = Resources
                .Load<AudioClip>("Sounds/Notes/G2Oh");

            NoteClips["G2Oo"] = Resources
                .Load<AudioClip>("Sounds/Notes/G2Oo");

            NoteClips["C3Eh"] = Resources
                .Load<AudioClip>("Sounds/Notes/C3Eh");

            NoteClips["C3Ee"] = Resources
                .Load<AudioClip>("Sounds/Notes/C3Ee");

            NoteClips["C3Ah"] = Resources
                .Load<AudioClip>("Sounds/Notes/C3Ah");

            NoteClips["C3Oh"] = Resources
                .Load<AudioClip>("Sounds/Notes/C3Oh");

            NoteClips["C3Oo"] = Resources
                .Load<AudioClip>("Sounds/Notes/C3Oo");

            NoteClips["F3Eh"] = Resources
                .Load<AudioClip>("Sounds/Notes/F3Eh");

            NoteClips["F3Ee"] = Resources
                .Load<AudioClip>("Sounds/Notes/F3Ee");

            NoteClips["F3Ah"] = Resources
                .Load<AudioClip>("Sounds/Notes/F3Ah");

            NoteClips["F3Oh"] = Resources
                .Load<AudioClip>("Sounds/Notes/F3Oh");

            NoteClips["F3Oo"] = Resources
                .Load<AudioClip>("Sounds/Notes/F3Oo");

            NoteClips["C4Eh"] = Resources
                .Load<AudioClip>("Sounds/Notes/C4Eh");

            NoteClips["C4Ee"] = Resources
                .Load<AudioClip>("Sounds/Notes/C4Ee");

            NoteClips["C4Ah"] = Resources
                .Load<AudioClip>("Sounds/Notes/C4Ah");

            NoteClips["C4Oh"] = Resources
                .Load<AudioClip>("Sounds/Notes/C4Oh");

            NoteClips["C4Oo"] = Resources
                .Load<AudioClip>("Sounds/Notes/C4Oo");

            NoteClips["F4Eh"] = Resources
                .Load<AudioClip>("Sounds/Notes/F4Eh");

            NoteClips["F4Ee"] = Resources
                .Load<AudioClip>("Sounds/Notes/F4Ee");

            NoteClips["F4Ah"] = Resources
                .Load<AudioClip>("Sounds/Notes/F4Ah");

            NoteClips["F4Oh"] = Resources
                .Load<AudioClip>("Sounds/Notes/F4Oh");

            NoteClips["F4Oo"] = Resources
                .Load<AudioClip>("Sounds/Notes/F4Oo");

            NoteClips["A#4Eh"] = Resources
                .Load<AudioClip>("Sounds/Notes/Bb4Eh");

            NoteClips["A#4Ee"] = Resources
                .Load<AudioClip>("Sounds/Notes/Bb4Ee");

            NoteClips["A#4Ah"] = Resources
                .Load<AudioClip>("Sounds/Notes/Bb4Ah");

            NoteClips["A#4Oh"] = Resources
                .Load<AudioClip>("Sounds/Notes/Bb4Oh");

            NoteClips["A#4Oo"] = Resources
                .Load<AudioClip>("Sounds/Notes/Bb4Oo");
        }

        protected float CalculateFrequencyRatio(MusicalNote fromNote, MusicalNote toNote)
        {
            string noteKey = $"{fromNote.Name}{fromNote.Octave}";

            if (!NoteFrequencies
                .ContainsKey(noteKey))
            {
                throw new ArgumentException($"'{noteKey}' does not exist in the note frequency table");
            }

            int fromIndex = MusicalScale
                .GetNoteAbsoluteIndex(fromNote.Name, 8, fromNote.Octave);

            int toIndex = MusicalScale
                .GetNoteAbsoluteIndex(toNote.Name, 8, toNote.Octave);

            float fromFrequency = NoteFrequencies[noteKey];

            int indexDifference = fromIndex - toIndex;

            float toFrequency = fromFrequency * Mathf.Pow(2, indexDifference / 12f);
         
            return toFrequency / fromFrequency;
        }

        protected void PlayNote(int voiceNumber, float volume, MusicalNote note)
        {
            if (voiceNumber >= AudioSources.Length)
            {
                throw new ArgumentException($"You trying to play a note using voice number {voiceNumber + 1} but we only have {AudioSources.Length} dude!");
            }

            float pitch = 1;
            AudioClip clip;

            string noteName = $"{note.Name}{note.Octave}{note.Timbre}";

            if (NoteClips
                .ContainsKey(noteName))
            {
                clip = NoteClips[noteName];
            }
            else
            {
                float min = float.MaxValue;
                MusicalNote closest = null;

                int requestedIndex = MusicalScale
                    .GetNoteAbsoluteIndex(note.Name, 8, note.Octave);

                foreach(var possibleNote in AvailableNotes)
                {
                    int noteIndex = MusicalScale
                        .GetNoteAbsoluteIndex(possibleNote.Name, 8, possibleNote.Octave);

                    int difference = Mathf.Abs(noteIndex - requestedIndex);
                    if (difference < min)
                    {
                        min = difference;
                        closest = possibleNote;
                    }
                }

                //Debug
                    //.Log($"REQUESTED '{note.Name}{note.Octave}{note.Timbre}' ACTUAL {closest.Name}{closest.Octave}{closest.Timbre}");

                clip = NoteClips[$"{closest.Name}{closest.Octave}{note.Timbre}"];
                pitch = CalculateFrequencyRatio(closest, note);
            }

            var source = AudioSources[voiceNumber];

            source
                .Stop();
            source.clip = null;
            source.pitch = pitch;
            source.volume = volume;
            source.clip = clip;
            source
                .Play();
        }

        protected void PlayRest(int voiceNumber)
        {
            if (voiceNumber >= AudioSources.Length)
            {
                throw new ArgumentException($"You trying to play a note using voice number {voiceNumber + 1} but we only have {AudioSources.Length} dude!");
            }

            var source = AudioSources[voiceNumber];

            source
                .Stop();
            source.clip = null;
        }

        protected void FinishSong()
        {
            IsPlaying = false;
            StopPlaying();

            OnSongFinished();
        }

        protected void HandleSongEvent(SongEvent songEvent)
        {
            if (songEvent.Note != null)
            {
                PlayNote(songEvent.VoiceNumber, songEvent.Volume, songEvent.Note);
                OnSongEventPlayed(songEvent);
            }
            else
            {
                PlayRest(songEvent.VoiceNumber);
            }
        }

        #endregion

        #region Unity

        protected override void Awake()
        {
            base
                .Awake();

            BeatCounter = 0;

            LoadNotes();
        }

        protected void Update()
        {
            if (Song == null)
            {
                return;
            }

            if (!IsPlaying)
            {
                return;
            }

            if (BeatCounter >= Song.TotalTime)
            {
                FinishSong();
                return;
            }

            BeatCounter += Time.deltaTime;

            for (int voiceIndex = 0; voiceIndex < Song.Voices.Count; voiceIndex++)
            {
                var voice = Song.Voices[voiceIndex];

                if (voice.CurrentEventIndex < voice.Events.Count)
                {
                    var currentSongEvent = voice.Events[voice.CurrentEventIndex];

                    if (BeatCounter >= currentSongEvent.OccursAt)
                    {
                        HandleSongEvent(currentSongEvent);
                        voice.CurrentEventIndex++;
                    }
                }
            }                                   
        }

        #endregion
    }
}