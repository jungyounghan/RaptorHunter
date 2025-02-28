using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 클래스
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public sealed class Item : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform {
        get
        {
            if (_hasTransform == false)
            {
                _hasTransform = true;
                _transform = transform;
            }
            return _transform;
        }
    }

    public const float MinAttackDamage = 0;
    public const float MaxAttackDamage = 20;

    [Header("공격력 증가 시간"), SerializeField, Range(MinAttackDamage, MaxAttackDamage)]
    private float _attackDamage = 0;

    public const float MinAttackSpeed = 0;
    public const float MaxAttackSpeed = 20;

    [Header("공격 속도 증가 시간"), SerializeField, Range(MinAttackSpeed, MaxAttackSpeed)]
    private float _attackSpeed = 0;

    public const float MinInvincibleValue = 0;
    public const float MaxInvincibleValue = 10;

    [SerializeField, Range(MinInvincibleValue, MaxInvincibleValue)]
    private float _invincible = 0;

    public const int MinHealValue = 0;
    public const int MaxHealValue = 100;

    [SerializeField, Range(MinHealValue, MaxHealValue)]
    private byte _lifePercent = 10;

    [SerializeField, Range(MinHealValue, MaxHealValue)]
    private byte _staminaPercent = 10;

    [SerializeField]
    private List<GameObject> _effectObjects = new List<GameObject>();

    [SerializeField]
    private ParticleSystem _particleSystem;

    private bool _acquisition = false;

    public Vector3 position {
        get
        {
            return getTransform.position;
        }
        set
        {
            getTransform.position = value;
        }
    }

    private void OnEnable()
    {
        _acquisition = false;
        foreach(GameObject effectObject in _effectObjects)
        {
            if (effectObject != null)
            {
                effectObject.SetActive(true);
            }
        }
        if(_particleSystem != null)
        {
            _particleSystem.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_acquisition == false)
        {
            Transform transform = other.transform;
            if (transform.tag == Controller.PlayerTag)
            {
                ManualController manualController = transform.GetComponent<ManualController>();
                if (manualController != null)
                {
                    manualController.Heal(_attackDamage, _attackSpeed, _invincible, _lifePercent, _staminaPercent);
                    _acquisition = true;
                    foreach (GameObject effectObject in _effectObjects)
                    {
                        if (effectObject != null)
                        {
                            effectObject.SetActive(false);
                        }
                    }
                    StartCoroutine(ShowParticleAndHide());
                    IEnumerator ShowParticleAndHide()
                    {
                        if (_particleSystem != null)
                        {
                            _particleSystem.gameObject.SetActive(true);
                            yield return new WaitWhile(() => _particleSystem != null && _particleSystem.isStopped == false);
                        }
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}