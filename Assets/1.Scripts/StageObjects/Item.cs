using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public sealed class Item : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform
    {
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

    [SerializeField]
    private Vector3 _speed;

    [SerializeField]
    private MeshRenderer _meshRenderer = null;

    [SerializeField]
    private ManualController.Damage _damage;

    public const float MinInvincibleValue = 0;
    public const float MaxInvincibleValue = 10;

    [SerializeField, Range(MinInvincibleValue, MaxInvincibleValue)]
    private float _invincible = 0;

    public const int MinHealValue = 0;
    public const int MaxHealValue = 100;

    [SerializeField, Range(MinHealValue, MaxHealValue)]
    private byte _healPercent = 10;

    [SerializeField]
    private ParticleSystem _particleSystem;

    private bool _acquisition = false;

    private void OnEnable()
    {
        _acquisition = false;
        if(_meshRenderer != null)
        {
            _meshRenderer.enabled = true;
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

    private void Update()
    {
        if (Time.timeScale > 0)
        {
            float deltaTime = Time.deltaTime;
            getTransform.Rotate(_speed.x * deltaTime, _speed.y * deltaTime, _speed.z * deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_acquisition == false)
        {
            Transform transform = other.transform;
            if (transform.tag == "Player")
            {
                ManualController manualController = transform.GetComponent<ManualController>();
                if (manualController != null)
                {
                    manualController.Take(_damage, _invincible, _healPercent);
                    if (_meshRenderer != null)
                    {
                        _meshRenderer.enabled = false;
                    }
                    StartCoroutine(DoHideAction());
                    IEnumerator DoHideAction()
                    {
                        _acquisition = true;
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