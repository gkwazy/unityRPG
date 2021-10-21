using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progession", menuName = "Stats/New Progession", order = 0)]
    public class Progession : ScriptableObject
    {

        [SerializeField] ProgessionCharacterClass[] characterClasses = null;
        
        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {

            BuildLookup();

            float[] levels =  lookupTable[characterClass][stat];
            if (levels.Length < level)
            {
                return 0;
                
            }
            return levels[level -1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup()
        {
           if ( lookupTable == null) 
           {
               lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

                foreach (ProgessionCharacterClass progression in characterClasses)
                {
                    var statLookupTable = new Dictionary<Stat, float[]>();

                    foreach (ProgessionStat progressStat in progression.stats)
                    {
                        statLookupTable[progressStat.stat] = progressStat.levels;
                    }

                    lookupTable[progression.characterClass] = statLookupTable;
                }
           }
        }

        [System.Serializable]
        class ProgessionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgessionStat[] stats;
        }

        [System.Serializable]
        class ProgessionStat
        {
            public Stat stat;
            public float[] levels;
        }



    }

}