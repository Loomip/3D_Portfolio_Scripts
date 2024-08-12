using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeathState : BossAttackableState
{
    // 사망 완료 처리 시간
    protected float time;
    [SerializeField] protected float deathDelayTime;
    
    // 사망 처리 이펙트
    [SerializeField] protected GameObject destroyParticlePrefab;

    public override void EnterState(e_BossState state)
    {
        // 이동 중지
        nav.isStopped = true;

        // 애니메이션 재생
        animator.SetInteger("State", (int)state);

        animator.SetBool("Die", true);
    }

    public override void UpdateState()
    {
        time += Time.deltaTime;

        // 사망 처리 지연시간이 지났다면
        if (time >= deathDelayTime)
        {
            // 몬스터가 소멸됨

            bossGround.MonsterDied();
            Instantiate(destroyParticlePrefab, transform.position, destroyParticlePrefab.transform.rotation);
            Destroy(gameObject);
        }
    }

    public override void ExitState()
    {

    }
}
