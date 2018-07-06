using SrpTask.Constants;

namespace SrpTask.Services
{
    public class RpgPlayer
    {
        private readonly IGameEngine _gameEngine;

        public int Health { get; set; }

        public int Armour { get; set; }

        public RpgPlayer(IGameEngine gameEngine)
        {
            _gameEngine = gameEngine;
        }

        public void TakeDamage(int damage)
        {
            if (damage < Armour)
            {
                _gameEngine.PlaySpecialEffect(Effects.Parry);
                return;
            }

            var damageToDeal = damage - Armour;
            Health -= damageToDeal;
            
            _gameEngine.PlaySpecialEffect(Effects.LotsOfGore);
        }
    }
}