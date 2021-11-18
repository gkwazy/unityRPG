using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.SceneManagement;
using RPG.Saving;
using RPG.Manager;

namespace RPG.SceneMangement
{
    public class Portal : MonoBehaviour
    {

        enum DestinationIdentifier
        {
            A,B,C,D,E,F
        }


        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeWaitTime = 0.5f;

        void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
           
        }

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            HeroManager playerController = GameObject.FindWithTag("Player").GetComponent<HeroManager>();
            playerController.enabled = false;
            yield return fader.FadeOut(fadeOutTime);

            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            HeroManager newPlayerController = GameObject.FindWithTag("Player").GetComponent<HeroManager>();
            newPlayerController.enabled = false;

            savingWrapper.Load();

            yield return new WaitForEndOfFrame();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);
           
            newPlayerController.enabled = true;
            Destroy(gameObject);
        }


        private Portal GetOtherPortal()
        {
           foreach (Portal portal in FindObjectsOfType<Portal>())
           {
               if(portal == this) continue;
               if(portal.destination != destination) continue;
               return portal;
           }
           return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
