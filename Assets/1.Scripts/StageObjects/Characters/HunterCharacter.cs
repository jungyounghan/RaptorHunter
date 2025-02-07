using UnityEngine;
/// <summary>
/// 조종 가능한 헌터 캐릭터 클래스
/// </summary>

public sealed class HunterCharacter : Character
{
    private static readonly string MoveFloat = "Move";
    private static readonly string TurnFloat = "Turn";
    private static readonly string SpeedFloat = "Speed";

    private readonly int _moveHashIndex = Animator.StringToHash("Move");

    public override void Move(float value)
    {
        if (alive == true)
        {
            if (dash == true)
            {
                getAnimator.SetFloat(MoveFloat, value * 2);
                if (value >= 0)
                {
                    value *= (_moveSpeed + _dashSpeed) * staminaRate;
                }
                else
                {
                    value *= -(_moveSpeed + _dashSpeed) * staminaRate * _reverseRate;
                }
                _currentStamina -= _currentStamina * _dashCost;
            }
            else
            {
                getAnimator.SetFloat(MoveFloat, value);
                value *= _moveSpeed;
            }
            //getRigidbody.MovePosition(getRigidbody.position + getTransform.forward.normalized * value * Time.deltaTime);
        }
    }

    public override void Turn(float value)
    {
        if(alive == true)
        {
            getAnimator.SetFloat(TurnFloat, value);
            //getRigidbody.MoveRotation(getRigidbody.rotation * Quaternion.Euler(0, value * _turnSpeed * Mathf.Rad2Deg * Time.deltaTime, 0));
        }
    }

    public override void MoveStop()
    {
        getAnimator.SetFloat(MoveFloat, 0);
    }

    public override void TurnStop()
    {
        getAnimator.SetFloat(TurnFloat, 0);
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
            //getAnimator.SetFloat(RotationFloat, result);
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
            //getAnimator.SetFloat(RotationFloat, result);
        }
        return false;
    }

    public override bool TryJump()
    {
        bool tryJump = base.TryJump();
        if(tryJump == true)
        {
            //애니메이션
        }
        return tryJump;
    }
    public override float GetAdvanceSpeed()
    {
        float speed = base.GetAdvanceSpeed();
        Debug.Log(speed);
        getAnimator.SetFloat(MoveFloat, +speed);
        return speed;
    }

    public override float GetRetreatSpeed()
    {
        float speed = base.GetRetreatSpeed();
        Debug.Log(speed);
        getAnimator.SetFloat(MoveFloat, -speed);
        return speed;
    }

}