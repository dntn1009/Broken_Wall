using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using DefinedEnums;
using UnityEngine.UI;


public class IngameManager : DonDestory<IngameManager>
{
    string FilePath;

#region Setting
    [Header("BOARD & INFO")]

    //Player BasicData
    PlayerData PlayerInfo;
    //Text
    [SerializeField] TextMeshProUGUI ScoreText; // Score
    [SerializeField] TextMeshProUGUI CoinText; // Coin
    //Buttons
    [SerializeField] GameObject IngameBtns; // �ΰ��� ��ư
    [SerializeField] GameObject MenuBtns; // �޴� ��ư
    [SerializeField] GameObject FinishUIs; // Finish UI
    [SerializeField] public Image Slash_Boster; // SlashBoster;
    //Sprite
    [SerializeField] Sprite FullHeart;
    [SerializeField] Sprite EmptyHeart;
    //Hearts
    [SerializeField] Image[] P_Hearts; // �÷��̾� ü�¹� �̹���
    [SerializeField] public MonHpbarController MonsterHpbar; // ���� HPbar
    //Upgrade
    [SerializeField] Image WeaponImage; // ���� ����
    [SerializeField] TextMeshProUGUI WeaponLevelText; // ���� ����
    [SerializeField] TextMeshProUGUI AttText; // ���� ���ݷ�
    //Weapon
    [SerializeField] Sprite NormalWeapon; // ��� ����(��)
    [SerializeField] Sprite HardWeapon; // �ϵ� ����(����)
    [SerializeField] GameObject PlayerWeapon; // �÷��̾� ����
    int UpgradeCoin = 20;

    public bool _isPhit = true;

    int IngameScore = 0;
    public SceneState _currentScene; // 1. MENU 2. PLAY 3. FINISH
    public int ChapterHP = 1;
    #endregion

    [SerializeField] PlayerController player;
    
    void Start()
    {
#if UNITY_EDITOR && UNITY_STANDALONE_WIN
        FilePath = Path.Combine(Application.streamingAssetsPath, "PlayerData.bin");
#elif UNITY_ANDROID
        FilePath = Path.Combine(Application.persistentDataPath, "PlayerData.bin");
#endif
        HideIngameButtons();
        ShowMenuButtons();
        PlayerInfo = new PlayerData();
        NewCheck();
    }

#region Menu & Finish Button Methods

    public void StartBtn()
    {
        ChangeCurrentScene(SceneState.Play);
    }
    public void UpgradeBtn()
    {
        if(player.playerdata.COIN - 20 >= 0 && player.playerdata.WEAPON < 10)
        {
            player.playerdata.COIN -= 20;
            player.playerdata.WEAPON += 1; // ���� ����
            player.playerdata.ATT += 2; // ���� ���ݷ�
            if(player.playerdata.WEAPON == 5)
            {
                WeaponImage.sprite = NormalWeapon;
                PlayerWeapon.GetComponent<SpriteRenderer>().sprite = NormalWeapon;
                PlayerWeapon.GetComponent<SpriteRenderer>().flipX = true;
            }
            else if(player.playerdata.WEAPON == 10)
            {
                WeaponImage.sprite = HardWeapon;
                PlayerWeapon.GetComponent<SpriteRenderer>().sprite = HardWeapon;
                PlayerWeapon.GetComponent<SpriteRenderer>().flipX = false;
            }
            SaveBinary();
            WeaponLevelText.text = "Lv." + player.playerdata.WEAPON;
            AttText.text = "Att: " + player.playerdata.ATT;
            CoinText.text = player.playerdata.COIN.ToString();
        }
    }
    public void RestartBtn()
    {
        ChangeCurrentScene(SceneState.Menu);
    }

    public void EnforceWeaponBtn()
    {
        //���� ��ȭ
        SaveBinary();
    }

#endregion

#region Change Scene & Menu Methods

    public void ChangeCurrentScene(SceneState _changeScene)
    {
        _currentScene = _changeScene;
        switch (_currentScene)
        {
            case SceneState.Menu:
                IncreaseHearts();
                HideIngameButtons();
                HideFinishUIs();
                ShowMenuButtons();
                ScoreText.text = "Best " + player.playerdata.BEST.ToString();
                CoinText.text = player.playerdata.COIN.ToString();
                break;
            case SceneState.Play:
                HideMenuButtons();
                HideFinishUIs();
                ShowIngameButtons();
                ScoreText.text = "0";
                CoinText.text = player.playerdata.COIN.ToString();
                break;
            case SceneState.Finish:
                Slash_Boster.fillAmount = 0f;
                if (IngameScore > player.playerdata.BEST)
                    player.playerdata.BEST = IngameScore;
                HideIngameButtons();
                HideMenuButtons();
                ShowFinishUIs();
                SaveBinary(); // ���� ����
                break;
        }

    }
    public void ShowMenuButtons()
    {
        MenuBtns.SetActive(true);
    }

    public void HideMenuButtons()
    {
        MenuBtns.SetActive(false);
    }

    public void ShowIngameButtons()
    {
        IngameBtns.SetActive(true);
    }
    public void HideIngameButtons()
    {
        IngameBtns.SetActive(false);
    }

    public void ShowFinishUIs()
    {
        FinishUIs.SetActive(true);
    }
    public void HideFinishUIs()
    {
        FinishUIs.SetActive(false);
    }

#endregion

#region IngameMethods

    public void DecreasHearts()
    {
        if (_isPhit)
        {
            player.m_hp -= 1;//ü�� ���̳ʽ�
            P_Hearts[player.m_hp].sprite = EmptyHeart;
            _isPhit = false;
        }

        if (player.m_hp <= 0)
        {
            IngameManager.Instance.ChangeCurrentScene(SceneState.Finish);
        }//ü���� 0�� �Ǹ� Finish Scene
    }
    public void IncreaseHearts()
    {
        player.m_hp = player.playerdata.HP;
        _isPhit = true;
        for(int i = 0; i < 3; i++)
        {
            P_Hearts[i].sprite = FullHeart;
        }
    }
    public void ScorePlus(int point)
    {
        IngameScore += point;
        ScoreText.text = IngameScore.ToString();
    }
    public void CoinPlus(int Coin)
    {
        player.playerdata.COIN += Coin;
        CoinText.text = player.playerdata.COIN.ToString();
    }

    public void SlashBosterIncrease()
    {
        if(Slash_Boster.fillAmount < 1f)
        {
            Slash_Boster.fillAmount += 0.125f;
        }
    }

    public int ScoreMonsterHPPlus()
    {
        if(IngameScore < 200)
        {
            return 0;
        }
        else if(IngameScore >= 200 && IngameScore < 400)
        {
            return 5;
        }
        else
        {
            return 10;
        }

    }
#endregion

#region PlayerDataMethods
    public void NewCheck()
    {
        if(!File.Exists(FilePath))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(FilePath));
            directoryInfo.Create();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(FilePath, FileMode.OpenOrCreate);
            binaryFormatter.Serialize(fileStream, PlayerInfo);
            fileStream.Close();
            player.playerdata = PlayerInfo;
        }
        else
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(FilePath, FileMode.Open);
            player.playerdata = (PlayerData)binaryFormatter.Deserialize(fileStream);
        }
        player.m_hp = player.playerdata.HP;
        ScoreText.text = "Best " + player.playerdata.BEST.ToString();
        CoinText.text = player.playerdata.COIN.ToString();
        WeaponLevelText.text = "Lv." + player.playerdata.WEAPON;
        AttText.text = "Att: " + player.playerdata.ATT;
        if (player.playerdata.WEAPON == 5)
        {
            WeaponImage.sprite = NormalWeapon;
            PlayerWeapon.GetComponent<SpriteRenderer>().sprite = NormalWeapon;
            PlayerWeapon.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if(player.playerdata.WEAPON == 10)
        {
            WeaponImage.sprite = HardWeapon;
            PlayerWeapon.GetComponent<SpriteRenderer>().sprite = HardWeapon;
            PlayerWeapon.GetComponent<SpriteRenderer>().flipX = false;
        }
    }//PlayerData�� ������ ���λ��� or ������ Data �о���� + ������ �� �о�� �����ͷ� �ٹ̱�
    public void SaveBinary() // Finish�� �Ǹ� or ������ ��ȭ�ϸ�
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Open(FilePath, FileMode.OpenOrCreate);
        binaryFormatter.Serialize(fileStream, player.playerdata);
        fileStream.Close();
    }
#endregion
}
