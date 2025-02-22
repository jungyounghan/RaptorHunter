using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 자동 컨트롤러 클래스 
/// </summary>
public sealed class AutomaticController : Controller
{
    [Header("걸음 속도"), Range(Stat.MinWalkSpeed, Stat.MaxWalkSpeed)]
    private float _walkSpeed = 3.5f;

    public Transform target;

    private Action<uint, uint, Controller> _lifeAction = null;

    private void Update()
    {
        if (_currentStamina < _fullStamina)
        {
            _currentStamina += Time.deltaTime * _recoverStamina;
            if (_currentStamina > _fullStamina)
            {
                _currentStamina = _fullStamina;
            }
        }
        if(alive == true)
        {
            character.Recharge();
        }
    }

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
                if (getNavMeshAgent.enabled == true)
                {
                    if (target != null)
                    {
                        Vector3 position = getTransform.position;
                        Vector3 forward = getTransform.forward;
                        Quaternion rotation = getTransform.rotation;
                        getNavMeshAgent.SetDestination(target.position);
                        yield return null;
                        if (position != getTransform.position || rotation != getTransform.rotation)
                        {
                            character.DoMoveAction(new Vector2(forward.x, forward.z), false);
                        }
                        else
                        {
                            character.DoStopAction();
                        }
                    }
                    else
                    {

                    }
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
            getNavMeshAgent.stoppingDistance = Mathf.Clamp(stat.stoppingDistance, Stat.MinStoppingDistance, Stat.MaxStoppingDistance);
            character.Set(stat.attackSpeed);
            _attackDamage = stat.attackDamage;
            _fullLife = stat.fullLife;
            _lifeAction?.Invoke(_currentLife, _fullLife, this);
        }
    }

    public override void Hit(Vector3 origin, Vector3 direction, uint damage)
    {
        if (alive == true)
        {
            if (_currentLife > damage)
            {
                _currentLife -= damage;
                character.DoHitAction(false);
            }
            else
            {
                _currentLife = 0;
                character.DoHitAction(_fullLife > 0);
            }
            _lifeAction?.Invoke(_currentLife, _fullLife, this);
        }
    }

    public override void Revive()
    {
        _currentStamina = _fullStamina;
        _currentLife = _fullLife;
        _lifeAction?.Invoke(_currentLife, _fullLife, this);
        character.DoReviveAction();
    }

    public void Initialize(Action<uint, uint, Controller> lifeAction)
    {
        _lifeAction = lifeAction;
    }
}