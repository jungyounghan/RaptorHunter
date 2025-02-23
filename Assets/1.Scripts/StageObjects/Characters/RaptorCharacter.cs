using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ ���� ĳ���� Ŭ����
/// </summary>
public sealed class RaptorCharacter : Character
{
    private static readonly int AttackActionHashIndex = Animator.StringToHash("AttackAction");
    private static readonly int AttackSpeedHashIndex = Animator.StringToHash("AttackSpeed");

    [SerializeField]
    private List<AudioClip> _attackAudioClips = new List<AudioClip>();

    [Header("�̻��� ����"), SerializeField]
    private List<Stinger> stingers = new List<Stinger>();

    [Header("���� ��Ÿ��"), SerializeField]
    private float _attackCoolTime = 0;

    private void SetStingerDamage(uint damage)
    {
        foreach (Stinger stinger in stingers)
        {
            if (stinger != null)
            {
                stinger.damage = damage;
            }
        }
    }

    public override void DoReviveAction()
    {
        base.DoReviveAction();
        foreach (Stinger stinger in stingers)
        {
            if (stinger != null)
            {
                //���þ� ����
            }
        }
    }

    public override void DoJumpAction()
    {
    }

    public override void DoLandAction()
    {
    }

    public override void DoHitAction(bool dead)
    {
        if (dead == false)
        {
            DoHitAction();
        }
        else
        {
            SetStingerDamage(0);
            DoDeadAction();
        }
    }

    public override void DoAttackAction(uint damage)
    {
        if (_attackCoolTime == 0)
        {
            int count = _attackAudioClips.Count;
            if (count > 0)
            {
                int index = Random.Range(0, count);
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
}