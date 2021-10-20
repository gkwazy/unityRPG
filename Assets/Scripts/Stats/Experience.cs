using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        public object CaptureState()
        {
           return experiencePoints;
        }

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }

        public float getExperiencePoints()
        {
           return experiencePoints;
        }
    }
}