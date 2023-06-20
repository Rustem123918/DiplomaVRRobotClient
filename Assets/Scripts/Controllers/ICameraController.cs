using UnityEngine;

public interface ICameraController
{
    public Texture2D GetImage(Reachy.Sdk.Camera.CameraId cameraId);
}