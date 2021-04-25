namespace KasJam.LD48.Unity.Behaviours.Music
{
    using System.Collections.Generic;
    using System.Linq;

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

        #region Public Methods

        public void SortEvents()
        {
            Events = Events
                .OrderBy(o => o.OccursAt)
                .ToList();
        }

        #endregion
    }
}