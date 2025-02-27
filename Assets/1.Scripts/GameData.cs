using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ���� ���࿡ �ʿ��� �������� ��� �ִ� Ŭ����
/// </summary>
public class GameData
{
    public static bool ally = Character.Hunter;

    public enum Enemy: byte
    {
        Hunter,
        Raptor,
        Mix
    }

    public static Enemy enemy = Enemy.Raptor;

    [Serializable]
    public struct Ranking
    {
        public bool character;
        public Enemy enemy;
        public uint killCount;
        public float survivalTime;

        public Ranking(bool character, Enemy enemy, uint killCount, float survivalTime)
        {
            this.character = character;
            this.enemy = enemy;
            this.killCount = killCount;
            this.survivalTime = survivalTime;
        }

        public static bool operator >(Ranking a, Ranking b)
        {
            return a.killCount * a.survivalTime > b.killCount * b.survivalTime;
        }

        public static bool operator <(Ranking a, Ranking b)
        {
            return a.killCount * a.survivalTime < b.killCount * b.survivalTime;
        }

        public static bool operator>=(Ranking a, Ranking b)
        {
            return a.killCount * a.survivalTime >= b.killCount * b.survivalTime;
        }

        public static bool operator <=(Ranking a, Ranking b)
        {
            return a.killCount * a.survivalTime <= b.killCount * b.survivalTime;
        }
    }

    [Serializable]
    public struct RankingArray
    {
        public Ranking[] array;

        public RankingArray(Ranking[] array)
        {
            this.array = array;
        }
    }

    private readonly static string FilePath = Path.Combine(Application.persistentDataPath, "ranking.json");

    //��ŷ ����
    public static bool Save(bool character, Enemy enemy, uint killCount, float survivalTime)
    {
        Ranking[] rankings = Load();
        List<Ranking> list = new List<Ranking>();
        Ranking ranking = new Ranking(character, enemy, killCount, survivalTime);
        int length = rankings != null ? rankings.Length : 0;
        bool done = false;
        for (int i = 0; i < length; i++)
        {
            if (rankings[i] < ranking)
            {
                list.Add(ranking);
                done = true;
            }
            list.Add(rankings[i]);
        }
        if (done == false)
        {
            list.Add(ranking);
        }
        string json = JsonUtility.ToJson(new RankingArray(list.ToArray()), true); // true = ���� ���� ����
        File.WriteAllText(FilePath, json); // ���Ͽ� ����
        return done;
    }

    public static Ranking[] Load()
    {
        // ������ �����ϴ��� Ȯ��
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath); // ���Ͽ��� JSON �б�
            try
            {
                RankingArray rankingArray = JsonUtility.FromJson<RankingArray>(json); // JSON�� ��ü�� ��ȯ
                return rankingArray.array;
            }
            catch
            {
                return null; // ���� �߻� �� null ��ȯ
            }
        }
        return null;
    }
}