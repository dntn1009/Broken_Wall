using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    #region Effect
    [SerializeField] GameObject HitEffect;
    [SerializeField] GameObject DemageText;
    [SerializeField] public Canvas canvas;
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Monster"))
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 1) * 10f; // 몬스터를 팅겨내서 위로 올라감
            collision.gameObject.GetComponent<MonsterController>().SetDemage(transform.parent.GetComponent<PlayerController>().playerdata.ATT);
            collision.gameObject.GetComponent<MonsterController>().SettingHpBar();
            Instantiate(HitEffect, new Vector2(0, collision.transform.position.y - 0.6f), Quaternion.identity);
            Instantiate(DemageText, Camera.main.WorldToScreenPoint(collision.transform.position + Vector3.right * 2f), Quaternion.identity, canvas.transform);
            DemageText.GetComponent<TextMeshProUGUI>().text = transform.parent.GetComponent<PlayerController>().playerdata.ATT.ToString();
        }
    }
}
