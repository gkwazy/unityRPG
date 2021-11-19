using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Manager;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;
        
        private void Awake() {
             player = GameObject.FindWithTag("Player");
        }

        private void OnEnable() {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl; 
        }

        private void OnDisable() {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }


        void DisableControl(PlayableDirector pd)
        {
          
           player.GetComponent<HeroManager>().enabled = false; 
           player.GetComponent<ActionContoller>().CancelCurrentAction();
        }

        void EnableControl(PlayableDirector pd)
        {
            player.GetComponent<HeroManager>().enabled = true;
        }
    }
}
