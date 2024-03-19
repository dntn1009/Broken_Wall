using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixScreenPixel : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID
        Screen.SetResolution(Screen.width, Screen.width * 16 / 9, true);
#endif
    }

}
