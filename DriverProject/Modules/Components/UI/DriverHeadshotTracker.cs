using UnityEngine;

namespace RobDriver.Modules.Components.UI
{
    public class DriverHeadshotTracker : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(this, 0.1f);
        }
    }
}