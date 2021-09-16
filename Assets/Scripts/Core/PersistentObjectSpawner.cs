using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectPrefab;

        static bool hasSpawned = false;

        private void Awake() {
            print("awake spawner");
            if (hasSpawned) 
            {
                print("none in spawner");
                return;
            }
            SpawnPersistentObject();
            hasSpawned = true;

        }

        private void SpawnPersistentObject()
        {
            GameObject persistentObject  = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }

    }
}
