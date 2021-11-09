using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using RPG.DeveloperTools;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour,ISaveable
    {

        [SerializeField] float regenerationPercentage = 70;
        [SerializeField] UnityEvent<float> takeDamage;
        [SerializeField] UnityEvent onDie;


        SlowLoad<float> healthPoints;

        bool isDead = false;
        bool experianceRewarded = false;

    private void Awake() 
    {
        healthPoints = new SlowLoad<float>(GetInitialHealth);    
    }

    private float GetInitialHealth()
    {
        return GetComponent<BaseStats>().GetStat(Stat.Health);
    }

    private void Start() 
    {
       healthPoints.Initialize();
    }
    
    private void OnEnable() 
    {
        GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
    }

        

    private void OnDisable() 
    {
        GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
    }
       

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float) state;
           
            if (healthPoints.value <= 0)
            {
                Die();
            }
        }


        public bool IsDead()
        {
            return isDead;
        }

        
        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            
            if (healthPoints.value == 0)
            {
              onDie.Invoke();
              Die();

              if (!experianceRewarded)
              {
                 AwardExperience(instigator);
              }
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        } 

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float getPercentage()
        {
            return getFraction() * 100;
        }

        public float getFraction()
        {
            return (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }  

        private void AwardExperience(GameObject instigator)
        {
            experianceRewarded = true;
            Experience experience = instigator.GetComponent<Experience>();
            if (experience != null)
            {
                experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
            }
            
        }

        public void Heal(float healingPercent)
        {
            float truePercent = healingPercent/100;
            float max = GetMaxHealthPoints();
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