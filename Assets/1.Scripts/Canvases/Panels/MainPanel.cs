using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainPanel : Panel
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
            switch(ally)
            {
                case Character.Hunter:
                    if (allyHunterButton != null && allyHunterButton.image != null)
                    {
                        allyHunterButton.image.color = PressedColor;
                    }
                    if (allyRaptorButton != null && allyRaptorButton.image != null)
                    {
                        allyRaptorButton.image.color = NormalColor;
                    }
                    break;
                case Character.Raptor:
                    if (allyHunterButton != null && allyHunterButton.image != null)
                    {
                        allyHunterButton.image.color = NormalColor;
                    }
                    if (allyRaptorButton != null && allyRaptorButton.image != null)
                    {
                        allyRaptorButton.image.color = PressedColor;
                    }
                    break;
            }
        }

        public void Select(GameData.Enemy enemy)
        {
            GameData.enemy = enemy;
            switch (enemy)
            {
                case GameData.Enemy.Hunter:
                    if (enemyHunterButton != null && enemyHunterButton.image != null)
                    {
                        enemyHunterButton.image.color = PressedColor;
                    }
                    if (enemyRaptorButton != null && enemyRaptorButton.image != null)
                    {
                        enemyRaptorButton.image.color = NormalColor;
                    }
                    if (enemyMixButton != null && enemyMixButton.image != null)
                    {
                        enemyMixButton.image.color = NormalColor;
                    }
                    break;
                case GameData.Enemy.Raptor:
                    if (enemyHunterButton != null && enemyHunterButton.image != null)
                    {
                        enemyHunterButton.image.color = NormalColor;
                    }
                    if (enemyRaptorButton != null && enemyRaptorButton.image != null)
                    {
                        enemyRaptorButton.image.color = PressedColor;
                    }
                    if (enemyMixButton != null && enemyMixButton.image != null)
                    {
                        enemyMixButton.image.color = NormalColor;
                    }
                    break;
                case GameData.Enemy.Mix:
                    if (enemyHunterButton != null && enemyHunterButton.image != null)
                    {
                        enemyHunterButton.image.color = NormalColor;
                    }
                    if (enemyRaptorButton != null && enemyRaptorButton.image != null)
                    {
                        enemyRaptorButton.image.color = NormalColor;
                    }
                    if (enemyMixButton != null && enemyMixButton.image != null)
                    {
                        enemyMixButton.image.color = PressedColor;
                    }
                    break;
            }
        }

        public void Open(bool value)
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
                allyHunterButton.gameObject.SetActive(value);
                if(allyHunterButton.image != null)
                {
                    allyHunterButton.image.color = GameData.ally == true ? PressedColor : NormalColor;
                }
            }
            if (allyRaptorButton != null)
            {
                allyRaptorButton.gameObject.SetActive(value);
                if (allyRaptorButton.image != null)
                {
                    allyRaptorButton.image.color = GameData.ally == false ? PressedColor : NormalColor;
                }
            }
            if (enemyHunterButton != null)
            {
                enemyHunterButton.gameObject.SetActive(value);
                if (enemyHunterButton.image != null)
                {
                    enemyHunterButton.image.color = GameData.enemy == GameData.Enemy.Hunter ? PressedColor : NormalColor;
                }
            }
            if (enemyRaptorButton != null)
            {
                enemyRaptorButton.gameObject.SetActive(value);
                if (enemyRaptorButton.image != null)
                {
                    enemyRaptorButton.image.color = GameData.enemy == GameData.Enemy.Raptor ? PressedColor : NormalColor;
                }
            }
            if (enemyMixButton != null)
            {
                enemyMixButton.gameObject.SetActive(value);
                if (enemyMixButton.image != null)
                {
                    enemyMixButton.image.color = GameData.enemy == GameData.Enemy.Mix ? PressedColor : NormalColor;
                }
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
        [Header("���� �ؽ�Ʈ"), SerializeField]
        private TMP_Text descriptionText;
        [Header("��� �ؽ�Ʈ"), SerializeField]
        private TMP_Text[] recordTexts;

        private static string Player = "�÷��̾�";

        private static string Enemy = "��";

        private static string Hunter = "��ɲ�";

        private static string Raptor = "����";

        private static string Mix = "��ɲ۰� ����";

        private static string Kill = "óġ";

        private static string Survival = "�� ����";

        public void Open(bool value)
        {
            if (titleText != null)
            {
                titleText.gameObject.SetActive(value);
            }
            if (descriptionText != null)
            {
                descriptionText.gameObject.SetActive(value);
            }
            int length = recordTexts != null ? recordTexts.Length : 0;
            for(int i = 0; i < length; i++)
            {
                if(recordTexts[i] != null)
                {
                    recordTexts[i].gameObject.SetActive(value);
                }
            }
        }

        public void Set(GameData.Ranking[] rankings)
        {
            if (recordTexts != null)
            {
                int length = rankings != null ? rankings.Length : 0;
                for (int i = 0; i < recordTexts.Length; i++)
                {
                    if (recordTexts[i] != null)
                    {
                        if (i < length)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            switch (rankings[i].character)
                            {
                                case Character.Hunter:
                                    stringBuilder.Append(Player + ": " + Hunter);
                                    break;
                                case Character.Raptor:
                                    stringBuilder.Append(Player + ": " + Raptor);
                                    break;
                            }
                            switch (rankings[i].enemy)
                            {
                                case GameData.Enemy.Hunter:
                                    stringBuilder.Append(" " + Enemy + ": " + Hunter);
                                    break;
                                case GameData.Enemy.Raptor:
                                    stringBuilder.Append(" " + Enemy + ": " + Raptor);
                                    break;
                                case GameData.Enemy.Mix:
                                    stringBuilder.Append(" " + Enemy + ": " + Mix);
                                    break;
                            }
                            stringBuilder.Append(" " + rankings[i].killCount + " " + Kill);
                            stringBuilder.Append(" " + rankings[i].survivalTime + Survival);
                            recordTexts[i].text = stringBuilder.ToString();
                        }
                        else
                        {
                            recordTexts[i].text = null;
                        }
                    }
                }
            }
        }
    }

    [Header("���� ���� ����"), SerializeField]
    private PlayBundle _playBundle;
    [Header("���� ���� ����"), SerializeField]
    private RankingBundle _rankingBundle;
    [Header("���� ȿ��"), SerializeField]
    private AudioPanel _audioPanel = null;
    [Header("�ε� ������"), SerializeField]
    private LoadingPanel _loadingPanel = null;

    private enum Mode
    {
        Play,
        Ranking,
        Audio,
    }

    private void Awake()
    {
        _rankingBundle.Set(GameData.Load());
    }

    private void Open(Mode mode)
    {
        Open();
        _playBundle.Open(mode == Mode.Play);
        _rankingBundle.Open(mode == Mode.Ranking);
        if (mode == Mode.Audio)
        {
            _audioPanel?.Open();
        }
        else
        {
            _audioPanel?.Close();
        }
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

    public void SelectStart()
    {
        if (_loadingPanel != null)
        {
            LoadingPanel loadingPanel = Instantiate(_loadingPanel, getRectTransform);
            loadingPanel.LoadScene(name);
        }
    }
}