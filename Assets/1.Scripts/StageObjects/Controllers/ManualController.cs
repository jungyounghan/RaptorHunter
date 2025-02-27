using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 입력 컨트롤러 클래스 
/// </summary>
public sealed class ManualController : Controller
{
    [Serializable]
    public struct Damage
    {
        public uint power;
        public float duration;
    }

    private Damage _damage;

    private float _invincible = 0;

    private static readonly string VerticalKey = "Vertical";
    private static readonly string HorizontalKey = "Horizontal";
    private static readonly string DashKey = "Dash";
    private static readonly string AttackKey = "Fire1";

    private static readonly float AimDistance = 3;

    private Action<float, float> _staminaAction = null;
    private Action<uint> _damageAction = null;

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
                    character.DoAttackAction(_attackDamage + _damage.power);
                }
                if (landing == true)
                {
                    character.Recharge();
                }
                if (_damage.duration > 0)
                {
                    _damage.duration -= deltaTime;
                    if(_damage.duration <= 0)
                    {
                        _damage.duration = 0;
                        _damage.power = 0;
                    }
                }
                if (_invincible > 0)
                {
                    _invincible -= deltaTime;
                    if(_invincible <= 0)
                    {
                        _invincible = 0;
                        //이펙트 효과 사라지게
                    }
                }
            }
            else
            {
                if(_damage.duration > 0)
                {
                    _damage.duration = 0;
                }
                if (_invincible > 0)
                {
                    _invincible = 0;
                }
            }
        }
    }

    public override void Hit(Vector3 origin, Vector3 direction, uint damage)
    {
        if (_invincible == 0)
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
        _staminaAction?.Invoke(_currentStamina, _fullStamina);
        _currentLife = _fullLife;
        _lifeAction?.Invoke(_currentLife, _fullLife, this);
        _damageAction?.Invoke(_attackDamage + _damage.power);
        character.DoReviveAction();
    }

    public void Initialize(Action<float, float> staminaAction, Action<uint, uint, Controller> lifeAction, Action<uint> damageAction)
    {
        _staminaAction = staminaAction;
        _lifeAction = lifeAction;
        _damageAction = damageAction;
    }

    public void Take(Damage damage, float invincible, byte heal)
    {
        _damage = damage;
        if(_damage.power > 0 && _damage.duration > 0)
        {
            _damageAction?.Invoke(_attackDamage + _damage.power);
        }
        _invincible = Mathf.Clamp(invincible, Item.MinInvincibleValue, Item.MaxInvincibleValue);
        float value = Mathf.Clamp(heal, Item.MinHealValue, Item.MaxHealValue) * 0.01f;
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
    }
}