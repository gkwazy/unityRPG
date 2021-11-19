using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {

        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = (int)state["lastSceneBuildIndex"];
            }
            yield return SceneManager.LoadSceneAsync(buildIndex);
            LoadObjectData(state);
        }

       public void Save(string saveFile)
       {
           Dictionary<string, object> state = LoadFile(saveFile);
           SaveObjectData(state);
           SaveFile(saveFile, state);
       }

        public void Load(string saveFile)
        {
            LoadObjectData(LoadFile(saveFile));
                
        }

        public void Reset(string saveFile)
        {
            DeleteFile(saveFile);
        }

        private void DeleteFile(string saveFile)
        {
            string path = GetFilePath(saveFile);
            File.Delete(path);
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            
            string path = GetFilePath(saveFile);
            if(!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetFilePath(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream,state);
            }
        }


        private void SaveObjectData(Dictionary<string, object> state)
        {
           foreach(SaveableObject saveable in FindObjectsOfType<SaveableObject>())
           {
               state[saveable.GetKey()] = saveable.SaveObjectInfo();
           }
           state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void LoadObjectData(Dictionary<string, object> state)
        {
            Dictionary<string, object> stateDict = state;
            foreach (SaveableObject saveable in FindObjectsOfType<SaveableObject>())
            {
                string id = saveable.GetKey();
                if (state.ContainsKey(id))
                {
                    saveable.LoadObjectInfo(state[id]);
                }
               
            }
        }

        private string GetFilePath(string saveFile)
       {
           return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
       }

    }
}

