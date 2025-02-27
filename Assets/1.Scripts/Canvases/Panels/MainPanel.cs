using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// ���� ȭ���� ����ϴ� �г� Ŭ����
/// </summary>
public sealed class MainPanel : Panel
{
    [Serializable]
    private struct PlayBundle
    {
        [Header("�÷��̾� ����"), SerializeField]
        private TMP_Text allyText;
        [Header("����� �� ����"), SerializeField]
        private TMP_Text enemyText;

        [Header("�÷��̾� ��ɲ� ��ư"), SerializeField]
        private Button allyHunterButton;
        [Header("�÷��̾� ���� ��ư"), SerializeField]
        private Button allyRaptorButton;

        [Header("����� �� ��ɲ� ��ư"), SerializeField]
        private Button enemyHunterButton;
        [Header("����� �� ���� ��ư"), SerializeField]
        private Button enemyRaptorButton;
        [Header("����� �� ȥ�� ��ư"), SerializeField]
        private Button enemyMixButton;

        [Header("���� ��ư"), SerializeField]
        private Button startButton;

        public void Select(bool ally)
        {
            GameData.ally = ally;
            EventSystem.current.SetSelectedGameObject(null);
            if(allyHunterButton != null && allyHunterButton.image != null)
            {
                allyHunterButton.image.color = ally == Character.Hunter ? allyHunterButton.colors.selectedColor : allyHunterButton.colors.normalColor;
            }
            if (allyRaptorButton != null && allyRaptorButton.image != null)
            {
                allyRaptorButton.image.color = ally == Character.Raptor ? allyRaptorButton.colors.selectedColor : allyRaptorButton.colors.normalColor;
            }
        }

        public void Select(GameData.Enemy enemy)
        {
            GameData.enemy = enemy;
            EventSystem.current.SetSelectedGameObject(null);
            if (enemyHunterButton != null && enemyHunterButton.image != null)
            {
                enemyHunterButton.image.color = enemy == GameData.Enemy.Hunter ? enemyHunterButton.colors.selectedColor : enemyHunterButton.colors.normalColor;
            }
            if (enemyRaptorButton != null && enemyRaptorButton.image != null)
            {
                enemyRaptorButton.image.color = enemy == GameData.Enemy.Raptor ? enemyRaptorButton.colors.selectedColor : enemyRaptorButton.colors.normalColor;
            }
            if (enemyMixButton != null && enemyMixButton.image != null)
            {
                enemyMixButton.image.color = enemy == GameData.Enemy.Mix ? enemyMixButton.colors.selectedColor : enemyMixButton.colors.normalColor;
            }
        }

        public void Set(bool value)
        {
            if (allyText != null)
            {
                allyText.gameObject.SetActive(value);
            }
            if (enemyText != null)
            {
                enemyText.gameObject.SetActive(value);
            }
            if (allyHunterButton != null)
            {
                if (value == true && allyHunterButton.image != null)
                {
                    ColorBlock colorBlock = allyHunterButton.colors;
                    allyHunterButton.image.color = GameData.ally == Character.Hunter ? colorBlock.selectedColor : colorBlock.normalColor;
                }
                allyHunterButton.gameObject.SetActive(value);
            }
            if (allyRaptorButton != null)
            {
                if (value == true && allyRaptorButton.image != null)
                {
                    ColorBlock colorBlock = allyRaptorButton.colors;
                    allyRaptorButton.image.color = GameData.ally == Character.Raptor ? colorBlock.selectedColor : colorBlock.normalColor;
                }
                allyRaptorButton.gameObject.SetActive(value);
            }
            if (enemyHunterButton != null)
            {
                if (value == true && enemyHunterButton.image != null)
                {
                    ColorBlock colorBlock = enemyHunterButton.colors;
                    enemyHunterButton.image.color = GameData.enemy == GameData.Enemy.Hunter ? colorBlock.selectedColor : colorBlock.normalColor;
                }
                enemyHunterButton.gameObject.SetActive(value);
            }
            if (enemyRaptorButton != null)
            {
                if (value == true && enemyRaptorButton.image != null)
                {
                    ColorBlock colorBlock = enemyRaptorButton.colors;
                    enemyRaptorButton.image.color = GameData.enemy == GameData.Enemy.Raptor ? colorBlock.selectedColor : colorBlock.normalColor;
                }
                enemyRaptorButton.gameObject.SetActive(value);
            }
            if (enemyMixButton != null)
            {
                if (value && enemyMixButton.image != null)
                {
                    ColorBlock colorBlock = enemyMixButton.colors;
                    enemyMixButton.image.color = GameData.enemy == GameData.Enemy.Mix ? colorBlock.selectedColor : colorBlock.normalColor;
                }
                enemyMixButton.gameObject.SetActive(value);
            }
            if (startButton != null)
            {
                startButton.gameObject.SetActive(value);
            }
        }
    }

    [Serializable]
    private struct RankingBundle
    {
        [Header("���� �ؽ�Ʈ"), SerializeField]
        private TMP_Text titleText;

        [Serializable]
        private struct Tag
        {
            private static string Hunter = "��ɲ�";
            private static string Raptor = "����";
            private static string Mix = "��ɲ۰� ����";
            private static string Hour = "�ð�";
            private static string Minute = "��";
            private static string Second = "��";
            private static readonly int HourSecond = 3600;
            private static readonly int MinuteSecond = 60;

            [SerializeField]
            private TMP_Text allyText;
            [SerializeField]
            private TMP_Text enemyText;
            [SerializeField]
            private TMP_Text killText;
            [SerializeField]
            private TMP_Text survivalText;

            public void Set()
            {
                if(allyText != null)
                {
                    allyText.text = null;
                }
                if (enemyText != null)
                {
                    enemyText.text = null;
                }
                if (killText != null)
                {
                    killText.text = null;
                }
                if (survivalText != null)
                {
                    survivalText.text = null;
                }
            }

            public void Set(bool value)
            {
                if (allyText != null)
                {
                    allyText.gameObject.SetActive(value);
                }
                if (enemyText != null)
                {
                    enemyText.gameObject.SetActive(value);
                }
                if (killText != null)
                {
                    killText.gameObject.SetActive(value);
                }
                if (survivalText != null)
                {
                    survivalText.gameObject.SetActive(value);
                }
            }

            public void Set(GameData.Ranking ranking)
            {
                if (allyText != null)
                {
                    switch (ranking.character)
                    {
                        case Character.Hunter:
                            allyText.text = Hunter;
                            break;
                        case Character.Raptor:
                            allyText.text = Raptor;
                            break;
                    }
                }
                if (enemyText != null)
                {
                    switch (ranking.enemy)
                    {
                        case GameData.Enemy.Hunter:
                            enemyText.text = Hunter;
                            break;
                        case GameData.Enemy.Raptor:
                            enemyText.text = Raptor;
                            break;
                        case GameData.Enemy.Mix:
                            enemyText.text = Mix;
                            break;
                    }
                }
                if(killText != null)
                {
                    killText.text = ranking.killCount.ToString();
                }
                if (survivalText != null)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    int hour = 0;
                    int minute = 0;
                    float second = ranking.survivalTime;
                    while (second >= HourSecond)
                    {
                        hour++;
                        second -= HourSecond;
                    }
                    while (second >= MinuteSecond)
                    {
                        minute++;
                        second -= MinuteSecond;
                    }
                    if (hour > 0 && minute > 0 && second > 0)
                    {
                        stringBuilder.Append(hour + Hour + " " + minute + Minute + " " + Mathf.Floor(second) + Second);
                    }
                    else if (hour > 0 && minute > 0)
                    {
                        stringBuilder.Append(hour + Hour + " " + minute + Minute);
                    }
                    else if (hour > 0 && second > 0)
                    {
                        stringBuilder.Append(hour + Hour + " " + Mathf.Floor(second) + Second);
                    }
                    else if (minute > 0 && second > 0)
                    {
                        stringBuilder.Append(minute + Minute + " " + Mathf.Floor(second) + Second);
                    }
                    else if (minute > 0)
                    {
                        stringBuilder.Append(minute + Minute);
                    }
                    else
                    {
                        stringBuilder.Append(Mathf.Floor(second) + Second);
                    }
                    survivalText.text = stringBuilder.ToString();
                }
            }
        }

        [Header("���� �±�"), SerializeField]
        private Tag descriptionTag;

        [Header("�ε��� �±׵�"), SerializeField]
        private Tag[] indexTags;

        public void Set(bool value)
        {
            if (titleText != null)
            {
                titleText.gameObject.SetActive(value);
            }
            descriptionTag.Set(value);
            int length = indexTags != null ? indexTags.Length : 0;
            for(int i = 0; i < length; i++)
            {
                indexTags[i].Set(value);
            }
        }

        public void Set(GameData.Ranking[] rankings)
        {
            if (indexTags != null)
            {
                int length = rankings != null ? rankings.Length : 0;
                for (int i = 0; i < indexTags.Length; i++)
                {
                    if(i < length)
                    {
                        indexTags[i].Set(rankings[i]);
                    }
                    else
                    {
                        indexTags[i].Set();
                    }
                }
            }
        }
    }

    [Serializable]
    private struct QuitBundle
    {
        [Header("���� �ؽ�Ʈ"), SerializeField]
        private TMP_Text titleText;
        [Header("�� ��ư"), SerializeField]
        private Button yesButton;
        [Header("�ƴϿ� ��ư"), SerializeField]
        private Button noButton;

        public void Set(bool value)
        {
            if(titleText != null)
            {
                titleText.gameObject.SetActive(value);
            }
            if (yesButton != null)
            {
                yesButton.gameObject.SetActive(value);
            }
            if (noButton != null)
            {
                noButton.gameObject.SetActive(value);
            }
        }
    }

    [Header("���� ���� ����"), SerializeField]
    private PlayBundle _playBundle;
    [Header("���� ���� ����"), SerializeField]
    private RankingBundle _rankingBundle;
    [Header("���� ȿ��"), SerializeField]
    private AudioPanel _audioPanel = null;
    [Header("�����ϱ� ����"), SerializeField]
    private QuitBundle _quitBundle;
    [Header("�ε� ������"), SerializeField]
    private LoadingPanel _loadingPanel = null;

    private enum Mode
    {
        Play,
        Ranking,
        Audio,
        Quit
    }

    private void Awake()
    {
        _rankingBundle.Set(GameData.Load());
    }

    private void Open(Mode mode)
    {
        Open();
        _playBundle.Set(mode == Mode.Play);
        _rankingBundle.Set(mode == Mode.Ranking);
        if (mode == Mode.Audio)
        {
            _audioPanel?.Open();
        }
        else
        {
            _audioPanel?.Close();
        }
        _quitBundle.Set(mode == Mode.Quit);
    }

    public void OpenPlay()
    {
        Open(Mode.Play);
    }

    public void OpenRanking()
    {
        Open(Mode.Ranking);
    }

    public void OpenAudio()
    {
        Open(Mode.Audio);
    }

    public void OpenQuit()
    {
        Open(Mode.Quit);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SelectAllyHunter()
    {
        _playBundle.Select(Character.Hunter);
    }

    public void SelectAllyRaptor()
    {
        _playBundle.Select(Character.Raptor);
    }

    public void SelectEnemyHunter()
    {
        _playBundle.Select(GameData.Enemy.Hunter);
    }

    public void SelectEnemyRaptor()
    {
        _playBundle.Select(GameData.Enemy.Raptor);
    }

    public void SelectEnemyMix()
    {
        _playBundle.Select(GameData.Enemy.Mix);
    }

    public void SelectStart(string scene)
    {
        if (_loadingPanel != null)
        {
            LoadingPanel loadingPanel = Instantiate(_loadingPanel, getRectTransform);
            loadingPanel.Open(scene);
        }
    }
}