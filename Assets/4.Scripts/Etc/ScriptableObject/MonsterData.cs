using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster Data", menuName = "Scriptable Object/Monster Data", order = int.MaxValue)]
public class MonsterData : ScriptableObject
{
    [SerializeField] string name;
    public string NAME { get { return name; } }

    [SerializeField] int hp;
    public int HP { get { return hp; } }

    [SerializeField] int coin;
    public int COIN { get { return coin; } }

    [SerializeField] int point;
    public int POINT { get { return point; } }
}
