using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 조종 가능한 추상 캐릭터 클래스
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public abstract class Character : MonoBehaviour
{
    private bool _hasAnimator = false;

    private Animator _animator = null;

    protected Animator getAnimator {
        get
        {
            if (_hasAnimator == false)
            {
                _hasAnimator = TryGetComponent(out _animator);
            }
            return _animator;
        }
    }

    private bool _hasAudioSource = false;

    private AudioSource _audioSource = null;

    private AudioSource getAudioSource
    {
        get
        {
            if(_hasAudioSource == false)
            {
                _hasAudioSource = TryGetComponent(out _audioSource);
            }
            return _audioSource;
        }
    }

    [SerializeField]
    protected Transform _target = null;

    [SerializeField]
    protected List<AudioClip> audioClips = new List<AudioClip>();

    private static readonly int ExitHashIndex = Animator.StringToHash("Exit");
    private static readonly float LinearInterpolation = 10;

    protected void PlaySound(AudioClip audioClip)
    {
        getAudioSource.clip = audioClip;
        getAudioSource.Play();
    }

    public virtual void LookAt(Vector3 position)
    {
        if (_target != null)
        {
            _target.position = Vector3.Lerp(_target.position, position, Time.deltaTime * LinearInterpolation);
        }
    }

    public virtual void DoReviveAction()
    {
        getAnimator.SetTrigger(ExitHashIndex);
    }

    public abstract void DoJumpAction();

    public abstract void DoLandAction();

    public abstract void DoStopAction();

    public abstract void DoHitAction(bool dead);

    public abstract void DoAttackAction(uint damage);

    public abstract void DoMoveAction(Vector2 direction, bool dash);

    public abstract void Recharge();
}