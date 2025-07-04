using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
/// <summary>
/// 충돌하거나 공격을 받으면 소리를 내고 파괴되면 임의의 확률로 아이템을 드롭하기도 함
/// </summary>
public sealed class Obstacle : MonoBehaviour, IHittable
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

    private bool _hasRigidbody = false;

    private Rigidbody _rigidbody = null;

    private Rigidbody getRigidbody {
        get
        {
            if (_hasRigidbody == false)
            {
                _hasRigidbody = TryGetComponent(out _rigidbody);
            }
            return _rigidbody;
        }
    }

    private bool _hasAudioSource = false;

    private AudioSource _audioSource = null;

    private AudioSource getAudioSource {
        get
        {
            if (_hasAudioSource == false)
            {
                _hasAudioSource = TryGetComponent(out _audioSource);
            }
            return _audioSource;
        }
    }

    [SerializeField]
    private List<AudioClip> _audioClips = new List<AudioClip>();

    [Header("민감도"), SerializeField, Range(1, int.MaxValue)]
    private float _sensitivity = 1;
    [Header("최대 체력"), SerializeField]
    private uint _fullLife = 0;
    [Header("현재 체력"), SerializeField]
    private uint _currentLife = 0;

    [SerializeField]
    private List<Item> _items = new List<Item>();

    private Action<Item, Vector3> _action = null;

    private static readonly float SoundValue = 6;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_currentLife > _fullLife)
        {
            _currentLife = _fullLife;
        }
    }
#endif

    private void Awake()
    {
        _currentLife = _fullLife;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(getRigidbody.velocity.magnitude >= SoundValue && (_currentLife > 0 || _currentLife == _fullLife))
        {
            PlaySound();
        }
    }

    private void PlaySound()
    {
        int count = _audioClips.Count;
        if (count > 0)
        {
            int index = UnityEngine.Random.Range(0, count);
            getAudioSource.clip = _audioClips[index];
            getAudioSource.Play();
        }
    }

    public void Initialize(Action<Item, Vector3> action)
    {
        _action = action;
    }

    public void Hit(Vector3 origin, Vector3 direction, uint force)
    {
        if (_currentLife > 0 || _currentLife == _fullLife)
        {
            getRigidbody.AddForce(direction.normalized * force * _sensitivity, ForceMode.Impulse);
            if (_fullLife > 0)
            {
                if (force >= _currentLife)
                {
                    _currentLife = 0;
                    int count = _items.Count;
                    if (count > 0)
                    {
                        _action?.Invoke(_items[UnityEngine.Random.Range(0, count)], getTransform.position);
                    }
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    _currentLife -= force;
                }
            }
            PlaySound();
        }
    }
}