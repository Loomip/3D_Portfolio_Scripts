using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    // 나중에 확장성은 여기다가 사냥터 배열을 써서 쭈루룩 할 예정
    [SerializeField] HuntingGround huntingGround;
    [SerializeField] BossGround bossGround;

    public HuntingGround HuntiogGround { get => huntingGround; set => huntingGround = value; }
    public BossGround BossGround { get => bossGround; set => bossGround = value; }
}
