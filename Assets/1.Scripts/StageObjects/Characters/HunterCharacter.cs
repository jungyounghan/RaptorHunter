using UnityEngine;

/// <summary>
/// 조종 가능한 사냥꾼 캐릭터 클래스
/// </summary>
public sealed class HunterCharacter : Character
{
    private readonly int _moveHashIndex = Animator.StringToHash("Move");
    private readonly int _turnHashIndex = Animator.StringToHash("Turn");

    [SerializeField]
    private Transform _gunPivot = null;
    [SerializeField]
    private Transform _laserPoint = null;
    [SerializeField]
    private LineRenderer _lineRenderer = null;

    private void OnAnimatorIK(int layerIndex)
    {
        if(_gunPivot != null)
        {
            _gunPivot.position = getAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);
            getAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            getAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        }
    }

    public override void LookAt(Vector3 position)
    {
        base.LookAt(position);
        if (_laserPoint != null && _lineRenderer != null)
        {
            if (Physics.Raycast(_laserPoint.position - _laserPoint.forward, _laserPoint.forward, out RaycastHit hit, Mathf.Infinity))
            {
                _lineRenderer.positionCount = 2;
                _lineRenderer.SetPosition(0, _laserPoint.position);
                _lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                _lineRenderer.positionCount = 0;
            }
        }
    }

    public override void DoJumpAction()
    {

    }

    public override void DoLandAction()
    {

    }

    public override void DoStopAction()
    {
        float deltaTime = Time.deltaTime;
        float turn = getAnimator.GetFloat(_turnHashIndex);
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
            getAnimator.SetFloat(_turnHashIndex, turn);
        }
        float move = getAnimator.GetFloat(_moveHashIndex);
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
            getAnimator.SetFloat(_moveHashIndex, move);
        }
    }

    public override void DoMoveAction(Vector2 direction, float speed, bool dash)
    {
        if(dash == true)
        {
            direction *= 2;
        }
        getAnimator.SetFloat(_turnHashIndex, direction.x);
        getAnimator.SetFloat(_moveHashIndex, direction.y);
    }

    public override void DoHitAction(bool dead)
    {
        throw new System.NotImplementedException();
    }
}