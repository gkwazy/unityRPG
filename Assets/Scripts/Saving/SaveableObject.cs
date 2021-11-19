using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using System;

namespace RPG.Saving
{

    [ExecuteAlways]
    public class SaveableObject : MonoBehaviour
    {
        [SerializeField] string uniqueKey = "";
        static Dictionary<string, SaveableObject> fileKeyDictionary = new Dictionary<string, SaveableObject>();

    #if UNITY_EDITOR
        void Update()
        {
            if(Application.IsPlaying(gameObject)) return;

            if (string.IsNullOrEmpty(gameObject.scene.path))return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueKey");

            
            if( string.IsNullOrEmpty(property.stringValue) || !IsKeyUsed(property.stringValue))
            {
                    property.stringValue = System.Guid.NewGuid().ToString();
                    serializedObject.ApplyModifiedProperties();
            }

            fileKeyDictionary[property.stringValue] = this;
        }

        private bool IsKeyUsed(string keyValue)
        {
            if (fileKeyDictionary.ContainsKey(keyValue))
            {
                return true;
            }
            if (fileKeyDictionary[keyValue] == this)
            {
                return true;
            }

            if (fileKeyDictionary[keyValue] == null)
            {
                fileKeyDictionary.Remove(keyValue);
                return true;
            }

            if (fileKeyDictionary[keyValue].GetKey() != keyValue)
            {
                fileKeyDictionary.Remove(keyValue);
            }

            return false;
        }
#endif

        public string GetKey()
        {
            return uniqueKey;
        }

        public object SaveObjectInfo()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach(ISaveable saveable in GetComponents<ISaveable>())
            {
               state[saveable.GetType().ToString()] = saveable.GetWeaponState(); 
            }
           return state;
        }

        public void LoadObjectInfo(object info)
        {
            Dictionary<string, object> infoDictionary = (Dictionary<string, object>) info;
            foreach(ISaveable saveable in GetComponents<ISaveable>())
            {
                string type = saveable.GetType().ToString();
                if (infoDictionary.ContainsKey(type))
                {
                    saveable.RestoreWeaponState(infoDictionary[type]);
                }
            }
        }
    }
}

