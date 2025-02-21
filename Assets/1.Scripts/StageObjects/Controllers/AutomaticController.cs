using System.Collections;
using UnityEngine;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 자동 컨트롤러 클래스 
/// </summary>
public sealed class AutomaticController : Controller
{
    [Header("걸음 속도"), Range(Stat.MinWalkSpeed, Stat.MaxWalkSpeed)]
    private float _walkSpeed = 3.5f;

    private enum State
    {

    }

    public Transform target;

    protected override void OnEnable()
    {
        _walkSpeed = getNavMeshAgent.speed;
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        getNavMeshAgent.speed = _walkSpeed;
        base.OnDisable();
    }

    protected override IEnumerator DoProcess()
    {
        while(true)
        {
            if(alive == true)
            {
                getNavMeshAgent.enabled = IsGrounded();
                if (target != null && Vector3.Distance(target.position, getTransform.position) <= getNavMeshAgent.stoppingDistance)
                {
                    //getCharacter.LookAt();
                }

            }
            yield return null;
        }
    }

    public override void Set(Stat stat)
    {
        if (stat != null)
        {
            _fullStamina = Mathf.Clamp(stat.fullStamina, Stat.MinFullStamina, Stat.MaxFullStamina);
            _recoverStamina = Mathf.Clamp(stat.recoverStamina, Stat.MinRecoverStamina, Stat.MaxRecoverStamina);
            _walkSpeed = Mathf.Clamp(stat.walkSpeed, Stat.MinWalkSpeed, Stat.MaxWalkSpeed);
            getNavMeshAgent.speed = _walkSpeed;
            _dashSpeed = Mathf.Clamp(stat.dashSpeed, Stat.MinDashSpeed, Stat.MaxDashSpeed);
            _dashCost = Mathf.Clamp(stat.dashCost, Stat.MinDashCost, Stat.MaxDashCost);
            _reverseRate = Mathf.Clamp(stat.reverseRate, Stat.MinReverseRate, Stat.MaxReverseRate);
            getCharacter.Set(stat.attackSpeed);
            _attackDamage = stat.attackDamage;
            _fullLife = stat.fullLife;
            _staminaAction?.Invoke(_currentStamina, _fullStamina);
            _lifeAction?.Invoke(_currentLife, _fullLife, this);
        }
    }
}