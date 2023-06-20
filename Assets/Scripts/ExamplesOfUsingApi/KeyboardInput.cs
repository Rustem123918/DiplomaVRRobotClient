using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    [SerializeField]
    private GameObject _rightHand;
    [SerializeField]
    private GameObject _rightGripper;
    [SerializeField]
    private GameObject _head;

    private bool _inittialized = false;

    private async void Start()
    {
        _rightHand.transform.position = await ReachyApi.GetHandCurrentPosition(Reachy.Sdk.Kinematics.ArmSide.Right);
        _inittialized = true;
    }

    private void FixedUpdate()
    {
        if (!_inittialized)
            return;

        ReachyApi.MoveHand(Reachy.Sdk.Kinematics.ArmSide.Right, _rightHand.transform.position);
        ReachyApi.RotateHead(_head.transform.rotation.eulerAngles);
        ReachyApi.RotateGripper(Reachy.Sdk.Kinematics.ArmSide.Right, _rightGripper.transform.rotation.y);
    }

    private void Update()
    {
        // Right hand
        if (Input.GetKey(KeyCode.W))
        {
            MoveRightHand(Vector3.right);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MoveRightHand(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            MoveRightHand(Vector3.up);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveRightHand(Vector3.down);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            MoveRightHand(Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            MoveRightHand(Vector3.back);
        }
        // Right gripper
        else if (Input.GetKey(KeyCode.C))
        {
            RotateRightGripper(Vector3.up);
        }
        else if (Input.GetKey(KeyCode.V))
        {
            RotateRightGripper(Vector3.down);
        }
        // Head
        else if (Input.GetKey(KeyCode.Z))
        {
            RotateHead(Vector3.up);
        }
        else if (Input.GetKey(KeyCode.X))
        {
            RotateHead(Vector3.down);
        }
    }

    private void MoveRightHand(Vector3 direction)
    {
        var speed = 0.3f;
        _rightHand.transform.Translate(direction * Time.deltaTime * speed, Space.World);
    }

    private void RotateRightGripper(Vector3 direction)
    {
        var speed = 30f;
        _rightGripper.transform.Rotate(direction, speed * Time.deltaTime);
    }

    private void RotateHead(Vector3 direction)
    {
        var speed = 30f;
        _head.transform.Rotate(direction, speed * Time.deltaTime);
    }
}