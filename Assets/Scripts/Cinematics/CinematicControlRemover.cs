using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;
        
        private void Start() {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
             player = GameObject.FindWithTag("Player");
        }
        void DisableControl(PlayableDirector pd)
        {
          
           player.GetComponent<PlayerController>().enabled = false; 
           player.GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        void EnableControl(PlayableDirector pd)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
