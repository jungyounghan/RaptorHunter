/// <summary>
/// ���� ������ ���� ĳ���� Ŭ����
/// </summary>
public class HunterCharacter : Character
{
    private static readonly string SpeedFloat = "Speed";
    private static readonly string RotationFloat = "Rotation";

    public override void MoveStop()
    {
        getAnimator.SetFloat(SpeedFloat, 0);
    }

    public override void TurnStop()
    {
        getAnimator.SetFloat(RotationFloat, 0);
    }

    public override bool TryAttack()
    {
        return false;
    }

    public override bool TryTurnRight()
    {
        float y = getRigidbody.rotation.eulerAngles.y;
        bool turn = base.TryTurnRight();
        if(turn == true)
        {
            float result = getRigidbody.rotation.eulerAngles.y - y;
            getAnimator.SetFloat(RotationFloat, result);
        }
        return false;
    }

    public override bool TryTurnLeft()
    {
        float y = getRigidbody.rotation.eulerAngles.y;
        bool turn = base.TryTurnLeft();
        if (turn == true)
        {
            float result = getRigidbody.rotation.eulerAngles.y - y;
            getAnimator.SetFloat(RotationFloat, result);
        }
        return false;
    }

    public override bool TryJump()
    {
        bool tryJump = base.TryJump();
        if(tryJump == true)
        {
            //�ִϸ��̼�
        }
        return tryJump;
    }
    public override float GetAdvanceSpeed()
    {
        float speed = base.GetAdvanceSpeed();
        getAnimator.SetFloat(SpeedFloat, +speed);
        return speed;
    }

    public override float GetRetreatSpeed()
    {
        float speed = base.GetRetreatSpeed();
        getAnimator.SetFloat(SpeedFloat, -speed);
        return speed;
    }

}