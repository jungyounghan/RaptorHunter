using UnityEngine;

/// <summary>
/// Á¶Á¾ °¡´ÉÇÑ »ç³É²Û Ä³¸¯ÅÍ Å¬·¡½º
/// </summary>
public sealed class HunterCharacter : Character
{
    private readonly int _moveHashIndex = Animator.StringToHash("Move");
    private readonly int _turnHashIndex = Animator.StringToHash("Turn");

    [SerializeField]
    private Transform _gunGrip = null;      //ÃÑ ¼ÕÀâÀÌ
    [SerializeField]
    private Transform _gunMagazine = null;  //ÃÑ ÅºÃ¢  
    [SerializeField]
    private Transform _laserPoint = null;
    [SerializeField]
    private LineRenderer _lineRenderer = null;

    [SerializeField]
    private Vector3 test;

    private void OnAnimatorIK(int layerIndex)
    {
        getAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        getAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        if(_gunMagazine != null)
        {
            getAnimator.SetIKPosition(AvatarIKGoal.LeftHand, _gunMagazine.position);
            getAnimator.SetIKRotation(AvatarIKGoal.LeftHand, _gunMagazine.rotation);
        }
        getAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        getAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        if (_gunGrip != null)
        {
            getAnimator.SetIKPosition(AvatarIKGoal.RightHand, _gunGrip.position);
            getAnimator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.Euler(_gunGrip.rotation.eulerAngles + test));
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