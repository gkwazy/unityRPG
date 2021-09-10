using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;

        private void Update() 
        {
            GameObject player = GameObject.FindWithTag("Player"); 
            if (Vector3.Distance(transform.position, player.transform.position) <= chaseDistance)
            {
                print(this.name + " Needs to chase the player");
            }
        }
    }
}