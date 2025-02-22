using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 입력 컨트롤러 클래스 
/// </summary>
public sealed class ManualController : Controller
{
    private static readonly string VerticalKey = "Vertical";
    private static readonly string HorizontalKey = "Horizontal";
    private static readonly string DashKey = "Dash";
    private static readonly string AttackKey = "Fire1";

    private static readonly float AimDistance = 5;

    private Action<float, float> _staminaAction = null;
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
            _staminaAction?.Invoke(_currentStamina, _fullStamina);
        }
        if (alive == true)
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                Vector3 position = getTransform.position;
                Vector3 forward = getTransform.forward;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                Vector3 point = ray.origin + ray.direction * AimDistance;
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity) && Vector3.Dot(forward, hit.point - position) > 0)
                {
                    point = hit.point;
                }
                character.LookAt(point);
            }
            bool attack = Input.GetButton(AttackKey);
            if(attack == true)
            {
                character.DoAttackAction(_attackDamage);
            }
            character.Recharge();
        }
    }

    protected override IEnumerator DoProcess()
    {
        while (true)
        {
            if (alive == true)
            {
                getNavMeshAgent.enabled = IsGrounded();
                if (getNavMeshAgent.enabled == true && getNavMeshAgent.isOnNavMesh == true)
                {
                    bool dash = Input.GetButton(DashKey);
                    float walk = Input.GetAxis(VerticalKey);
                    float turn = Input.GetAxis(HorizontalKey);
                    float deltaTime = Time.deltaTime; ;
                    Vector2 direction = new Vector2(turn, walk);
                    float speed = getNavMeshAgent.speed;
                    if (dash == true)
                    {
                        speed += speed * _dashSpeed * staminaRate;
                        _currentStamina -= _currentStamina * _dashCost;
                        _staminaAction?.Invoke(_currentStamina, _fullStamina);
                    }
                    if (direction.y < 0)
                    {
                        speed *= _reverseRate;
                    }
                    Vector3 position = getTransform.position;
                    Vector3 forward = getTransform.forward;
                    Quaternion rotation = getTransform.rotation;
                    getTransform.rotation = Quaternion.Slerp(rotation, Quaternion.Euler(0, turn, 0) * rotation, deltaTime * getNavMeshAgent.angularSpeed);
                    getNavMeshAgent.Move(forward.normalized * direction.y * deltaTime * speed);
                    if (position != getTransform.position || turn != 0)
                    {
                        character.DoMoveAction(direction, speed > getNavMeshAgent.speed || (walk < 0 && dash));
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
            getNavMeshAgent.speed = Mathf.Clamp(stat.walkSpeed, Stat.MinWalkSpeed, Stat.MaxWalkSpeed);
            _dashSpeed = Mathf.Clamp(stat.dashSpeed, Stat.MinDashSpeed, Stat.MaxDashSpeed);
            _dashCost = Mathf.Clamp(stat.dashCost, Stat.MinDashCost, Stat.MaxDashCost);
            _reverseRate = Mathf.Clamp(stat.reverseRate, Stat.MinReverseRate, Stat.MaxReverseRate);
            getNavMeshAgent.stoppingDistance = Mathf.Clamp(stat.stoppingDistance, Stat.MinStoppingDistance, Stat.MaxStoppingDistance);
            character.Set(stat.attackSpeed);
            _attackDamage = stat.attackDamage;
            _fullLife = stat.fullLife;
            _staminaAction?.Invoke(_currentStamina, _fullStamina);
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
        _staminaAction?.Invoke(_currentStamina, _fullStamina);
        _currentLife = _fullLife;
        _lifeAction?.Invoke(_currentLife, _fullLife, this);
        character.DoReviveAction();
    }

    public void Initialize(Action<float, float> staminaAction, Action<uint, uint, Controller> lifeAction)
    {
        _staminaAction = staminaAction;
        _lifeAction = lifeAction;
    }
}