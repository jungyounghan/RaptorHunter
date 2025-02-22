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
    private Transform _target = null;

    [SerializeField]
    private List<AudioClip> _walkAudioClips = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> _runAudioClips = new List<AudioClip>();

    private static readonly int MoveHashIndex = Animator.StringToHash("Move");
    private static readonly int TurnHashIndex = Animator.StringToHash("Turn");
    private static readonly float LinearInterpolation = 10;

    public const bool Hunter = true;
    public const bool Raptor = false;

    private void PlaySoundWalk()
    {
        int count = _walkAudioClips.Count;
        if (count > 0)
        {
            int index = Random.Range(0, count);
            PlaySound(_walkAudioClips[index]);
        }
    }

    private void PlaySoundRun()
    {
        int count = _runAudioClips.Count;
        if (count > 0)
        {
            int index = Random.Range(0, count);
            PlaySound(_runAudioClips[index]);
        }
    }

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
        getAnimator.Play(0, 0, 0);
    }

    public abstract void DoJumpAction();

    public abstract void DoLandAction();


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

    public void DoMoveAction(Vector2 direction, bool dash)
    {
        if (dash == true)
        {
            direction *= 2;
        }
        getAnimator.SetFloat(TurnHashIndex, direction.x);
        getAnimator.SetFloat(MoveHashIndex, direction.y);
    }

    public abstract void Set(float attackSpeed);

    public abstract void Recharge();

    public abstract bool IsHuman();
}