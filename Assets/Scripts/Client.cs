using UnityEngine;
using Grpc.Core;
using static Reachy.Sdk.Joint.JointService;
using static Reachy.Sdk.Kinematics.ArmKinematics;
using static Reachy.Sdk.Camera.CameraService;

public class Client : MonoBehaviour
{
    const string IP = "127.0.0.1";
    const int PORT = 50055;
    const int CAMERA_PORT = 50057;

    private JointServiceClient _jointServiceClient;
    private ArmKinematicsClient _armKinematicsClient;
    private CameraServiceClient _cameraServiceClient;

    private void Awake()
    {
        var channel = new Channel(IP, PORT, ChannelCredentials.Insecure);
        _jointServiceClient = new JointServiceClient(channel);
        _armKinematicsClient = new ArmKinematicsClient(channel);

        var camChannel = new Channel(IP, CAMERA_PORT, ChannelCredentials.Insecure);
        _cameraServiceClient = new CameraServiceClient(camChannel);

        var jointsController = new JointsController();
        jointsController.Init(_jointServiceClient, _armKinematicsClient);

        var cameraController = new CameraController();
        cameraController.Init(_cameraServiceClient);

        ReachyApi.Init(jointsController, cameraController);
    }
}
