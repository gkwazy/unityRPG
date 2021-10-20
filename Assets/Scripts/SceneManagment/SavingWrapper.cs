using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 1.0f;

        const string defaultSaveFile = "save";

        private IEnumerator Start() 
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();

            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Reset());
                
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }
        public IEnumerator Reset()
        //Still needs work to get the charater to be back in the original postion
        {
            Debug.Log("Fadeout with reset");
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();

            GetComponent<SavingSystem>().Reset(defaultSaveFile);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn(fadeInTime);
        }
    }
}