using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRangedAttackState : MonsterAttackableState
{
    // 총알이 발사될 위치
    [SerializeField] private Transform bulletPos;

    // 총알 프리펩
    [SerializeField] private GameObject bullet;

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

    public void Shot()
    {
        // 총알 생성
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        Bullet bullets = intantBullet.GetComponent<Bullet>();

        // 총알 발사 속도
        bulletRigid.velocity = bulletPos.forward * 20f;

        bullets.Atk = atk;
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
