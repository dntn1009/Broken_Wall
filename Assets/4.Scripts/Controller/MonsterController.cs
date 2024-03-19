using UnityEngine;
using DefinedEnums;

public class MonsterController : MonoBehaviour
{

    #region Setting & Effect
    Rigidbody2D m_rigidbody;
    bool _isStop = false; // 바닥착지시 멀어져있던 몬스터가 떨어질때 위로 올라가는 것 방지
    [SerializeField] GameObject BreakEffect;
    #endregion

    #region Monster Info
    [SerializeField] MonsterData monsterdata;
    int m_hp;
    int Maxhp;
    #endregion

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(IngameManager.Instance._currentScene == SceneState.Finish)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) // 쉴드 쳤을떄 다른 몬스터들 위로 올라가게
    {
        if (collision.collider.CompareTag("Monster"))
        {
            if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.y > 0)
            {
                m_rigidbody.velocity = Vector2.zero;
                m_rigidbody.velocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity * 1.5f;
            }
            else
            {
                if (_isStop)
                {
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    collision.gameObject.GetComponent<MonsterController>()._isStop = true;
                }
                else
                {
                    m_rigidbody.velocity = Vector2.zero;
                    m_rigidbody.velocity = new Vector2(0, 1) * 5f;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>()._isBtn = true;
            _isStop = false;
            IngameManager.Instance._isPhit = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) // 몬스터가 땅에 박았을때 or 몬스터가 땅에박기 전 + 플레이어와 겹쳐있을 때
    {
        if (collision.CompareTag("Player") && transform.position.y <= -2.36f)
        {
            IngameManager.Instance.DecreasHearts();
            if(collision.gameObject.GetComponent<PlayerController>().m_hp <= 0)
            {
                IngameManager.Instance.ChangeCurrentScene(SceneState.Finish);
            }//체력이 0이 되면 Finish Scene
            if (collision.gameObject.GetComponent<PlayerController>()._isBtn)
            {
                collision.gameObject.GetComponent<PlayerController>().SetIdle();
            }
            collision.gameObject.GetComponent<PlayerController>()._isBtn = false;
            _isStop = true;
        }
        if (collision.CompareTag("Monster"))
        {
            if (!_isStop)
            {
                collision.gameObject.GetComponent<MonsterController>()._isStop = false;
            }
        }
    }


    #region Pool & Demage Methods
    public void Init_monster(float y)
    {
        m_hp = monsterdata.HP + IngameManager.Instance.ScoreMonsterHPPlus();
        Maxhp = m_hp;
        gameObject.transform.position = new Vector2(0, y);
        gameObject.SetActive(true);
    }

    public void SetDemage(int Att)
    {
        m_hp -= Att;
        if (m_hp <= 0)
        {
            m_hp = 0;
            Instantiate(BreakEffect, transform.position, Quaternion.identity);
            IngameManager.Instance.ScorePlus(monsterdata.POINT);
            IngameManager.Instance.CoinPlus(monsterdata.COIN);
            IngameManager.Instance.SlashBosterIncrease();
            this.gameObject.SetActive(false);
        }
    }

    public void SettingHpBar()
    {
        IngameManager.Instance.MonsterHpbar.GetComponent<MonHpbarController>().SetHpbar(monsterdata.NAME, m_hp, Maxhp);
    }
    #endregion

}
