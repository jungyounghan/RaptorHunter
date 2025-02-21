using UnityEngine;

/// <summary>
/// ���� ������ ���� ĳ���� Ŭ����
/// </summary>
public class RaptorCharacter : Character
{
    private static readonly int AttackActionHashIndex = Animator.StringToHash("AttackAction");
    private static readonly int AttackSpeedHashIndex = Animator.StringToHash("AttackSpeed");

    [Header("���� �ӵ�(�ʴ� n��)"), SerializeField, Range(Stat.MinAttackSpeed, Stat.MaxAttackSpeed)]
    private float _attackSpeed = 10;
    private float _attackCoolTime = 0;
    public override void DoJumpAction()
    {
    }

    public override void DoLandAction()
    {
    }

    public override void DoHitAction(bool dead)
    {
    }

    public override void DoAttackAction(uint damage)
    {
        if(_attackCoolTime == 0)
        {
            getAnimator.SetTrigger(AttackActionHashIndex);
            _attackCoolTime = 1 / _attackSpeed;
        }
    }

    public override void Set(float attackSpeed)
    {
        float value = Mathf.Clamp(attackSpeed, Stat.MinAttackSpeed, Stat.MaxAttackSpeed);
        _attackSpeed = value;
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
}