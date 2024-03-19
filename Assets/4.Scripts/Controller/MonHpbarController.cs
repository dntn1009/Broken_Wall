using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonHpbarController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI MonsterName;
    [SerializeField] public TextMeshProUGUI HPText;
    [SerializeField] public Slider HPBAR;

    public void SetHpbar(string name, int hp, int hpmax)
    {
        MonsterName.text = name;
        HPText.text = hp + " / " + hpmax;
        float normalizedHP = hp / (float)hpmax;
        if (normalizedHP <= 0f)
        {
            HPBAR.value = 0f;
        }
        else
            HPBAR.value = normalizedHP;
        gameObject.SetActive(true);
        Invoke("SetActiveFalse", 0.5f);
    }

    void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
