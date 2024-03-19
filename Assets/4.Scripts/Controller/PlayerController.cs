using DefinedEnums;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : PlayerAnimationController
{

    //Var
    #region Player Info
    public PlayerData playerdata;
    public int m_hp; // �÷��̾� ü��
    #endregion

    #region Setting
    public PlayerMotion _currentMotion; // �ֱ� ���
    Rigidbody2D m_rigidbody; // RigidBody_1
    
    Vector2 JumpVector; // new Vector 0, 1
    bool _isFall = false; // ���� ����
    bool _isGround; // ���� ����
    bool _isJump = false;

    bool _isSlash = true; // ���� Ÿ�̹�
    float SlashTime = 0.2f; // ���� ���� �ð�
    float DurSlashTime; // ���� ��Ÿ�� ����ð�

    bool _isShield = true; // ���� ����
    int ShieldTime = 3; // ���� �Ұ��� �ð�
    float DurShieldTime; // ���� ��Ÿ�� ����ð�


    public bool _isBtn = true;
    [Header("�⺻ ����")]
    [SerializeField] BoxCollider2D Attack_Rng; // Slash ����(Box Colider2D)
    [SerializeField] float SPEED; // ���� �ӵ� �ο�
    [SerializeField] Image Shieldcoltime; // ���� ��Ÿ�� �̹���
    #endregion

    #region Effect & Position
    [Header("����Ʈ & ������")]
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
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = m_rigidbody.velocity / 2 + JumpVector * 5f; // ���͸� �ðܳ��� ���� �ö�\

                m_rigidbody.velocity = Vector2.zero;
                SetIdle();
                _isShield = false;
            }
            else
            {
                m_rigidbody.velocity = Vector2.zero;
                m_rigidbody.velocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity; // ���ǰ� ���� ������
            }   
        }
    } // ����� Ÿ�̹��ְ� ������
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (_currentMotion == PlayerMotion.Shield)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = m_rigidbody.velocity / 2 + JumpVector * 20f; // ���͸� �ðܳ��� ���� �ö�\
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
            _isJump = true; // ���� �����ϸ� ���� �ٽ� ����
            _isGround = true; // ���� ���� ����
            if (!_isFall)
                m_rigidbody.velocity = Vector2.zero; // ���� �������� zero, _isfall = false
        }
        else
        {
            _isJump = false; // �����ϴ� ���� 2�� ���� ����
            _isFall = false; // ������ �ϸ鼭 _isfall = true,  RayHit�� ����� false
            _isGround = false; // �����ϰ� �ִ� ��
        }
        if (_isGround)
        {
            transform.position = new Vector2(0, -2.42f); // ���� ����
        }
    }//Ground ���� ���� RayCast

    void PlayerKeySetting()
    {
        if (_isSlash)
        {
            DurSlashTime += Time.deltaTime;
        }

        if (!_isShield)
        {
            DurShieldTime += Time.deltaTime;
            Shieldcoltime.fillAmount += Time.deltaTime / ShieldTime; //coltime fill ä���
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
