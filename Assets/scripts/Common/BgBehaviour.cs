using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgBehaviour : MonoBehaviour
{
    public int scale = 2;
    void Awake()
    {
        this.transform.localScale = new Vector3(Screen.width * scale, Screen.height * scale, 1);
    }

}
