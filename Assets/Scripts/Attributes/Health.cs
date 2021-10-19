using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour,ISaveable
    {
        [SerializeField] float healthPoints = 100f;

        bool isDead = false;


    private void Awake() 
    {
        healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
    }

        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            Debug.Log($"{name}'s health before restore is {healthPoints}");
          
            healthPoints = (float) state;
            Debug.Log($"{name}'s health before after is {healthPoints}");

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