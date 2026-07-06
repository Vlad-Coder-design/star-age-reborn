using System.IO;
using UnityEngine;

namespace StarAge3D
{
    public class SaveManager : MonoBehaviour
    {
        const string FileName = "star-age-3d-save.json";
        public StarAgeSaveData Data { get; private set; }

        public void Load()
        {
            string path = Path.Combine(Application.persistentDataPath, FileName);
            if (File.Exists(path))
            {
                try
                {
                    Data = JsonUtility.FromJson<StarAgeSaveData>(File.ReadAllText(path));
                }
                catch
                {
                    Data = null;
                }
            }

            if (Data == null) Data = new StarAgeSaveData();
            Data.EnsureDefaults();
        }

        public void Save()
        {
            if (Data == null) return;
            Data.EnsureDefaults();
            string path = Path.Combine(Application.persistentDataPath, FileName);
            File.WriteAllText(path, JsonUtility.ToJson(Data, true));
        }
    }
}
