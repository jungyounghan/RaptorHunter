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

    [Header("�÷��̾�"), SerializeField]
    private TMP_Text _text = null;

    private static string Player = "�÷��̾�";

    private static string Enemy = "��";

    private static string Hunter = "��ɲ�";

    private static string Raptor = "����";

    private static string Mix = "��ɲ۰� ����";

    private static string Kill = "óġ";

    private static string Survival = "�� ����";

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