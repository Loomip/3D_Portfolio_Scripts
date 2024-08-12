using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackableState : BossState
{
    [SerializeField] protected float attackDistance; // 플레이어 공격 가능 거리
    [SerializeField] protected float detactDistance; // 플레이어 추적 가능 거리

    // 회전 보간 수치
    [SerializeField] protected float smoothValue;

    // 공격력을 담을 변수
    public int atk
    {
        get => state.Atk;
        set => state.Atk = value;
    }

    public override void EnterState(e_BossState state)
    {

    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }
}
