using System.Collections.Generic;

namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class Song
    {
        #region Constructors

        public Song(int tempo)
        {
            Tempo = tempo;
            Events = new List<SongEvent>();
        }

        #endregion

        #region Members

        public List<SongEvent> Events { get; protected set; }

        public int Tempo { get; protected set; }

        #endregion
    }
}