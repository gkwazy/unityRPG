using System.Collections.Generic;

namespace RPG.Stats
{
   public interface IModifierProvider
   {
       IEnumerable<float> GetBonusAdditive(Stat stat);
       IEnumerable<float> GetBonusPercentage(Stat stat);
   } 
}
