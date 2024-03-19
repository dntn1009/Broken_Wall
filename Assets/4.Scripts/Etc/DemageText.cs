using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemageText : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down / 2f;
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
