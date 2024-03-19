using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BosterController : MonoBehaviour
{
    [SerializeField] GameObject DemageText;
    public Canvas canvas;

    private void Update()
    {
        if (transform.position.y < 41f)
            transform.Translate(Vector3.up * 25f * Time.deltaTime);
        else
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            collision.gameObject.GetComponent<MonsterController>().SetDemage(99);
            collision.gameObject.GetComponent<MonsterController>().SettingHpBar();
            Instantiate(DemageText, Camera.main.WorldToScreenPoint(collision.transform.position + Vector3.right * 2f), Quaternion.identity, canvas.transform);
            DemageText.GetComponent<TextMeshProUGUI>().text = "99";
        }
    }
}
