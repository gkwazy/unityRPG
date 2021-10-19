using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progession", menuName = "Stats/New Progession", order = 0)]
    public class Progession : ScriptableObject
    {

        [SerializeField] ProgessionCharacterClass[] characterClasses = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
           foreach (ProgessionCharacterClass progression in characterClasses)
           {
               if (progression.characterClass != characterClass) continue;

               foreach (ProgessionStat progressStat in progression.stats)
               {
                  if (progressStat.stat != stat) continue;

                  if (progressStat.levels.Length < level) continue;
                  
                    return progressStat.levels[level -1];
                  
                  
               }
           }
           return 0;
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