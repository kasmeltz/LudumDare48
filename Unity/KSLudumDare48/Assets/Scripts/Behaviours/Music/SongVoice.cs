using System.Collections.Generic;

namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class SongVoice
    {
        #region Constructors

        public SongVoice()
        {
            Events = new List<SongEvent>();
        }

        #endregion

        #region Members

        public int CurrentEventIndex { get; set; }

        public List<SongEvent> Events { get; protected set; }

        #endregion
    }
}