using System;
using System.Text;
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
    private TMP_Text _text = null;

    private static string Player = "플레이어";

    private static string Enemy = "적";

    private static string Hunter = "사냥꾼";

    private static string Raptor = "랩터";

    private static string Mix = "사냥꾼과 랩터";

    private static string Kill = "처치";

    private static string Survival = "초 생존";

    public void Set(bool character, GameData.Enemy enemy, uint killCount, float survivalTime)
    {
        StringBuilder stringBuilder = new StringBuilder();
        switch (character)
        {
            case Character.Hunter:
                stringBuilder.Append(Player + ": " + Hunter);
                break;
            case Character.Raptor:
                stringBuilder.Append(Player + ": " + Raptor);
                break;
        }
        switch (enemy)
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
        stringBuilder.Append(" " + killCount + " " + Kill);
        stringBuilder.Append(" " + survivalTime + Survival);
        if(_text != null)
        {
            _text.text = stringBuilder.ToString();
        }
    }
}