using System.Collections;

/// <summary>
/// Ư�� �÷��̾� ��ü�� ������ �� �ִ� �ڵ� ��Ʈ�ѷ� Ŭ���� 
/// </summary>
public sealed class AutoController : Controller
{
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
