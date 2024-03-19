using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinedEnums;
public class MonsterManager : DonDestory<MonsterManager>
{
    //Var
    #region Object Pool
    [Header("������Ʈ Ǯ")]
    GameObjectPool<MonsterController> N_WoodPool;
    [SerializeField] GameObject NWoodPrefab;
    GameObjectPool<MonsterController> H_WoodPool;
    [SerializeField] GameObject HWoodPrefab;
    #endregion


    #region Monster Spawn
    int Max = 10; // ���� �ִ� ����
    float Init_Y = 41f;

    float SpawnTime = 4.5f;
    float DurationTime;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Pool_Settiong();
    }

    // Update is called once per frame
    void Update()
    {
        if (IngameManager.Instance._currentScene == SceneState.Play)
        {
            DurationTime += Time.deltaTime;
            if (SpawnTime <= DurationTime)
            {
                CreateMonster();
                DurationTime = 0;
            }
        }
    }


    #region Monster_SpawnMethods
    void Pool_Settiong()
    {
        N_WoodPool = new GameObjectPool<MonsterController>(Max, () =>
        {
            var obj = Instantiate(NWoodPrefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            var mon = obj.GetComponent<MonsterController>();
            return mon;
        });// �Ϲ� �볪�� ������Ʈ Ǯ

        H_WoodPool = new GameObjectPool<MonsterController>(Max, () =>
        {
            var obj = Instantiate(HWoodPrefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            var mon = obj.GetComponent<MonsterController>();
            return mon;
        });// �Ϲ� �볪�� ������Ʈ Ǯ
    }// ������Ʈ Ǯ ����

    public void CreateMonster()
    {
        float StackY = 0;
        for (int i = 0; i < 2; i++)
        {
            int Rnadom = Random.Range(0, 11);
            if (i >= 1)
                StackY += 1.5f;

            if(Rnadom <= 7)
            {
                var Nwood = N_WoodPool.Get();
                Nwood.Init_monster(Init_Y + StackY);
            }
            else
            {
                var Hwood = H_WoodPool.Get();
                Hwood.Init_monster(Init_Y + StackY);
            }   
        }
    }
    #endregion
}
