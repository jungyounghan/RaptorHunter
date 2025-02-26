using UnityEngine;

/// <summary>
/// 메인 메뉴 클래스
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _popupObject = null;

    [Header("게임 시작"), SerializeField]
    private GameObject _playObject = null;

    [Header("랭킹")]
    [SerializeField]
    private GameObject _rankingObject = null;
    [SerializeField]
    private Transform _rankingContent = null;
    [SerializeField]
    private Ranking _rankingPrefab = null;

    [Header("음향 효과"), SerializeField]
    private AudioPanel _audioPanel = null;

    private enum Select
    {
        Play,
        Ranking,
        Audio,
    }

    private void Awake()
    {
        Ranking.Info[] infos = GameData.Load();
        if(infos != null && _rankingContent != null && _rankingPrefab != null)
        {
            foreach(Ranking.Info info in infos)
            {
                Ranking ranking = Instantiate(_rankingPrefab, _rankingContent);
                ranking.Set(info.character, info.enemy, info.killCount, info.survivalTime);
            }
        }
    }

    private void Show(Select select)
    {
        if (_popupObject != null)
        {
            _popupObject.SetActive(true);
        }
        if (_playObject != null)
        {
            _playObject.SetActive(select == Select.Play);
        }
        if (_rankingObject != null)
        {
            _rankingObject.SetActive(select == Select.Ranking);
        }
        _audioPanel?.SetActive(select == Select.Audio);
    }

    public void SelectPlayer(bool human)
    {

    }

    public void SelectEnemy(int index)
    {

    }

    public void ReturnPrevious()
    {
        if (_popupObject != null)
        {
            _popupObject.SetActive(false);
        }
    }

    public void ShowPlay()
    {
        Show(Select.Play);
    }

    public void ShowRanking()
    {
        Show(Select.Ranking);
    }

    public void ShowAudio()
    {
        Show(Select.Audio);
    }

    public void Quit()
    {
        Application.Quit();
    }
}