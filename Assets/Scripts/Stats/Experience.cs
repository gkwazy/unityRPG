using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        public event Action onExperienceGained;

        public object GetWeaponState()
        {
           return experiencePoints;
        }

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public void RestoreWeaponState(object state)
        {
            experiencePoints = (float)state;
        }

        public float getExperiencePoints()
        {
           return experiencePoints;
        }
    }
}