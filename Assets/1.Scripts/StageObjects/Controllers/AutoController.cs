using System.Collections;

/// <summary>
/// Ư�� �÷��̾� ��ü�� ������ �� �ִ� �ڵ� ��Ʈ�ѷ� Ŭ���� 
/// </summary>
public class AutoController : Controller
{
    protected override IEnumerator DoProcess()
    {
        while(true)
        {

            yield return null;
        }
    }
}
