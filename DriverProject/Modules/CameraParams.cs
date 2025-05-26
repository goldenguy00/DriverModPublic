using HG.BlendableTypes;
using RoR2;
using UnityEngine;

internal enum DriverCameraParams
{
    DEFAULT,
    AIM_PISTOL,
    AIM_SNIPER,
    EMOTE
}

namespace RobDriver.Modules
{
    internal static class CameraParams
    {
        internal static CharacterCameraParamsData defaultCameraParams;
        internal static CharacterCameraParamsData aimCameraParams;
        internal static CharacterCameraParamsData sniperAimCameraParams;
        internal static CharacterCameraParamsData emoteCameraParams;

        internal static void InitializeParams()
        {
            defaultCameraParams = NewCameraParams(/*"ccpRobDriver",*/ 70f, 1.4f, new Vector3(0f, 0f, -8f));
            aimCameraParams = NewCameraParams(/*"ccpRobDriverAim",*/ 70f, 0.8f, new Vector3(1f, 0f, -4f));
            sniperAimCameraParams = NewCameraParams(/*"ccpRobDriverSniperAim",*/ 70f, 0.8f, new Vector3(0f, 0f, 0.75f), true);
            emoteCameraParams = NewCameraParams(/*"ccpRobDriverEmote",*/ 70f, 0.4f, new Vector3(0f, 0f, -6f));
        }

        private static CharacterCameraParamsData NewCameraParams(float pitch, float pivotVerticalOffset, Vector3 idealPosition, bool isFirstPerson = false)
        {
            return new CharacterCameraParamsData
            {
                maxPitch = pitch,
                minPitch = -pitch,
                pivotVerticalOffset = pivotVerticalOffset,
                idealLocalCameraPos = idealPosition,
                isFirstPerson = isFirstPerson,
                wallCushion = 0.1f,
                overrideFirstPersonFadeDuration = 0f,
                fov = new BlendableFloat
                {
                    value = 60f,
                    alpha = 0f
                }
            };
        }

        internal static CameraTargetParams.CameraParamsOverrideHandle OverrideCameraParams(CameraTargetParams camParams, DriverCameraParams camera, float transitionDuration = 0.5f)
        {
            var request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = GetNewParams(camera),
                priority = 0,
            };

            return camParams.AddParamsOverride(request, transitionDuration);
        }

        internal static CharacterCameraParams CreateCameraParamsWithData(DriverCameraParams camera)
        {
            CharacterCameraParams newCameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            newCameraParams.name = "ccpRobDriverDefaultParams";
            newCameraParams.data = GetNewParams(camera);

            return newCameraParams;
        }

        internal static CharacterCameraParamsData GetNewParams(DriverCameraParams camera) => camera switch
        {
            DriverCameraParams.AIM_PISTOL => aimCameraParams,
            DriverCameraParams.AIM_SNIPER => sniperAimCameraParams,
            DriverCameraParams.EMOTE => emoteCameraParams,
            _ => defaultCameraParams,
        };
    }
}