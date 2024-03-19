using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [SerializeField] int hp;
    public int HP { get { return hp; } set { hp = value; } }
    [SerializeField] int weapon;
    public int WEAPON { get { return weapon; } set { weapon = value; } }
    [SerializeField] int att;
    public int ATT { get { return att; } set { att = value; } }
    [SerializeField] int best;
    public int BEST { get { return best; } set { best = value; } }
    [SerializeField] int coin;
    public int COIN { get { return coin; } set { coin = value; } }

    public PlayerData()
    {
        hp = 3;
        weapon = 1;
        att = 5;
        best = 0;
        coin = 0;
    }
}
