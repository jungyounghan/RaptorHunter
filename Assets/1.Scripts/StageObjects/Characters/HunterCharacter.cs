using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// 조종 가능한 사냥꾼 캐릭터 클래스
/// </summary>
public sealed class HunterCharacter : Character
{
    private static readonly int Hit1HashIndex = Animator.StringToHash("Hit1");
    private static readonly int Hit2HashIndex = Animator.StringToHash("Hit2");
    private static readonly int HitIndexCount = 2;

    [Header("조준 제약")]
    [SerializeField]
    private MultiAimConstraint _headConstraint = null;
    [SerializeField]
    private MultiAimConstraint _spineConstraint = null;

    [SerializeField]
    private List<AudioClip> _landAudioClips = new List<AudioClip>();

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
        if(_headConstraint != null)
        {
            _headConstraint.weight = value;
        }
        if (_spineConstraint != null)
        {
            _spineConstraint.weight = value;
        }
        if(active == false)
        {
            _gun?.HideLine();
        }
    }

    public override void LookAt(Vector3 position)
    {
        base.LookAt(position);
        _gun?.LookAt(position);
    }

    public override void DoLandAction()
    {
        base.DoLandAction();
        int count = _landAudioClips.Count;
        if (count > 0)
        {
            int index = Random.Range(0, count);
            PlayStepSound(_landAudioClips[index]);
        }
        PlaySoundHit();
    }

    public override void DoReviveAction()
    {
        base.DoReviveAction();
        SetGun(true);
    }

    public override void DoHitAction(bool dead)
    {
        if (dead == false)
        {
            PlaySoundHit();
            int index = Random.Range(0, HitIndexCount);
            switch(index)
            {
                case 0:
                    getAnimator.SetTrigger(Hit1HashIndex);
                    break;
                case 1:
                    getAnimator.SetTrigger(Hit2HashIndex);
                    break;
            }
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

    public override Transform GetWeaponTransform()
    {
        if(_gun != null)
        {
            return _gun.GetMuzzleTransform();
        }
        return null;
    }
}