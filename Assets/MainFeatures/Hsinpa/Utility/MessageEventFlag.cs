using UnityEngine;
namespace Hsinpa.Event
{
    /// <summary>
    /// For multiplayer event id
    /// </summary>
    public class MessageEventFlag : MonoBehaviour
    {

        #region Hsinpa Section (1000 - 5000)
        public class HsinpaEvent
        {
            public class General
            {
                public const int SceneStructLoaded = 3007;
            }

            public class UIEVent
            {
                public const int DayLightSwitchEvent = 3050;
            }
        }
        #endregion

    }
}
