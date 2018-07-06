using System.Collections.Generic;

namespace SrpTask
{
    public interface IGameEngine
    {
        void PlaySpecialEffect(string effectName);
        List<IEnemy> GetEnemiesNear(RpgPlayer player);
    }
}