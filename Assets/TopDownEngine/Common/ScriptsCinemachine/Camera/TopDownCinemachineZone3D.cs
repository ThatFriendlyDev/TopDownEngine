using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Add this class to a box collider and it'll let you define a zone that, when entered, enables a virtual camera, letting you define sections inside your level easily
    /// </summary>
    public class TopDownCinemachineZone3D : MMCinemachineZone3D
    {
        protected CinemachineCameraController _cinemachineCameraController;
        
        /// <summary>
        /// On Awake, adds a camera controller if needed
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                _cinemachineCameraController = VirtualCamera.gameObject.MMGetComponentAroundOrAdd<CinemachineCameraController>();
                _cinemachineCameraController.ConfineCameraToLevelBounds = false;    
            }
        }

        /// <summary>
        /// Enables/Disables the camera
        /// </summary>
        /// <param name="state"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        protected override IEnumerator EnableCamera(bool state, int frames)
        {
            yield return base.EnableCamera(state, frames);
            if (state)
            {
                _cinemachineCameraController.FollowsAPlayer = true;
                _cinemachineCameraController.StartFollowing();
            }
            else
            {
                _cinemachineCameraController.StopFollowing();
                _cinemachineCameraController.FollowsAPlayer = false;
            }
        }
    }    
}

