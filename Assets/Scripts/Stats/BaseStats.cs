using System;
using System.Collections;
using System.Collections.Generic;
using RPG.DeveloperTools;
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
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;

        SlowLoad<int> currentLevel;
        Experience experience;

        private void Awake() {
            experience= GetComponent<Experience>();
            currentLevel = new SlowLoad<int>(CalLevel);
        }

        private void Start()
         {
            currentLevel.Initialize();
        }

        private void OnEnable() 
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable() {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }


        void UpdateLevel()
        {
            int newLevel = CalLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
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
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + (GetPercentageModifier(stat)/100));
        }


        private float GetBaseStat(Stat stat)
        {
            return progession.GetStat(stat, characterClass, GetLevel());
        }


        public int GetLevel()
        {
            return currentLevel.value;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            float total = 0;
            if(!shouldUseModifiers)
            {
                return total;
            }

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach ( float modifier in provider.GetAdditiveModifier(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            float total = 0;
            if (!shouldUseModifiers)
            {
                return total;
            }
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
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
