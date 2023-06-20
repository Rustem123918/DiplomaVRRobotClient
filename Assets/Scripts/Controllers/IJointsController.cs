using UnityEngine;
using System.Threading.Tasks;

public interface IJointsController
{
    public void MoveHand(Reachy.Sdk.Kinematics.ArmSide armSide, Vector3 targetPosition);
    public void RotateHead(Vector3 eulerAngles);
    public void RotateGripper(Reachy.Sdk.Kinematics.ArmSide armSide, float angleRad);
    public Task<Vector3> GetHandCurrentPosition(Reachy.Sdk.Kinematics.ArmSide armSide);
}