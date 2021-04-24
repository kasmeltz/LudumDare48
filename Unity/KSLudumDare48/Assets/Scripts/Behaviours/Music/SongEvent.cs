namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class SongEvent
    {
        #region Constructors

        public SongEvent(float occursAt, int voiceNumber, float volume, MusicalNote note)
        {
            OccursAt = occursAt;
            VoiceNumber = voiceNumber;
            Note = note;
            Volume = volume;
        }

        #endregion

        #region Members

        public float OccursAt { get; protected set; }

        public int VoiceNumber { get; protected set; }

        public float Volume { get; protected set; }

        public MusicalNote Note { get; protected set; }

        #endregion
    }
}