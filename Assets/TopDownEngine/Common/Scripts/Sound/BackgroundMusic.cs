using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Add this class to a GameObject to have it play a background music when instanciated.
    /// </summary>
    [AddComponentMenu("TopDown Engine/Sound/BackgroundMusic")]
    public class BackgroundMusic : MonoBehaviour
    {
        /// the background music
        [Tooltip("the audio clip to use as background music")]
        public AudioClip SoundClip;
        /// whether or not the music should loop
        [Tooltip("whether or not the music should loop")]
        public bool Loop = true;

        /// <summary>
        /// Gets the AudioSource associated to that GameObject, and asks the GameManager to play it.
        /// </summary>
        protected virtual void Start()
        {
            MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
            options.Loop = Loop;
            options.Location = Vector3.zero;
            options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Music;
            
            MMSoundManagerSoundPlayEvent.Trigger(SoundClip, options);
        }
    }
}