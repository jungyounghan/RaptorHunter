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
    [Header("멈추는 거리"), SerializeField, Range(Stat.MinStoppingDistance, Stat.MaxStoppingDistance)]
    private float _stoppingDistance = 5;
    [Header("사라지는 시간"), SerializeField, Range(0, 10)]
    private float _fadeTime = 2.0f;

    private Controller _target = null;

    private static readonly float MaxAngle = 180;

    private void Update()
    {
        if (Time.timeScale > 0)
        {
            if (_currentStamina < _fullStamina)
            {
                _currentStamina += Time.deltaTime * _recoverStamina;
                if (_currentStamina > _fullStamina)
                {
                    _currentStamina = _fullStamina;
                }
            }
            if (alive == true && landing == true)
            {
                character.Recharge();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.transform.tag == StairTag)
        {
            _onStair = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == StairTag)
        {
            _onStair = false;
        }
    }

    protected override void OnEnable()
    {
        _walkSpeed = getNavMeshAgent.speed;
        _stoppingDistance = getNavMeshAgent.stoppingDistance;
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        getNavMeshAgent.speed = _walkSpeed;
        getNavMeshAgent.stoppingDistance = _stoppingDistance;
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
                getNavMeshAgent.enabled = _onStair || landing;
                if (getNavMeshAgent.enabled == true && getNavMeshAgent.isOnNavMesh == true)
                {
                    if (_target != null)
                    {
                        Vector3 forward = getTransform.forward;
                        Vector3 myPosition = getTransform.position;
                        Vector3 targetPosition = _target.transform.position;
                        getNavMeshAgent.SetDestination(targetPosition);
                        bool findAlly = false;
                        Transform transform = character.GetWeaponTransform();
                        if (transform != null && Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit))
                        {
                            transform = raycastHit.collider.transform;
                            while (transform != null)
                            {
                                IHittable hittable = transform.GetComponent<IHittable>();
                                if (hittable != null && hittable as AutomaticController)
                                {
                                    findAlly = true;
                                    getNavMeshAgent.stoppingDistance = 0;
                                    break;
                                }
                                transform = transform.parent;
                            }
                        }
                        if(findAlly == false)
                        {
                            getNavMeshAgent.stoppingDistance = _stoppingDistance;
                        }
                        float angle = 0;
                        Vector3 direction = (targetPosition - getTransform.position).normalized;
                        direction.y = 0; // 수직 회전 방지 (바닥에서만 회전)
                        if (direction != Vector3.zero)
                        {
                            Quaternion quaternion = getTransform.rotation;
                            getTransform.rotation = Quaternion.Slerp(getTransform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * RotationSpeed);
                            angle = Quaternion.Angle(quaternion, getTransform.rotation);
                            float directionSign = Mathf.Sign(Vector3.Dot(Vector3.Cross(forward, direction), getTransform.up));
                            if (directionSign > 0)
                            {
                                angle /= MaxAngle;
                            }
                            else if (directionSign < 0)
                            {
                                angle /= -MaxAngle;
                            }
                        }
                        if (Vector3.Distance(myPosition, targetPosition) < getNavMeshAgent.stoppingDistance)
                        {
                            Vector3 point = _target.GetHitPoint();
                            character.LookAt(point);                           
                            if (_target.alive == true)
                            {
                                character.DoAttackAction(_attackDamage);
                            }
                            character.DoStopAction();
                        }
                        else
                        {
                            bool dash = _onStair || Vector3.Distance(myPosition, targetPosition) < getNavMeshAgent.stoppingDistance * 2;
                            if (dash == true)
                            {
                                getNavMeshAgent.speed = _walkSpeed + (_walkSpeed * _dashSpeed * staminaRate);
                                _currentStamina -= _currentStamina * _dashCost;
                            }
                            character.DoMoveAction(new Vector2(angle, Mathf.Clamp(Vector3.Dot(myPosition, targetPosition), -1, 1)), dash);
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
                    yield return new WaitUntil(() => landing);
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
            _stoppingDistance = Mathf.Clamp(stat.stoppingDistance, Stat.MinStoppingDistance, Stat.MaxStoppingDistance);
            getNavMeshAgent.stoppingDistance = _stoppingDistance;
            character.Set(stat.attackSpeed);
            _attackDamage = stat.attackDamage;
            _fullLife = stat.fullLife;
        }
    }

    public override void Revive()
    {
        getCollider.enabled = true;
        getNavMeshAgent.enabled = false;
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