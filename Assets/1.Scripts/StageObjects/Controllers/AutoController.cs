using System.Collections;

/// <summary>
/// Ư�� �÷��̾� ��ü�� ������ �� �ִ� �ڵ� ��Ʈ�ѷ� Ŭ���� 
/// </summary>
public sealed class AutoController : Controller
{
    private float _defaultSpeed = 0;

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
            // if (agent.isOnOffMeshLink) // ���� ������ �����ϸ�
            //{
            //    StartCoroutine(JumpAcross());
            //}
            yield return null;
        }
    }
}
