using UnityEngine;
using System.Threading.Tasks;
using Reachy.Sdk.Joint;
using static Reachy.Sdk.Joint.JointService;
using static Reachy.Sdk.Kinematics.ArmKinematics;
using System.Collections.Generic;
using System.Linq;

public class JointsController : IJointsController
{
    const string NECK_YAW = "neck_yaw";
    const string RIGHT_GRIPPER = "r_gripper";
    const string LEFT_GRIPPER = "l_gripper";

    private JointServiceClient _jointServiceClient;
    private ArmKinematicsClient _armKinematicsClient;

    private List<double> _baseMatrix = new List<double>
    {
        0, 0, -1, 0,
        0, 1, 0, 0,
        1, 0, 0, 0,
        0, 0, 0, 1
    };

    private bool _readyToNextMovement = true;
    private bool _readyToNextHeadMove = true;
    private bool _readyToNextGripperMove = true;
    private bool _readyToGetHandCurrentPosition = true;

    private Vector3 _handCurrentPosition;

    public void Init(JointServiceClient jointServiceClient, ArmKinematicsClient armKinematicsClient)
    {
        _jointServiceClient = jointServiceClient;
        _armKinematicsClient = armKinematicsClient;
    }

    public void MoveHand(Reachy.Sdk.Kinematics.ArmSide armSide, Vector3 targetPosition)
    {
        UpdateBaseMatrixByPosition(targetPosition);

        if (_readyToNextMovement)
            MoveHandInternal(armSide, _baseMatrix);
    }

    public void RotateGripper(Reachy.Sdk.Kinematics.ArmSide armSide, float angleRad)
    {
        if (_readyToNextGripperMove)
            RotateGripperInternal(armSide, angleRad);
    }

    public void RotateHead(Vector3 eulerAngles)
    {
        if (_readyToNextHeadMove)
            RotateHeadInternal(eulerAngles);
    }

    public async Task<Vector3> GetHandCurrentPosition(Reachy.Sdk.Kinematics.ArmSide armSide)
    {
        if (_readyToGetHandCurrentPosition)
            await GetHandCurrentPositionInternal(armSide);

        return _handCurrentPosition;
    }

    private void UpdateBaseMatrixByPosition(Vector3 pos)
    {
        _baseMatrix[3] = pos.x;
        _baseMatrix[7] = pos.y;
        _baseMatrix[11] = pos.z;
    }

    private async Task RotateGripperInternal(Reachy.Sdk.Kinematics.ArmSide armSide, float angleRad)
    {
        _readyToNextGripperMove = false;

        var jointName = armSide == Reachy.Sdk.Kinematics.ArmSide.Right ? RIGHT_GRIPPER : LEFT_GRIPPER;
        var request = new JointsCommand();
        var command = new JointCommand
        {
            Id = new JointId
            {
                Name = jointName
            },
            GoalPosition = angleRad
        };
        request.Commands.Add(command);

        await _jointServiceClient.SendJointsCommandsAsync(request);
        _readyToNextGripperMove = true;
    }

    private async Task RotateHeadInternal(Vector3 eulerAngles)
    {
        _readyToNextHeadMove = false;

        var request = new JointsCommand();
        var command = new JointCommand
        {
            Id = new JointId
            {
                Name = NECK_YAW
            },
            GoalPosition = Deg2Rad(eulerAngles.y)
        };
        request.Commands.Add(command);

        await _jointServiceClient.SendJointsCommandsAsync(request);
        _readyToNextHeadMove = true;
    }

    private async Task MoveHandInternal(Reachy.Sdk.Kinematics.ArmSide armSide, List<double> pose)
    {
        _readyToNextMovement = false;
        var solution = await _armKinematicsClient.ComputeArmIKAsync(new Reachy.Sdk.Kinematics.ArmIKRequest
        {
            Target = new Reachy.Sdk.Kinematics.ArmEndEffector
            {
                Side = armSide,
                Pose = new Reachy.Sdk.Kinematics.Matrix4x4 { Data = { pose } }
            }
        });

        if(solution.Success)
        {
            var armPositions = solution.ArmPosition.Positions;
            var request = new JointsCommand();
            for (int i = 0; i < solution.ArmPosition.Positions.Ids.Count; i++)
            {
                var command = new JointCommand
                {
                    Id = armPositions.Ids[i],
                    GoalPosition = (float?)armPositions.Positions[i]
                };
                request.Commands.Add(command);
            }
            await _jointServiceClient.SendJointsCommandsAsync(request);
        }
        _readyToNextMovement = true;
    }

    private async Task GetHandCurrentPositionInternal(Reachy.Sdk.Kinematics.ArmSide armSide)
    {
        _readyToGetHandCurrentPosition = false;

        List<JointId> ids = new List<JointId>();
        ids.Add(new JointId { Name = "r_shoulder_pitch" });
        ids.Add(new JointId { Name = "r_shoulder_roll" });
        ids.Add(new JointId { Name = "r_arm_yaw" });
        ids.Add(new JointId { Name = "r_elbow_pitch" });
        ids.Add(new JointId { Name = "r_forearm_yaw" });
        ids.Add(new JointId { Name = "r_wrist_pitch" });
        ids.Add(new JointId { Name = "r_wrist_roll" });

        var request = new JointsStateRequest();
        request.Ids.Add(ids);
        request.RequestedFields.Add(JointField.PresentPosition);

        var states = await _jointServiceClient.GetJointsStateAsync(request);

        var positions = new Reachy.Sdk.Kinematics.JointPosition();
        positions.Ids.Add(states.Ids);
        positions.Positions.Add(states.States.Select(s => s.PresentPosition.HasValue ? (double)s.PresentPosition.Value : 0).ToArray());

        var solution = await _armKinematicsClient.ComputeArmFKAsync(new Reachy.Sdk.Kinematics.ArmFKRequest
        {
            ArmPosition = new Reachy.Sdk.Kinematics.ArmJointPosition
            {
                Side = armSide,
                Positions = positions
            }
        });

        _handCurrentPosition.x = (float)solution.EndEffector.Pose.Data[3];
        _handCurrentPosition.y = (float)solution.EndEffector.Pose.Data[7];
        _handCurrentPosition.z = (float)solution.EndEffector.Pose.Data[11];

        _readyToGetHandCurrentPosition = true;
    }

    private float Deg2Rad(float deg)
    {
        return deg * Mathf.Deg2Rad;
    }
}