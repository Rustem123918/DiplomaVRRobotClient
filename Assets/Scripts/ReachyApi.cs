using UnityEngine;

public static class ReachyApi
{
    private static IJointsController _jointsController;
    private static ICameraController _cameraController;
    private static bool _isInitialized;

    public static void Init(IJointsController jointsController, ICameraController cameraController)
    {
        _jointsController = jointsController;
        _cameraController = cameraController;
        _isInitialized = true;
    }

    public static void MoveHand(Reachy.Sdk.Kinematics.ArmSide armSide, Vector3 targetPosition)
    {
        if (_isInitialized)
            _jointsController.MoveHand(armSide, targetPosition);
        else
            throw new System.Exception("Try use MoveHand method when ReachyApi is not initialized");
    }

    public static void RotateHead(Vector3 eulerAngles)
    {
        if (_isInitialized)
            _jointsController.RotateHead(eulerAngles);
        else
            throw new System.Exception("Try use RotateHead method when ReachyApi is not initialized");
    }

    public static void RotateGripper(Reachy.Sdk.Kinematics.ArmSide armSide, float angleRad)
    {
        if (_isInitialized)
            _jointsController.RotateGripper(armSide, angleRad);
        else
            throw new System.Exception("Try use RotateGripper method when ReachyApi is not initialized");
    }

    public static async System.Threading.Tasks.Task<Vector3> GetHandCurrentPosition(Reachy.Sdk.Kinematics.ArmSide armSide)
    {
        if (_isInitialized)
            return await _jointsController.GetHandCurrentPosition(armSide);
        else
            throw new System.Exception("Try use GetHandCurrentPosition method when ReachyApi is not initialized");

        return Vector3.zero;
    }

    public static Texture2D GetImage(Reachy.Sdk.Camera.CameraId cameraId)
    {
        if (_isInitialized)
            return _cameraController.GetImage(cameraId);
        else
            throw new System.Exception("Try use GetImage method when ReachyApi is not initialized");

        return null;
    }
}