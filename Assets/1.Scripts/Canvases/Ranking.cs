using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class Ranking : MonoBehaviour
{
    [Serializable]
    public struct Info
    {
        public bool character;
        public GameData.Enemy enemy;
        public uint killCount;
        public float survivalTime;

        public Info(bool character, GameData.Enemy enemy, uint killCount, float survivalTime)
        {
            this.character = character;
            this.enemy = enemy;
            this.killCount = killCount;
            this.survivalTime = survivalTime;
        }
    }

    [Serializable]
    public struct InfoList
    {
        public Info[] infos;

        public InfoList(Info[] infos)
        {
            this.infos = infos;
        }
    }

    [Header("플레이어"), SerializeField]
    private List<TMP_Text> _text = new List<TMP_Text>();

    private static string Player = "플레이어";

    private static string Enemy = "적";

    private static string Hunter = "사냥꾼";

    private static string Raptor = "랩터";

    private static string Mix = "사냥꾼과 랩터";

    private static string Kill = "처치";

    private static string Survival = "초 생존";

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void SetScore(Info[] infos)
    {
        int length = infos != null ? infos.Length : 0;
        for(int i = 0; i < _text.Count; i++)
        {
            if (_text[i] != null)
            {
                if (i < length)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    switch (infos[i].character)
                    {
                        case Character.Hunter:
                            stringBuilder.Append(Player + ": " + Hunter);
                            break;
                        case Character.Raptor:
                            stringBuilder.Append(Player + ": " + Raptor);
                            break;
                    }
                    switch (infos[i].enemy)
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
                    stringBuilder.Append(" " + infos[i].killCount + " " + Kill);
                    stringBuilder.Append(" " + infos[i].survivalTime + Survival);
                    _text[i].text = stringBuilder.ToString();
                }
                else
                {
                    _text[i].text = null;
                }
            }
        }
    }
}