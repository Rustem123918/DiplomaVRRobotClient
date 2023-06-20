using UnityEngine;
using System.Threading.Tasks;
using static Reachy.Sdk.Camera.CameraService;

public class CameraController : ICameraController
{
    const int resWidth = 960;
    const int resHeight = 720;

    private CameraServiceClient _cameraServiceClient;

    private Texture2D _texture;
    private bool _readyToNextImage = true;

    public void Init(CameraServiceClient cameraServiceClient)
    {
        _cameraServiceClient = cameraServiceClient;
        _texture = new Texture2D(resWidth, resHeight);
    }

    public Texture2D GetImage(Reachy.Sdk.Camera.CameraId cameraId)
    {
        if (_readyToNextImage)
            GetImageInternal(cameraId);

        return _texture;
    }

    private async Task GetImageInternal(Reachy.Sdk.Camera.CameraId cameraId)
    {
        _readyToNextImage = false;
        var result = await _cameraServiceClient.GetImageAsync(new Reachy.Sdk.Camera.ImageRequest
        {
            Camera = new Reachy.Sdk.Camera.Camera
            {
                Id = cameraId
            }
        });
        _texture.LoadImage(result.Data.ToByteArray());
        _readyToNextImage = true;
    }
}