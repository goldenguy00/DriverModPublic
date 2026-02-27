using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class StopCoinSound : MonoBehaviour
    {
        private uint SoundId;
        private bool Played;
        public string SoundEventToPlay;

        void Awake()
        {
            if (!Played)
            {
                Played = true;
                SoundId = AkSoundEngine.PostEvent(SoundEventToPlay, gameObject);
            }
        }

        void OnDestroy()
        {
            AkSoundEngine.StopPlayingID(SoundId);
            Destroy(gameObject);
        }
    }
}