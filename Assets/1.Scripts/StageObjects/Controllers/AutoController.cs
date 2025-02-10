using System.Collections;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 자동 컨트롤러 클래스 
/// </summary>
public sealed class AutoController : Controller
{
    protected override IEnumerator DoProcess()
    {
        while(true)
        {
            // if (agent.isOnOffMeshLink) // 점프 구간에 진입하면
            //{
            //    StartCoroutine(JumpAcross());
            //}
            yield return null;
        }
    }
}
