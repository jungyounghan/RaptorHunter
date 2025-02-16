using UnityEngine;

/// <summary>
/// 조종 가능한 사냥꾼 캐릭터 클래스
/// </summary>
public sealed class HunterCharacter : Character
{
    private readonly int _moveHashIndex = Animator.StringToHash("Move");
    private readonly int _turnHashIndex = Animator.StringToHash("Turn");

    public override void DoJumpAction()
    {

    }

    public override void DoLandAction()
    {

    }

    public override void DoStopAction()
    {
        float deltaTime = Time.deltaTime;
        float turn = getAnimator.GetFloat(_turnHashIndex);
        if (turn != 0)
        {
            if(turn > 0)
            {
                turn -= deltaTime;
                if(turn < 0)
                {
                    turn = 0;
                }
            }
            else
            {
                turn += deltaTime;
                if(turn > 0)
                {
                    turn = 0;
                }
            }
            getAnimator.SetFloat(_turnHashIndex, turn);
        }
        float move = getAnimator.GetFloat(_moveHashIndex);
        if(move != 0)
        {
            if (move > 0)
            {
                move -= deltaTime;
                if (move < 0)
                {
                    move = 0;
                }
            }
            else
            {
                move += deltaTime;
                if (move > 0)
                {
                    move = 0;
                }
            }
            getAnimator.SetFloat(_moveHashIndex, move);
        }
    }

    public override void DoMoveAction(Vector2 direction, float speed, bool dash)
    {
        if(dash == true)
        {
            direction *= 2;
        }
        getAnimator.SetFloat(_turnHashIndex, direction.x);
        getAnimator.SetFloat(_moveHashIndex, direction.y);
    }

    public override float GetForwardSize()
    {
        return 0.5f;
    }
}