using RoR2;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class DestroyWeaponOnTimer : MonoBehaviour
    {
        public float duration = 60f;
        public float age;

        public void FixedUpdate()
        {
            age += Time.fixedDeltaTime;
            if (age > duration)
            {
                age = 0f;
                Achievements.DriverSupplyDropAchievement.weaponHasDespawned = true;
                Object.Destroy(base.gameObject);
            }
        }

        public void OnDisable()
        {
            age = 0f;
        }
    }
}
