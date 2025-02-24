using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// 조종 가능한 사냥꾼 캐릭터 클래스
/// </summary>
public sealed class HunterCharacter : Character
{ 
    private readonly int JumpHashIndex = Animator.StringToHash("Jump");

    [Header("조준 제약")]
    [SerializeField]
    private MultiAimConstraint _spineConstraint = null;
    [SerializeField]
    private MultiAimConstraint _headConstraint = null;

    [Header("총"), SerializeField]
    private Gun _gun = null;

    [Header("왼손 악력") ,SerializeField]
    private float _leftGrasp = 1.0f;

    private void OnAnimatorIK(int layerIndex)
    {
        if (_leftGrasp == 1)
        {
            getAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _leftGrasp);
            getAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _leftGrasp);
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
    }

    private void SetGun(bool active)
    {
        float value = active == true ? 1 : 0;
        _leftGrasp = value;
        if (_spineConstraint != null)
        {
            _spineConstraint.weight = value;
        }
        if (_headConstraint != null)
        {
            _headConstraint.weight = value;
        }
        if(active == false)
        {
            _gun?.HideLaser();
        }
    }

    public override void LookAt(Vector3 position)
    {
        base.LookAt(position);
        _gun?.LookAt(position);
    }

    public override void DoReviveAction()
    {
        base.DoReviveAction();
        SetGun(true);
    }

    public override void DoJumpAction()
    {
        getAnimator.SetBool(JumpHashIndex, true);
    }

    public override void DoLandAction()
    {
        getAnimator.SetBool(JumpHashIndex, false);
    }

    public override void DoHitAction(bool dead)
    {
        if (dead == false)
        {
            DoHitAction();
        }
        else
        {
            DoDeadAction();
            SetGun(false);
        }
    }

    public override void DoAttackAction(uint damage)
    {
        _gun?.Shot(damage);
    }

    public override void Set(float attackSpeed)
    {
        _gun?.Set(attackSpeed);
    }

    public override void Recharge()
    {
        _gun?.Recharge();
    }

    public override bool IsHuman()
    {
        return Hunter;
    }
}