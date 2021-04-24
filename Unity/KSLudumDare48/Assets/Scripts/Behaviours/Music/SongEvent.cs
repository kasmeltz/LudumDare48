namespace KasJam.LD48.Unity.Behaviours.Music
{
    public class SongEvent
    {
        #region Constructors

        public SongEvent(float occursAt, params MusicalNote[] notes)
        {
            OccursAt = occursAt;
            Notes = notes;
        }

        #endregion

        #region Members

        public MusicalNote[] Notes { get; protected set; }

        public float OccursAt { get; protected set; }

        #endregion
    }
}