using UnityEngine;

/// <summary>
/// ���� �޴� Ŭ����
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _popupObject = null;

    [Header("���� ����"), SerializeField]
    private GameObject _playObject = null;

    [SerializeField]
    private SceneLoader _sceneLoader = null;

    [Header("��ŷ"), SerializeField]
    private Ranking _ranking = null;

    [Header("���� ȿ��"), SerializeField]
    private AudioPanel _audioPanel = null;

    private enum Select
    {
        Play,
        Ranking,
        Audio,
    }

    private void Awake()
    {
        //_ranking?.SetScore(GameData.Load());
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
        if(select == Select.Play)
        {

        }
        _ranking?.SetActive(select == Select.Ranking);
        if(select == Select.Audio)
        {
            _audioPanel?.Open();
        }
        else
        {
            _audioPanel?.Close();
        }
    }

    public void SelectPlayer(bool human)
    {

    }

    public void SelectEnemy(int index)
    {

    }

    public void LoadScene(string name)
    {
        if (_sceneLoader != null)
        {
            SceneLoader sceneLoader = Instantiate(_sceneLoader, transform);
            sceneLoader.Load(name);
        }
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