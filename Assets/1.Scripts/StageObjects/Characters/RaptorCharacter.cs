using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 조종 가능한 랩터 캐릭터 클래스
/// </summary>
[RequireComponent(typeof(LODGroup))]
public sealed class RaptorCharacter : Character
{
    private static readonly int AttackActionHashIndex = Animator.StringToHash("AttackAction");
    private static readonly int AttackSpeedHashIndex = Animator.StringToHash("AttackSpeed");
    private static readonly int Hit1HashIndex = Animator.StringToHash("Hit1");
    private static readonly int Hit2HashIndex = Animator.StringToHash("Hit2");
    private static readonly int Hit3HashIndex = Animator.StringToHash("Hit3");
    private static readonly int HitIndexCount = 3;

    [SerializeField]
    private List<AudioClip> _jumpAudioClips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> _attackAudioClips = new List<AudioClip>();

    [Header("이빨과 발톱"), SerializeField]
    private List<Stinger> _stingers = new List<Stinger>();

    [Header("머리 트랜스폼"), SerializeField]
    private Transform _headTransform = null;

    [Header("공격 쿨타임"), SerializeField]
    private float _attackCoolTime = 0;

    private void SetStingerDamage(uint damage)
    {
        foreach (Stinger stinger in _stingers)
        {
            if (stinger != null)
            {
                stinger.damage = damage;
            }
        }
    }

    public override void DoJumpAction()
    {
        int count = _jumpAudioClips.Count;
        if (count > 0)
        {
            int index = UnityEngine.Random.Range(0, count);
            PlaySound(_jumpAudioClips[index]);
        }
        base.DoJumpAction();
    }

    public override void DoReviveAction()
    {
        base.DoReviveAction();
        Action action = () => SetStingerDamage(0);
        foreach (Stinger stinger in _stingers)
        {
            stinger?.Initialize(action);
        }
    }

    public override void DoHitAction(bool dead)
    {
        if (dead == true)
        {
            SetStingerDamage(0);
            DoDeadAction();
        }
        else if (_attackCoolTime == 0)
        {
            PlaySoundHit();
            int index = UnityEngine.Random.Range(0, HitIndexCount);
            switch(index)
            {
                case 0:
                    getAnimator.SetTrigger(Hit1HashIndex);
                    break;
                case 1:
                    getAnimator.SetTrigger(Hit2HashIndex);
                    break;
                case 2:
                    getAnimator.SetTrigger(Hit3HashIndex);
                    break;
            }
        }
    }

    public override void DoAttackAction(uint damage)
    {
        if (_attackCoolTime == 0)
        {
            int count = _attackAudioClips.Count;
            if (count > 0)
            {
                int index = UnityEngine.Random.Range(0, count);
                PlaySound(_attackAudioClips[index]);
            }
            getAnimator.SetTrigger(AttackActionHashIndex);
            _attackCoolTime = 1 / getAnimator.GetFloat(AttackSpeedHashIndex);
            SetStingerDamage(damage);
        }
    }

    public override void Set(float attackSpeed)
    {
        float value = Mathf.Clamp(attackSpeed, Stat.MinAttackSpeed, Stat.MaxAttackSpeed);
        getAnimator.SetFloat(AttackSpeedHashIndex, value);
    }

    public override void Recharge()
    {
        float deltaTime = Time.deltaTime;
        if (_attackCoolTime > 0)
        {
            _attackCoolTime -= deltaTime;
            if (_attackCoolTime <= 0)
            {
                _attackCoolTime = 0;
                SetStingerDamage(0);
            }
        }
    }

    public override bool IsHuman()
    {
        return Raptor;
    }

    public override Transform GetWeaponTransform()
    {
        return _headTransform;
    }
}