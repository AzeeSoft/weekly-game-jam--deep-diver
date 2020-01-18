using UnityEngine;
using Cinemachine;
 
[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class CinemachineVerticalTransposer : CinemachineExtension
{
    public float yOffset;
    public float zOffset;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (!vcam.Follow)
        {
            return;
        }

        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            pos.y = vcam.Follow.position.y + yOffset;
            pos.z = vcam.Follow.position.z + zOffset;
            state.RawPosition = pos;
        }
    }
}