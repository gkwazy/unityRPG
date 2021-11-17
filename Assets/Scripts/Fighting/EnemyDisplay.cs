using System;
using System.Collections;
using System.Collections.Generic;
using RPG.HealthObject;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Fighting
{
    public class EnemyDisplay : MonoBehaviour
    {
        AttackCombat target;
        private void Awake() 
        {
            target = GameObject.FindWithTag("Player").GetComponent<AttackCombat>();
        }

        private void Update() 
        {
            if (target.GetTarget() == null)
            {
                GetComponent<Text>().text = "-";
                return;
            }
            Health health = target.GetTarget();
            
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.restoreHealth(), health.GetMaxHealth());
        }
    }
}
