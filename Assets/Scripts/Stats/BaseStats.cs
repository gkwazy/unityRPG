using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progession progession = null;
        [SerializeField] GameObject levelUpParticals = null;

        public event Action onLevelUp;

        int currentLevel = 0;

        private void Start()
         {
            currentLevel = CalLevel();
            Debug.Log("currentlevel " + currentLevel);
            Experience experience = GetComponent<Experience>();
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }


        void UpdateLevel()
        {
            int newLevel = CalLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                LevelUpEffect();
                onLevelUp();
            }

        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticals,transform);
        }

        public float GetStat(Stat stat)
        {
            return progession.GetStat(stat, characterClass, GetLevel()); 
        }

        public int GetLevel()
        {
            if (currentLevel <= 0)
            {
                currentLevel = CalLevel();
            }
            return currentLevel;
        }


        public int CalLevel()
        {
            Experience experience = GetComponent<Experience>();
           
            if (experience == null)
            {
                return startingLevel;
            }

            float currentXP = experience.getExperiencePoints();
            int maxLevel = progession.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int i = 1; i < maxLevel; i++)
            {
                float xpForLevel = progession.GetStat(Stat.ExperienceToLevelUp, characterClass, i);
                if (xpForLevel > currentXP)
                {
                    return i;
                }
            }
            return maxLevel + 1;
        }

    }
}
