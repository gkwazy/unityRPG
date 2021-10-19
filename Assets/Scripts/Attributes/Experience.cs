using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Attributes
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
            // Debug.Log($"{name}'s health before restore is {healthPoints}");
            experiencePoints = (float)state;
            // Debug.Log($"{name}'s health before after is {healthPoints}");
        }

        public float getExperiencePoints()
        {
           return experiencePoints;
        }
    }
}