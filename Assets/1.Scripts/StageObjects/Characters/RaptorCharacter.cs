using UnityEngine;

/// <summary>
/// 조종 가능한 랩터 캐릭터 클래스
/// </summary>
public class RaptorCharacter : Character
{
    private static readonly int AttackActionHashIndex = Animator.StringToHash("AttackAction");
    private static readonly int AttackSpeedHashIndex = Animator.StringToHash("AttackSpeed");
    private readonly int DieHashIndex = Animator.StringToHash("Die");

    private float _attackCoolTime = 0;

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
            //윽 효과음
        }
        else
        {
            getAnimator.SetTrigger(DieHashIndex);
        }
    }

    public override void DoAttackAction(uint damage)
    {
        if (_attackCoolTime == 0)
        {
            getAnimator.SetTrigger(AttackActionHashIndex);
            _attackCoolTime = 1 / getAnimator.GetFloat(AttackSpeedHashIndex);
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
            if (_attackCoolTime < 0)
            {
                _attackCoolTime = 0;
            }
        }
    }

    public override bool IsHuman()
    {
        return Raptor;
    }
}