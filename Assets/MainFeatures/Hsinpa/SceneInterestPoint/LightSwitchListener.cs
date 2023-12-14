using Hsinpa.Event;
using Hsinpa.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.GameScene
{
    public class LightSwitchListener : MonoBehaviour
    {
        [SerializeField]
        private bool enable_on_night_time;

        [SerializeField]
        private bool enable_on_day_time;

        public void SetVisibility(bool is_day_time) {

            if (!is_day_time && enable_on_night_time) {
                this.gameObject.SetActive(true);
                return;
            }

            if (is_day_time && enable_on_day_time) {
                this.gameObject.SetActive(true);
                return;
            }

            this.gameObject.SetActive(false);

        }
    }
}