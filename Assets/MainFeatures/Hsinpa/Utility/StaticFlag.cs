using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Static
{
    public class StaticFlag
    {
        public class Config {
            public const float FOG_INTENSITY_MAX = 0.3f;
            public const float FOG_INTENSITY_MIN = 0.0f;
        }

        public class Input
        {
            public const string Keyboard_1 = "1";
            public const string Keyboard_2 = "2";
            public const string Keyboard_3 = "3";
            public const string Keyboard_4 = "4";
            public const string Keyboard_5 = "5";
            public const string Keyboard_6 = "6";
            public const string Keyboard_7 = "7";
            public const string Keyboard_8 = "8";
        }

        public class Path {
            public const string AddressableList = "addressable_list.json";
        }

        public class PlayerPref {
            public const string DayNightTogglePref = "pref@day_night_toggle";
            public const string FogIntensityPref = "pref@fog_intensity";
            public const string LoadScenePref = "pref@load_scene";


            public const string ContrastTogglePref = "pref@contrast_toggle";
            public const string SaturateTogglePref = "pref@saturate_toggle";
            public const string Color_RTogglePref = "pref@color_r_toggle";
            public const string Color_GTogglePref = "pref@color_g_toggle";
            public const string Color_BTogglePref = "pref@color_b_toggle";
        }
    }
}
