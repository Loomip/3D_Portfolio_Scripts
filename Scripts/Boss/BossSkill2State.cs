using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill2State : BossAttackableState
{
    [SerializeField] private GameObject Skill_2;
    [SerializeField] private Transform bulletPos3;

    IEnumerator ExecutePattern3()
    {
        GameObject rangeInstant = Instantiate(Skill_2, bulletPos3.position, bulletPos3.rotation);
        var evt = rangeInstant.GetComponentInChildren<ExplosionEvent>();
        evt.SetOwner(gameObject);

        yield return new WaitForSeconds(8f);
        DestroyImmediate(rangeInstant, true);
    }

    public void BossSkill_2()
    {
        StartCoroutine(ExecutePattern3());
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


    // 대기 상태 시작(진입) 처리 (상태 초기화)
    public override void EnterState(e_BossState state)
    {
        // 애니메이션 재생
        animator.SetInteger("State", (int)state);
        controller.IsSkill = true;
        SoundManager.instance.PlaySfx(e_Sfx.BossSkill2);
    }

    // 대기 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        if (state.Hp <= 0)
        {
            controller.TransactionToState(e_BossState.Die);
            return;
        }

        LookAtTarget();

        if (controller.IsSkill == false)
        {
            if (controller.GetPlayerDistance() > detactDistance)
            {
                controller.TransactionToState(e_BossState.Idle);
                return;
            }
        }
    }

    // 대기 상태 종료(다른상태로 전이) 동작 처리(상태 정리)
    public override void ExitState()
    {

    }
}
