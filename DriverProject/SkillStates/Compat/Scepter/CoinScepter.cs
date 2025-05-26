using RobDriver.Modules;
using RobDriver.SkillStates.Driver;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.Scepter
{
    public class CoinScepter : Coin
    {
        public override GameObject projectilePrefab => Projectiles.coinScepterProjectile;
    }
}
