using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using RPG.DeveloperTools;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

namespace RPG.HealthObject
{
    public class Health : MonoBehaviour,ISaveable
    {

        [SerializeField] float regenerationPercentage = 70;
        [SerializeField] bool StartDead = false;
        [SerializeField] UnityEvent<float> damageTaken;
        [SerializeField] UnityEvent whenKilled;
       


        SlowLoad<float> healthPoints;

        bool killed = false;
        bool expGiven = false;

    private void Awake() 
    {
        healthPoints = new SlowLoad<float>(GetStartingHealth);
        if (StartDead){
            Kill();
        }    
    }

    private float GetStartingHealth()
    {
        return GetComponent<BaseStats>().GetStat(Stat.Health);
    }

    private void Start() 
    {
       healthPoints.Initialize();
    }
    
    private void OnEnable() 
    {
        GetComponent<BaseStats>().lvlGain += RegenerateHealth;
    }

        

    private void OnDisable() 
    {
        GetComponent<BaseStats>().lvlGain -= RegenerateHealth;
    }
       

        public object GetWeaponState()
        {
            return healthPoints.value;
        }

        public void RestoreWeaponState(object state)
        {
            healthPoints.value = (float) state;
           
            if (healthPoints.value <= 0)
            {
                Kill();
            }
        }


        public bool Killed()
        {
            return killed;
        }

        
        public void DamageHealth(GameObject player, float damageAmount)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damageAmount, 0);
            
            if (healthPoints.value == 0)
            {
              whenKilled.Invoke();
              Kill();

              if (!expGiven)
              {
                 AwardExperience(player);
              }
            }
            else
            {
                damageTaken.Invoke(damageAmount);
            }
        } 

        public float restoreHealth()
        {
            return healthPoints.value;
        }

        public float GetMaxHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float getHealthPercentage()
        {
            return convertToFraction() * 100;
        }

        public float convertToFraction()
        {
            return (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void Kill()
        {
            StartCoroutine(DeathRoutine());
        }  

        IEnumerator DeathRoutine()
        {
            yield return 0;

            if (killed)
            {
                yield break;
            }
            else
            {
                killed = true;
                GetComponent<Animator>().SetTrigger("die");
                GetComponent<ActionContoller>().CancelCurrentAction();
            }

           
        }

        private void AwardExperience(GameObject instigator)
        {
            expGiven = true;
            Experience experience = instigator.GetComponent<Experience>();
            if (experience != null)
            {
                experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
            }
            
        }

        public void restoreHealth(float healingPercent)
        {
            float truePercent = healingPercent/100;
            float max = GetMaxHealth();
            print("amount healed "  + Mathf.Min(healthPoints.value + (max * truePercent), max));
            print ("heal caled " + ((max * truePercent)));
            healthPoints.value = Mathf.Min(healthPoints.value + (max * truePercent), max);
        }

        private void RegenerateHealth()
        {
           float regenHealthPoints = (GetComponent<BaseStats>().GetStat(Stat.Health) * regenerationPercentage) /100; 
           healthPoints.value = Mathf.Min(healthPoints.value, regenHealthPoints);
        }
    }
}