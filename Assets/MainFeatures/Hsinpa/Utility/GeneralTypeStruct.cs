using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hsinpa.Type
{
    public class GeneralTypeStruct 
    {
        public static ConfigFileStruct GlobalConfigFileStruct;

        [System.Serializable]
        public struct ConfigFileStruct {
            public float Project_1_Pos_X;
            public float Project_1_Pos_Y;
            public float Project_1_Pos_Z;

            public float Project_1_Rot_Y;
            public float Project_1_Rot_X;
            public float Project_1_Rot_Z;

            public float Project_2_Pos_X;
            public float Project_2_Pos_Y;
            public float Project_2_Pos_Z;

            public float Project_2_Rot_Y;
            public float Project_2_Rot_X;
            public float Project_2_Rot_Z;

            public bool SyncData;
        }

        [System.Serializable]
        public struct CameraTransform {
            public Vector3 position;
            public Vector3 rotation;
        }

        public static ConfigFileStruct SyncData() {
            string file_path = Path.Combine(Application.streamingAssetsPath, "Data", "config.json");
            
            if (File.Exists(file_path))
            {
                string raw_text = File.ReadAllText(file_path);
                return JsonUtility.FromJson<ConfigFileStruct>(raw_text);
            }

            return default(ConfigFileStruct);
        }
    }
}