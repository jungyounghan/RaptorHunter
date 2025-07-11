using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 게임 진행에 필요한 정보들을 담고 있는 클래스
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

    private readonly static int MaxRankingCount = 5;
    private readonly static string FilePath = Path.Combine(Application.persistentDataPath, "ranking.json");

    //랭킹 저장
    public static bool Save(bool character, Enemy enemy, uint killCount, float survivalTime)
    {
        Ranking[] rankings = Load();
        List<Ranking> list = new List<Ranking>();
        Ranking ranking = new Ranking(character, enemy, killCount, survivalTime);
        int length = rankings != null ? rankings.Length : 0;
        bool done = false;
        int index = 0;
        for (int i = 0; i < length; i++)
        {
            if (done == false && rankings[i] < ranking)
            {
                list.Add(ranking);
                done = true;
            }
            list.Add(rankings[i]);
            index++;
        }
        if (done == false)
        {
            list.Add(ranking);
            index++;
        }
        string json = JsonUtility.ToJson(new RankingArray(list.ToArray()), true); // true = 보기 좋게 포맷
        File.WriteAllText(FilePath, json); // 파일에 저장
        return done && index < MaxRankingCount;
    }

    public static Ranking[] Load()
    {
        // 파일이 존재하는지 확인
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath); // 파일에서 JSON 읽기
            try
            {
                RankingArray rankingArray = JsonUtility.FromJson<RankingArray>(json); // JSON을 객체로 변환
                return rankingArray.array;
            }
            catch
            {
                return null; // 예외 발생 시 null 반환
            }
        }
        return null;
    }
}