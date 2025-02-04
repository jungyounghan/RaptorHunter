using System.Collections;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 자동 컨트롤러 클래스 
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
