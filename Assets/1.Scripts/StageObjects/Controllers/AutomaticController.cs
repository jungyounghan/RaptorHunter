using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 자동 컨트롤러 클래스 
/// </summary>
public sealed class AutomaticController : Controller
{
    [Header("계단 위"), SerializeField]
    private bool _onStair = false;
    [Header("걸음 속도"), SerializeField, Range(Stat.MinWalkSpeed, Stat.MaxWalkSpeed)]
    private float _walkSpeed = 3.5f;
    [Header("사라지는 시간"), SerializeField, Range(0, 10)]
    private float _fadeTime = 2.0f;

    private Controller _target = null;

    private static readonly string Stair = "Stair";


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

    private void OnCollisionStay(Collision collision)
    {
        if(collision.transform.tag == Stair)
        {
            _onStair = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == Stair)
        {
            _onStair = false;
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

    public override void Hit(Vector3 origin, Vector3 direction, uint damage)
    {
        bool alive = this.alive;
        base.Hit(origin, direction, damage);
        if(alive != this.alive)
        {
            StartCoroutine(DoFadeObject());
            IEnumerator DoFadeObject()
            {
                yield return new WaitForSeconds(_fadeTime);
                StopAllCoroutines();
                gameObject.SetActive(false);
            }
        }
    }

    protected override IEnumerator DoProcess()
    {
        while(true)
        {
            if(alive == true)
            {
                getNavMeshAgent.enabled = _onStair || IsGrounded();
                if (getNavMeshAgent.enabled == true && getNavMeshAgent.isOnNavMesh == true)
                {
                    if (_target != null)
                    {
                        Vector3 forward = getTransform.forward;
                        bool dash = _onStair;
                        float speed = _walkSpeed;
                        if (dash == true)
                        {
                            getNavMeshAgent.speed = _walkSpeed + (speed * _dashSpeed * staminaRate);
                            _currentStamina -= _currentStamina * _dashCost;
                        }
                        Vector3 position = _target.transform.position;
                        getNavMeshAgent.SetDestination(position);
                        character.LookAt(position);
                        if (Vector3.Distance(getTransform.position, position) < getNavMeshAgent.stoppingDistance)
                        {
                            Vector3 direction = (position - getTransform.position).normalized;
                            direction.y = 0; // 수직 회전 방지 (바닥에서만 회전)
                            if (direction != Vector3.zero)
                            {
                                getTransform.rotation = Quaternion.Slerp(getTransform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * RotationSpeed);
                            }
                            if (_target.alive == true)
                            {
                                character.DoAttackAction(_attackDamage);
                            }
                            else
                            {
                                //조롱하기
                            }
                            character.DoStopAction();
                        }
                        else
                        {
                            character.DoMoveAction(new Vector2(forward.x, forward.z), dash);
                        }
                    }
                    else
                    {
                        character.DoStopAction();
                    }
                }
                else
                {
                    character.DoJumpAction();
                    yield return new WaitUntil(() => IsGrounded());
                    character.DoLandAction();
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
        }
    }

    public override void Revive()
    {
        getCollider.enabled = true;
        getNavMeshAgent.enabled = false;
        getRigidbody.isKinematic = false;
        _currentStamina = _fullStamina;
        _currentLife = _fullLife;
        _lifeAction?.Invoke(_currentLife, _fullLife, this);
        character.DoReviveAction();
    }

    public void Initialize(Action<uint, uint, Controller> lifeAction, Controller target)
    {
        _lifeAction = lifeAction;
        _target = target;
    }
}