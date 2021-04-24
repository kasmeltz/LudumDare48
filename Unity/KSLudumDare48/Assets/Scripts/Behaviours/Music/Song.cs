namespace KasJam.LD48.Unity.Behaviours.Music
{
    using System.Collections.Generic;

    public class Song
    {
        #region Constructors

        public Song(int tempo)
        {
            Tempo = tempo;
            Voices = new List<SongVoice>();
        }

        #endregion

        #region Members

        public List<SongVoice> Voices { get; protected set; }

        public int Tempo { get; protected set; }

        public float TotalTime { get; set; }

        #endregion
    }
}