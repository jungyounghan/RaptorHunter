using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// ���� ������ ��ɲ� ĳ���� Ŭ����
/// </summary>
public sealed class HunterCharacter : Character
{
    private readonly int MoveHashIndex = Animator.StringToHash("Move");
    private readonly int TurnHashIndex = Animator.StringToHash("Turn");
    private readonly int JumpHashIndex = Animator.StringToHash("Jump");
    private readonly int HitHashIndex = Animator.StringToHash("Hit");
    private readonly int DieHashIndex = Animator.StringToHash("Die");

    [Header("������ Ʈ������")]
    [SerializeField]
    private Transform _gunMagazine = null;  //źâ

    [Header("������")]
    [SerializeField]
    private Transform _laserPoint = null;   //������ ������ ��
    [SerializeField]
    private LineRenderer _laserLine = null; //������ ��

    [Header("�ѱ�")]
    [SerializeField]
    private Transform _muzzlePoint = null;  //�Ѿ� ������ ��
    [SerializeField]
    private LineRenderer _muzzleLine = null;//�Ѿ� ���� ������

    [Header("ȿ����")]
    [SerializeField]
    private AudioSource _shotAudio = null;  //�� �Ҹ�

    [Header("���� ����")]
    [SerializeField]
    private MultiAimConstraint _spineConstraint = null;
    [SerializeField]
    private MultiAimConstraint _headConstraint = null;

    private void OnAnimatorIK(int layerIndex)
    {
        getAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        getAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        if (_gunMagazine != null)
        {
            getAnimator.SetIKPosition(AvatarIKGoal.LeftHand, _gunMagazine.position);
            getAnimator.SetIKRotation(AvatarIKGoal.LeftHand, _gunMagazine.rotation);
        }
    }

    public override void LookAt(Vector3 position)
    {
        base.LookAt(position);
        if (_laserPoint != null && _laserLine != null)
        {
            if (Physics.Raycast(_laserPoint.position - _laserPoint.forward, _laserPoint.forward, out RaycastHit hit, Mathf.Infinity))
            {
                _laserLine.positionCount = 2;
                _laserLine.SetPosition(0, _laserPoint.position);
                _laserLine.SetPosition(1, hit.point);
            }
            else
            {
                _laserLine.positionCount = 0;
            }
        }
    }

    public override void DoReviveAction()
    {
        base.DoReviveAction();
        if (_spineConstraint != null)
        {
            _spineConstraint.weight = 1;
        }
        if (_headConstraint != null)
        {
            _headConstraint.weight = 1;
        }
    }

    public override void DoJumpAction()
    {
        getAnimator.SetBool(JumpHashIndex, true);
    }

    public override void DoLandAction()
    {
        getAnimator.SetBool(JumpHashIndex, false);
    }

    public override void DoStopAction()
    {
        float deltaTime = Time.deltaTime;
        float turn = getAnimator.GetFloat(TurnHashIndex);
        if (turn != 0)
        {
            if(turn > 0)
            {
                turn -= deltaTime;
                if(turn < 0)
                {
                    turn = 0;
                }
            }
            else
            {
                turn += deltaTime;
                if(turn > 0)
                {
                    turn = 0;
                }
            }
            getAnimator.SetFloat(TurnHashIndex, turn);
        }
        float move = getAnimator.GetFloat(MoveHashIndex);
        if(move != 0)
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

    public override void DoHitAction(bool dead)
    {
        if (dead == false)
        {
            getAnimator.SetTrigger(HitHashIndex);
        }
        else
        {
            getAnimator.SetTrigger(DieHashIndex);
            if(_spineConstraint != null)
            {
                _spineConstraint.weight = 0;
            }
            if (_headConstraint != null)
            {
                _headConstraint.weight = 0;
            }
        }
    }

    public override void DoMoveAction(Vector2 direction, float speed, bool dash)
    {
        //Debug.Log(speed);
        if(dash == true)
        {
            direction *= 2;
        }
        getAnimator.SetFloat(TurnHashIndex, direction.x);
        getAnimator.SetFloat(MoveHashIndex, direction.y);
    }
}