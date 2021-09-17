using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;


namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
       public void Save(string saveFile)
       {
            string path = GetPathFromFile(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                Transform playerTransform = GetPlayerTransform();

                BinaryFormatter formatter = new BinaryFormatter();
                SerializableVector3 postion = new SerializableVector3(playerTransform.position);

                formatter.Serialize(stream,postion);
            }
       }

        

        public void Load(string saveFile)
        {
                print("Loading from " + GetPathFromFile(saveFile));
                string path = GetPathFromFile(saveFile);
                print("Saving to " + path);
                using (FileStream stream = File.Open(path, FileMode.Open))
                {
                    Transform playerTransform = GetPlayerTransform();
                    BinaryFormatter formatter = new BinaryFormatter();
                    SerializableVector3 position = (SerializableVector3) formatter.Deserialize(stream);
                    playerTransform.position =position.ToVector(); 
                }
                
        }

        private Transform GetPlayerTransform()
        {
            return GameObject.FindWithTag("Player").transform;
        }

       private byte[] SerializeVector(Vector3 vector)
       {
           byte[] vectorBytes = new byte[12];
            BitConverter.GetBytes(vector.x).CopyTo(vectorBytes, 0);
            BitConverter.GetBytes(vector.y).CopyTo(vectorBytes, 4);
            BitConverter.GetBytes(vector.z).CopyTo(vectorBytes, 8);
            return vectorBytes;
       }

       private Vector3 DeserializeVector(byte[] bytes)
       {
           Vector3 result = new Vector3();
           result.x = BitConverter.ToSingle(bytes,0);
           result.y = BitConverter.ToSingle(bytes, 4);
           result.z = BitConverter.ToSingle(bytes, 8);
           return result;
       }

       private string GetPathFromFile(string saveFile)
       {
           
           return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
       }

    }
}

