using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyController : MonoBehaviour
{
    
    public void DestroyObject()
    {
        Destroy(this.transform.gameObject);
    }
    public void DestroyObject_float(float destroy_time)
    {
        Destroy(this.transform.gameObject, destroy_time);
    }
}
