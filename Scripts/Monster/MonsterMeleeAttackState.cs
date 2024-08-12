using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMeleeAttackState : MonsterAttackableState
{
    // 공격 타겟 중심점 위치
    [SerializeField] private Transform attackTransfom;

    // 공격 범위
    [SerializeField] private float attackRadius;

    // 공격 범위 각도
    [SerializeField] private float hitAngle;

    // 공격 대상 레이어
    [SerializeField] protected LayerMask targetLayer;

    // 회전 보간 수치
    [SerializeField] protected float smoothValue;

    // 공격력을 담을 변수
    private int atk
    {
        get => state.Atk;
        set => state.Atk = value;
    }

    // 공격 대상을 주시
    protected void LookAtTarget()
    {
        // 공격 대상을 향한 방향을 계산
        Vector3 direction = (controller.Player.transform.position - transform.position).normalized;

        // 회전 쿼터니언 계산
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

        // 보간 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * smoothValue);
    }

    public void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(attackTransfom.position, attackRadius, targetLayer);

        foreach (Collider hit in hits)
        {
            Vector3 directionToTargert = hit.transform.position - transform.position;

            float angleToTarget = Vector3.Angle(transform.forward, directionToTargert);

            if (angleToTarget < hitAngle)
            {
                // Player 피격 처리
                if (hit.tag == "Player")
                {
                    hit.GetComponent<Health>().Hit(atk);
                }
            }
        }
    }

    public override void EnterState(e_MonsterState monsterState)
    {
        nav.isStopped = true;
        nav.speed = 0f;

        animator.SetInteger("State", (int)monsterState);
    }

    public override void UpdateState()
    {
        if (state.Hp <= 0)
        {
            controller.TransactionToState(e_MonsterState.Die);
            return;
        }

        if (controller.GetPlayerDistance() > attackDistance)
        {
            controller.TransactionToState(e_MonsterState.Run);
            return;
        }

        LookAtTarget();
    }

    public override void ExitState()
    {

    }
}
