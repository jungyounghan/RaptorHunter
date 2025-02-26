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
        [Header("플레이어 선택"), SerializeField]
        private TMP_Text allyText;
        [Header("상대할 적 선택"), SerializeField]
        private TMP_Text enemyText;

        [Header("플레이어 사냥꾼 버튼"), SerializeField]
        private Button allyHunterButton;
        [Header("플레이어 랩터 버튼"), SerializeField]
        private Button allyRaptorButton;

        [Header("상대할 적 사냥꾼 버튼"), SerializeField]
        private Button enemyHunterButton;
        [Header("상대할 적 랩터 버튼"), SerializeField]
        private Button enemyRaptorButton;
        [Header("상대할 적 혼합 버튼"), SerializeField]
        private Button enemyMixButton;

        [Header("시작 버튼"), SerializeField]
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
        [Header("제목 텍스트"), SerializeField]
        private TMP_Text titleText;
        [Header("설명 텍스트"), SerializeField]
        private TMP_Text descriptionText;
        [Header("기록 텍스트"), SerializeField]
        private TMP_Text[] recordTexts;

        private static string Player = "플레이어";

        private static string Enemy = "적";

        private static string Hunter = "사냥꾼";

        private static string Raptor = "랩터";

        private static string Mix = "사냥꾼과 랩터";

        private static string Kill = "처치";

        private static string Survival = "초 생존";

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

    [Header("게임 시작 묶음"), SerializeField]
    private PlayBundle _playBundle;
    [Header("순위 보기 묶음"), SerializeField]
    private RankingBundle _rankingBundle;
    [Header("음향 효과"), SerializeField]
    private AudioPanel _audioPanel = null;
    [Header("로딩 프리팹"), SerializeField]
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