using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.PlayBgm(e_Bgm.Start);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}


