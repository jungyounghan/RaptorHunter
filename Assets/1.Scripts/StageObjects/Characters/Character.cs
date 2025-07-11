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

    [SerializeField]
    private AudioSource _voiceAudioSource = null;
    [SerializeField]
    private AudioSource _stepAudioSource = null;

    [SerializeField]
    private List<AudioClip> _walkAudioClips = new List<AudioClip>();
    [SerializeField]
    private List<AudioClip> _runAudioClips = new List<AudioClip>();
    [SerializeField]
    private List<AudioClip> _hitAudioClips = new List<AudioClip>();
    [SerializeField]
    private List<AudioClip> _deadAudioClips = new List<AudioClip>();

    [SerializeField]
    private Transform _target = null;

    private static readonly int MoveHashIndex = Animator.StringToHash("Move");
    private static readonly int TurnHashIndex = Animator.StringToHash("Turn");
    private static readonly int JumpHashIndex = Animator.StringToHash("Jump");
    private static readonly int DieHashIndex = Animator.StringToHash("Die");

    private static readonly float LinearInterpolation = 10;

    public const bool Hunter = true;
    public const bool Raptor = false;

    private void PlaySoundWalk()
    {
        int count = _walkAudioClips.Count;
        if (count > 0)
        {
            int index = Random.Range(0, count);
            PlayStepSound(_walkAudioClips[index]);
        }
    }

    private void PlaySoundRun()
    {
        int count = _runAudioClips.Count;
        if (count > 0)
        {
            int index = Random.Range(0, count);
            PlayStepSound(_runAudioClips[index]);
        }
    }

    protected void PlayStepSound(AudioClip audioClip)
    {
        if(_stepAudioSource != null)
        {
            _stepAudioSource.clip = audioClip;
            _stepAudioSource.Play();
        }
    }

    protected void PlayVoiceSound(AudioClip audioClip)
    {
        if (_voiceAudioSource != null)
        {
            _voiceAudioSource.clip = audioClip;
            _voiceAudioSource.Play();
        }
    }

    protected void PlaySoundHit()
    {
        int count = _hitAudioClips.Count;
        if (count > 0)
        {
            int index = Random.Range(0, count);
            PlayVoiceSound(_hitAudioClips[index]);
        }
    }

    protected void DoDeadAction()
    {
        int count = _deadAudioClips.Count;
        if (count > 0)
        {
            int index = Random.Range(0, count);
            PlayVoiceSound(_deadAudioClips[index]);
        }
        getAnimator.SetBool(DieHashIndex, true);
    }

    public void DoMoveAction(Vector2 direction, bool dash)
    {
        if (dash == true)
        {
            direction *= 2;
        }
        getAnimator.SetFloat(TurnHashIndex, direction.x);
        getAnimator.SetFloat(MoveHashIndex, direction.y);
    }

    public virtual void LookAt(Vector3 position)
    {
        if (_target != null)
        {
            _target.position = Vector3.Lerp(_target.position, position, Time.deltaTime * LinearInterpolation);
        }
    }

    public virtual void DoJumpAction()
    {
        getAnimator.SetBool(JumpHashIndex, true);
    }

    public virtual void DoLandAction()
    {
        getAnimator.SetBool(JumpHashIndex, false);
    }

    public virtual void DoReviveAction()
    {
        getAnimator.SetBool(DieHashIndex, false);
    }

    public void DoStopAction()
    {
        float deltaTime = Time.deltaTime;
        float turn = getAnimator.GetFloat(TurnHashIndex);
        if (turn != 0)
        {
            if (turn > 0)
            {
                turn -= deltaTime;
                if (turn < 0)
                {
                    turn = 0;
                }
            }
            else
            {
                turn += deltaTime;
                if (turn > 0)
                {
                    turn = 0;
                }
            }
            getAnimator.SetFloat(TurnHashIndex, turn);
        }
        float move = getAnimator.GetFloat(MoveHashIndex);
        if (move != 0)
        {
            if (move > 0)
            {
                move -= deltaTime;
                if (move < 0)
                {
                    move = 0;
                }
            }
            else
            {
                move += deltaTime;
                if (move > 0)
                {
                    move = 0;
                }
            }
            getAnimator.SetFloat(MoveHashIndex, move);
        }
    }

    public abstract void DoHitAction(bool dead);

    public abstract void DoAttackAction(uint damage);

    public abstract void SetAttackSpeed(float value);

    public abstract void Recharge();

    public abstract bool IsHuman();

    public abstract float GetAttackSpeed();

    public abstract Transform GetWeaponTransform();
}