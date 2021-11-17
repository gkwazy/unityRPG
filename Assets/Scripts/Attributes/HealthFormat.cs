using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthFormat : MonoBehaviour
    {

        Health playerHealth;
        private void Awake() 
        {
            playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update() 
        {
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", playerHealth.restoreHealth(), playerHealth.GetMaxHealth());
        }
    }
}
