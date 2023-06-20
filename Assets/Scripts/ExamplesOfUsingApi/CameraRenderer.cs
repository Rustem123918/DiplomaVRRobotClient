using UnityEngine;

public class CameraRenderer : MonoBehaviour
{
    private Texture2D _texture;

    private void FixedUpdate()
    {
        _texture = ReachyApi.GetImage(Reachy.Sdk.Camera.CameraId.Right);
    }

    private void OnGUI()
    {
        if (Event.current.type.Equals(EventType.Repaint))
        {
            if(_texture != null)
                Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);
        }
    }
}