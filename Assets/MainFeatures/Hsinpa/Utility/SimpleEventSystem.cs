using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Utility {
    public static class SimpleEventSystem
    {
        public static System.Action<int, object[]> CustomEventListener;
        public static System.Action OnDisposeEvent;

        public static void Send(int tag, params object[] customObjects) {
            try
            {
                if (CustomEventListener != null)
                {
                    CustomEventListener(tag, customObjects);
                }
            }
            catch (System.Exception exception) {
                Debug.LogError("EventSytem SimpleEventSystem Exception Message: "+exception.Message + "\n" + exception.StackTrace);
            }
        }

        public static void Dispose() {
            CustomEventListener = null;

            if (OnDisposeEvent != null)
                OnDisposeEvent.Invoke();
        }
    }
}
