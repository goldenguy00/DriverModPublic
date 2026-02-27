
using RoR2;
using RoR2.Orbs;

namespace RobDriver.Modules.Components
{
    public class CoinOrbEffect : OrbEffect
    {
        private void Awake()
        {
            base.onArrival.AddListener(Event);
        }
        
        private void Event()
        {
            base.transform.Find("Trail").SetParent(null);
        }
    }
}
