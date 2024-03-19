using DefinedEnums;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : PlayerAnimationController
{

    //Var
    #region Player Info
    public PlayerData playerdata;
    public int m_hp; // 플레이어 체력
    #endregion

    #region Setting
    public PlayerMotion _currentMotion; // 최근 모션
    Rigidbody2D m_rigidbody; // RigidBody_1
    
    Vector2 JumpVector; // new Vector 0, 1
    bool _isFall = false; // 낙하 감지
    bool _isGround; // 착지 감지
    bool _isJump = false;

    bool _isSlash = true; // 공격 타이밍
    float SlashTime = 0.2f; // 공격 가능 시간
    float DurSlashTime; // 공격 쿨타임 진행시간

    bool _isShield = true; // 쉴드 가능
    int ShieldTime = 3; // 쉴드 불가능 시간
    float DurShieldTime; // 쉴드 쿨타임 진행시간


    public bool _isBtn = true;
    [Header("기본 세팅")]
    [SerializeField] BoxCollider2D Attack_Rng; // Slash 범위(Box Colider2D)
    [SerializeField] float SPEED; // 공중 속도 부여
    [SerializeField] Image Shieldcoltime; // 쉴드 쿨타임 이미지
    #endregion

    #region Effect & Position
    [Header("이펙트 & 포지션")]
    [SerializeField] Transform Jump_Pos;
    [SerializeField] GameObject Jump_Effect;
    [SerializeField] Transform Boster_Pos;
    [SerializeField] GameObject Boster_Effect;
    #endregion
    //
    void Start()
    {
        _currentMotion = PlayerMotion.Idle;
        m_rigidbody = GetComponent<Rigidbody2D>();// RigidBody_2
        JumpVector = new Vector2(0, 1); // JumpVecotr
    }

    void Update()
    {
        Ground_Check();
        PlayerKeySetting();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if(_currentMotion == PlayerMotion.Shield)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = m_rigidbody.velocity / 2 + JumpVector * 5f; // 몬스터를 팅겨내서 위로 올라감\

                m_rigidbody.velocity = Vector2.zero;
                SetIdle();
                _isShield = false;
            }
            else
            {
                m_rigidbody.velocity = Vector2.zero;
                m_rigidbody.velocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity; // 물건과 같이 내려옴
            }   
        }
    } // 쉴드로 타이밍있게 쳤을때
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (_currentMotion == PlayerMotion.Shield)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = m_rigidbody.velocity / 2 + JumpVector * 20f; // 몬스터를 팅겨내서 위로 올라감\
                m_rigidbody.velocity = Vector2.zero;
                SetIdle();
                _isShield = false;
            }
        }
    }

    #region Player SettingMethods
    void Ground_Check()
    {
        int lMask = 1 << LayerMask.NameToLayer("Ground"); // Ground LayerMask
        RaycastHit2D rHit2D = Physics2D.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1.5f, lMask); //Player -> Ground 1.5f DownRay
        if (rHit2D.transform != null && Vector3.Distance(rHit2D.point, transform.position) <= 0.75f)
        {
            _isJump = true; // 땅에 착지하면 점프 다시 가능
            _isGround = true; // 지면 고정 역할
            if (!_isFall)
                m_rigidbody.velocity = Vector2.zero; // 점프 전까지는 zero, _isfall = false
        }
        else
        {
            _isJump = false; // 점프하는 동안 2단 점프 방지
            _isFall = false; // 점프를 하면서 _isfall = true,  RayHit을 벗어나면 false
            _isGround = false; // 점프하고 있는 중
        }
        if (_isGround)
        {
            transform.position = new Vector2(0, -2.42f); // 착지 지점
        }
    }//Ground 착지 지점 RayCast

    void PlayerKeySetting()
    {
        if (_isSlash)
        {
            DurSlashTime += Time.deltaTime;
        }

        if (!_isShield)
        {
            DurShieldTime += Time.deltaTime;
            Shieldcoltime.fillAmount += Time.deltaTime / ShieldTime; //coltime fill 채우기
            if (ShieldTime <= DurShieldTime)
            {
                _isShield = true;
                Shieldcoltime.fillAmount = 0;
                DurShieldTime = 0f;
            }
        }

    }//Player KeyTime

    #endregion

    #region ButtonMethods

    public void JumpBtn()
    {
        if (_isBtn)
        {
            if (_isJump)
            {
                _isFall = true;
                m_rigidbody.velocity = JumpVector * SPEED; // Jump_Vector
                Instantiate(Jump_Effect, Jump_Pos);
            }
        }
    }

    public void ShieldBtn()
    {
        if (_isShield)
        {
            Play(PlayerMotion.Shield);
            _currentMotion = PlayerMotion.Shield;
        }
    }

    public void SlashBtn()
    {
        if (_isBtn)
        {
            if (_isSlash)
            {
                if (SlashTime <= DurSlashTime)
                {
                    if(IngameManager.Instance.Slash_Boster.fillAmount >= 1f)
                    {
                        IngameManager.Instance.Slash_Boster.fillAmount = 0f;
                        var boster = Instantiate(Boster_Effect, Boster_Pos.position, Quaternion.identity);
                        boster.GetComponent<BosterController>().canvas = GameObject.FindGameObjectWithTag("AttRng").GetComponent<AttackController>().canvas;
                    }
                    Play(PlayerMotion.Slash1);
                    _currentMotion = PlayerMotion.Slash1;
                    DurSlashTime = 0f;
                }
            }
        }
    }

    #endregion

    #region AnimEventMethods
    public void AnimEventAttackRngSetFalse()
    {
        _isSlash = true;
        Attack_Rng.enabled = false;
        SetIdle();
    }
    public void AnimEventAttackRngSetTrue()
    {
        _isSlash = false;
        Attack_Rng.enabled = true;
    }
    #endregion

    #region etcMethods

    public void SetIdle()
    {
        Play(PlayerMotion.Idle);
        _currentMotion = PlayerMotion.Idle;
    }

    #endregion
}
