namespace KasJam.LD48.Unity.Behaviours.Music
{
    using System.Collections.Generic;

    public class Song
    {
        #region Constructors

        public Song(int noteLengthMin)
        {
            NoteLengthMin = noteLengthMin;
            Voices = new List<SongVoice>();
        }

        #endregion

        #region Members

        public List<SongVoice> Voices { get; protected set; }

        public float NoteLengthMin { get; protected set; }

        public float TotalTime { get; set; }

        #endregion
    }
}