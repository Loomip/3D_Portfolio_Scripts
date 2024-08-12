using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDieSound : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.PlaySfx(e_Sfx.EnemyDieEffectSound);
    }
}
