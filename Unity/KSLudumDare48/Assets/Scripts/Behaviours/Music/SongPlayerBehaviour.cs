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
            ["C3"] = 130.81f,
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

            var note = Resources
                .Load<AudioClip>("Sounds/Notes/C3");

            NoteClips["C3"] = note;
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

            string noteName = $"{note.Name}{note.Octave}";

            if (NoteClips
                .ContainsKey(noteName))
            {
                clip = NoteClips[noteName];
            }
            else
            {
                clip = NoteClips["C3"];

                var availableNote = new MusicalNote("C", 3, NoteTimbre.Ah);
                pitch = CalculateFrequencyRatio(availableNote, note);
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