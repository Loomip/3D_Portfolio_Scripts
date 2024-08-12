using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackableState : MonsterState
{
    [SerializeField] protected float attackDistance; // 플레이어 공격 가능 거리
    [SerializeField] protected float detactDistance; // 플레이어 추적 가능 거리

    public override void EnterState(e_MonsterState state)
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
}
