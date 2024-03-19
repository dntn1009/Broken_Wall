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
    [SerializeField] GameObject IngameBtns; // 인게임 버튼
    [SerializeField] GameObject MenuBtns; // 메뉴 버튼
    [SerializeField] GameObject FinishUIs; // Finish UI
    [SerializeField] public Image Slash_Boster; // SlashBoster;
    //Sprite
    [SerializeField] Sprite FullHeart;
    [SerializeField] Sprite EmptyHeart;
    //Hearts
    [SerializeField] Image[] P_Hearts; // 플레이어 체력바 이미지
    [SerializeField] public MonHpbarController MonsterHpbar; // 몬스터 HPbar
    //Upgrade
    [SerializeField] Image WeaponImage; // 무기 외형
    [SerializeField] TextMeshProUGUI WeaponLevelText; // 무기 레벨
    [SerializeField] TextMeshProUGUI AttText; // 무기 공격력
    //Weapon
    [SerializeField] Sprite NormalWeapon; // 노멀 무기(검)
    [SerializeField] Sprite HardWeapon; // 하드 무기(도끼)
    [SerializeField] GameObject PlayerWeapon; // 플레이어 무기
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
            player.playerdata.WEAPON += 1; // 무기 레벨
            player.playerdata.ATT += 2; // 무기 공격력
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
        //무기 강화
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
                SaveBinary(); // 게임 저장
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
            player.m_hp -= 1;//체력 마이너스
            P_Hearts[player.m_hp].sprite = EmptyHeart;
            _isPhit = false;
        }

        if (player.m_hp <= 0)
        {
            IngameManager.Instance.ChangeCurrentScene(SceneState.Finish);
        }//체력이 0이 되면 Finish Scene
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
    }//PlayerData가 없으면 새로생성 or 있으면 Data 읽어오기 + 시작한 후 읽어온 데이터로 꾸미기
    public void SaveBinary() // Finish가 되면 or 아이템 강화하면
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Open(FilePath, FileMode.OpenOrCreate);
        binaryFormatter.Serialize(fileStream, player.playerdata);
        fileStream.Close();
    }
#endregion
}
