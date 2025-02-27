using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 입력 컨트롤러 클래스 
/// </summary>
public sealed class ManualController : Controller
{
    [Header("공격력 증가 시간"), SerializeField]
    private float _attackDamageTime = 0;
    [Header("공격 속도 증가 시간"), SerializeField]
    private float _attackSpeedTime = 0;
    [Header("무적 지속시간"), SerializeField]
    private float _invincibleTime = 0;

    private Action<uint> _attackDamageAction = null;
    private Action<float> _attackSpeedAction = null;
    private Action<float, float> _staminaAction = null;

    private static readonly string VerticalKey = "Vertical";
    private static readonly string HorizontalKey = "Horizontal";
    private static readonly string DashKey = "Dash";
    private static readonly string AttackKey = "Fire1";
    private static readonly uint DoublePoint = 2;
    private static readonly float AimDistance = 3;
    private static readonly float BasisPoint = 0.01f;

    private void Update()
    {
        if (Time.timeScale > 0)
        {
            float deltaTime = Time.deltaTime;
            if (_currentStamina < _fullStamina)
            {
                _currentStamina += deltaTime * _recoverStamina;
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
                    RaycastHit[] raycastHits = Physics.RaycastAll(ray);
                    bool done = false;
                    foreach (RaycastHit raycastHit in raycastHits)
                    {
                        if (Vector3.Dot(forward, raycastHit.point - position) > AimDistance)
                        {
                            done = true;
                            character.LookAt(raycastHit.point);
                            break;
                        }
                    }
                    if (done == false)
                    {
                        Plane plane = new Plane(forward, position + (forward * AimDistance));
                        plane.Raycast(ray, out float distance);
                        character.LookAt(ray.GetPoint(distance));
                    }
                }
                bool attack = Input.GetButton(AttackKey);
                if (attack == true)
                {
                    character.DoAttackAction(_attackDamageTime > 0 ? _attackDamage * DoublePoint : _attackDamage);
                }
                if (landing == true)
                {
                    character.Recharge();
                }
                if (_attackDamageTime > 0)
                {
                    _attackDamageTime -= deltaTime;
                    if (_attackDamageTime <= 0)
                    {
                        _attackDamageTime = 0;
                        _attackDamageAction?.Invoke(_attackDamage);
                    }
                }
                if (_attackSpeedTime > 0)
                {
                    _attackSpeedTime -= deltaTime;
                    if (_attackSpeedTime <= 0)
                    {
                        _attackSpeedTime = 0;
                        character.SetAttackSpeed(character.GetAttackSpeed() / DoublePoint);
                        _attackSpeedAction?.Invoke(character.GetAttackSpeed());
                    }
                }
                if (_invincibleTime > 0)
                {
                    _invincibleTime -= deltaTime;
                    if(_invincibleTime <= 0)
                    {
                        _invincibleTime = 0;
                    }
                }
            }
            else
            {
                if (_attackDamageTime > 0)
                {
                    _attackDamageTime = 0;
                    _attackDamageAction?.Invoke(_attackDamage);
                }
                if (_attackSpeedTime > 0)
                {
                    _attackSpeedTime = 0;
                    character.SetAttackSpeed(character.GetAttackSpeed() / DoublePoint);
                    _attackSpeedAction?.Invoke(character.GetAttackSpeed());
                }
                if (_invincibleTime > 0)
                {
                    _invincibleTime = 0;
                }
            }
        }
    }

    public override void Hit(Vector3 origin, Vector3 direction, uint damage)
    {
        if (_invincibleTime == 0)
        {
            base.Hit(origin, direction, damage);
        }
    }

    protected override IEnumerator DoProcess()
    {
        while (true)
        {
            if (alive == true)
            {
                getNavMeshAgent.enabled = landing;
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
                    getTransform.rotation = Quaternion.Slerp(rotation, Quaternion.Euler(0, turn * speed, 0) * rotation, deltaTime * RotationSpeed);
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
            getNavMeshAgent.speed = Mathf.Clamp(stat.walkSpeed, Stat.MinWalkSpeed, Stat.MaxWalkSpeed);
            _dashSpeed = Mathf.Clamp(stat.dashSpeed, Stat.MinDashSpeed, Stat.MaxDashSpeed);
            _dashCost = Mathf.Clamp(stat.dashCost, Stat.MinDashCost, Stat.MaxDashCost);
            _reverseRate = Mathf.Clamp(stat.reverseRate, Stat.MinReverseRate, Stat.MaxReverseRate);
            getNavMeshAgent.stoppingDistance = Mathf.Clamp(stat.stoppingDistance, Stat.MinStoppingDistance, Stat.MaxStoppingDistance);
            character.SetAttackSpeed(stat.attackSpeed);
            _attackDamage = stat.attackDamage;
            _fullLife = stat.fullLife;
        }
    }

    public override void Revive()
    {
        getCollider.enabled = true;
        getNavMeshAgent.enabled = false;
        _currentStamina = _fullStamina;
        _staminaAction?.Invoke(_currentStamina, _fullStamina);
        _currentLife = _fullLife;
        _lifeAction?.Invoke(_currentLife, _fullLife, this);
        _attackDamageAction?.Invoke(_attackDamage);
        _attackSpeedAction?.Invoke(character.GetAttackSpeed());
        character.DoReviveAction();
    }

    public void Initialize(Action<float, float> staminaAction, Action<uint, uint, Controller> lifeAction, Action<uint> attackDamageAction, Action<float> attackSpeedAction)
    {
        _staminaAction = staminaAction;
        _lifeAction = lifeAction;
        _attackDamageAction = attackDamageAction;
        _attackSpeedAction = attackSpeedAction;
    }

    public void Heal(float attackDamage, float attackSpeed, float invincible, byte life, byte stamina)
    {
        if(attackDamage > 0)
        {
            _attackDamageTime += attackDamage;
            _attackDamageAction?.Invoke(_attackDamage * DoublePoint);
        }
        if(attackSpeed > 0)
        {
            if(_attackSpeedTime == 0)
            {
                character.SetAttackSpeed(character.GetAttackSpeed() * DoublePoint);
            }
            _attackSpeedTime += attackSpeed;
            _attackSpeedAction?.Invoke(character.GetAttackSpeed());
        }
        _invincibleTime += invincible;
        float value = Mathf.Clamp(life, Item.MinHealValue, Item.MaxHealValue) * BasisPoint;
        uint amount = (uint)(_fullLife * value);
        if (amount > 0)
        {
            if (_currentLife + amount >= _fullLife)
            {
                _currentLife = _fullLife;
            }
            else
            {
                _currentLife += amount;
            }
            _lifeAction?.Invoke(_currentLife, _fullLife, this);
        }
        value = Mathf.Clamp(stamina, Item.MinHealValue, Item.MaxHealValue) * BasisPoint * _fullStamina;
        if(value > 0)
        {
            if(_currentStamina + value >= _fullStamina)
            {
                _currentStamina = _fullStamina;
            }
            else
            {
                _currentStamina += value;
            }
            _staminaAction?.Invoke(_currentStamina, _fullStamina);
        }
    }
}