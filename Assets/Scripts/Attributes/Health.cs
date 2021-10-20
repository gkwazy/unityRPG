using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour,ISaveable
    {
        float healthPoints = -1f;

        bool isDead = false;


    private void Awake() 
    {
        if (healthPoints < 0)
        {

        }
        healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
    }

        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            healthPoints = (float) state;
           
            if (healthPoints <= 0)
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
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints == 0)
            {
              Die();
              AwardExperience(instigator);
            }
        } 

        public float getPercentage()
        {
            Debug.Log($"{name}'s health before restore is { GetComponent<BaseStats>().GetStat(Stat.Health)}");
            return (healthPoints / GetComponent<BaseStats>().GetStat(Stat.Health)) * 100;
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
            Experience experience = instigator.GetComponent<Experience>();
            if (experience != null)
            {
                experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
            }
            
        }
    }
}