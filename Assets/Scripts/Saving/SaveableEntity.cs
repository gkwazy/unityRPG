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
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";
        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

    #if UNITY_EDITOR
        void Update()
        {
            if(Application.IsPlaying(gameObject)) return;

            if (string.IsNullOrEmpty(gameObject.scene.path))return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            
            if( string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                    property.stringValue = System.Guid.NewGuid().ToString();
                    serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }

        private bool IsUnique(string stringValue)
        {
            if (globalLookup.ContainsKey(stringValue))
            {
                return true;
            }
            if (globalLookup[stringValue] == this)
            {
                return true;
            }

            if (globalLookup[stringValue] == null)
            {
                globalLookup.Remove(stringValue);
                return true;
            }

            if (globalLookup[stringValue].GetUniqueIdentifier() != stringValue)
            {
                globalLookup.Remove(stringValue);
            }

            return false;
        }
#endif

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach(ISaveable saveable in GetComponents<ISaveable>())
            {
               state[saveable.GetType().ToString()] = saveable.GetWeaponState(); 
            }
           return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>) state;
            foreach(ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreWeaponState(stateDict[typeString]);
                }
            }
        }
    }
}

