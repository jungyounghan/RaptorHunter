using System.Collections;
using UnityEngine;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 자동 컨트롤러 클래스 
/// </summary>
public sealed class AutoController : Controller
{
    private float _defaultSpeed = 0;

    private enum State
    {

    }

    public Transform target;

    protected override void OnEnable()
    {
        _defaultSpeed = getNavMeshAgent.speed;
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        getNavMeshAgent.speed = _defaultSpeed;
        base.OnDisable();
    }

    protected override IEnumerator DoProcess()
    {
        while(true)
        {
            if(alive == true)
            {
                getNavMeshAgent.enabled = IsGrounded();
                if (target != null && Vector3.Distance(target.position, getTransform.position) <= getNavMeshAgent.stoppingDistance)
                {
                    //getCharacter.LookAt();
                }

            }
            yield return null;
        }
    }
}