using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{

    public static bool ally = Character.Hunter;

    public enum Enemy: byte
    {
        Hunter,
        Raptor,
        Mix
    }

    public static Enemy enemy = Enemy.Raptor;


    //������� ����
    //ȿ���� ���� ����
    //��ŷ ����
}