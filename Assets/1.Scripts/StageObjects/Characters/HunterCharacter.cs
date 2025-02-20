using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// 조종 가능한 사냥꾼 캐릭터 클래스
/// </summary>
public sealed class HunterCharacter : Character
{
    private readonly int MoveHashIndex = Animator.StringToHash("Move");
    private readonly int TurnHashIndex = Animator.StringToHash("Turn");
    private readonly int JumpHashIndex = Animator.StringToHash("Jump");
    private readonly int HitHashIndex = Animator.StringToHash("Hit");
    private readonly int DieHashIndex = Animator.StringToHash("Die");

    //[Header("효과음"), SerializeField]
    //private AudioClip _footAudioClip = null;    //사람이 내는 소리

    [Header("조준 제약")]
    [SerializeField]
    private MultiAimConstraint _spineConstraint = null;
    [SerializeField]
    private MultiAimConstraint _headConstraint = null;

    [Header("총"), SerializeField]
    private Gun _gun = null;

    private void OnAnimatorIK(int layerIndex)
    {
        getAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        getAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        if (_gun != null)
        {
            Gun.Handle handle = _gun.GetMagazineHandle();
            if (handle != null)
            {
                getAnimator.SetIKPosition(AvatarIKGoal.LeftHand, handle.position);
                getAnimator.SetIKRotation(AvatarIKGoal.LeftHand, handle.rotation);
            }
        }
    }

    public override void LookAt(Vector3 position)
    {
        base.LookAt(position);
        _gun?.ShowLaserAim();
    }

    public override void DoReviveAction()
    {
        base.DoReviveAction();
        if (_spineConstraint != null)
        {
            _spineConstraint.weight = 1;
        }
        if (_headConstraint != null)
        {
            _headConstraint.weight = 1;
        }
    }

    public override void DoJumpAction()
    {
        getAnimator.SetBool(JumpHashIndex, true);
    }

    public override void DoLandAction()
    {
        getAnimator.SetBool(JumpHashIndex, false);
    }

    public override void DoStopAction()
    {
        float deltaTime = Time.deltaTime;
        float turn = getAnimator.GetFloat(TurnHashIndex);
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
            getAnimator.SetFloat(TurnHashIndex, turn);
        }
        float move = getAnimator.GetFloat(MoveHashIndex);
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
            getAnimator.SetFloat(MoveHashIndex, move);
        }
    }

    public override void DoHitAction(bool dead)
    {
        if (dead == false)
        {
            getAnimator.SetTrigger(HitHashIndex);
        }
        else
        {
            getAnimator.SetTrigger(DieHashIndex);
            if(_spineConstraint != null)
            {
                _spineConstraint.weight = 0;
            }
            if (_headConstraint != null)
            {
                _headConstraint.weight = 0;
            }
        }
    }

    public override void DoAttackAction(uint damage)
    {
        if(_gun != null && _gun.TryShot(damage) == true)
        {
        }
    }

    public override void DoMoveAction(Vector2 direction, bool dash)
    {
        if(dash == true)
        {
            direction *= 2;
        }
        getAnimator.SetFloat(TurnHashIndex, direction.x);
        getAnimator.SetFloat(MoveHashIndex, direction.y);
    }

    public override void Recharge()
    {
        _gun?.Recharge();
    }
}