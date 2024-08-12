using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossRunState : BossAttackableState
{
    // 현재 발동된 스킬 인덱스
    private int skillIndex;

    // 대기 상태 시작(진입) 처리 (상태 초기화)
    public override void EnterState(e_BossState state)
    {
        // 네비게이션 에이전트 이동
        nav.isStopped = false;

        // 애니메이션 재생
        animator.SetInteger("State", (int)state);
    }

    // 대기 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        if (state.Hp <= 0)
        {
            controller.TransactionToState(e_BossState.Die);
            return;
        }

        if (controller.GetPlayerDistance() <= attackDistance)
        {
            int selectState;

            do
            {
                selectState = Random.Range(0, 3); // 공격 패턴 랜덤 재생
            }
            while (selectState == skillIndex); // 이전에 선택한 스킬과 다른 스킬이 선택될 때까지 반복

            skillIndex = selectState; // 선택한 스킬 인덱스 저장

            switch (selectState)
            {
                case 0:
                    // 기본 공격
                    controller.TransactionToState(e_BossState.Attack);
                    return;
                case 1:
                    // 스킬 1번
                    controller.TransactionToState(e_BossState.Skill1);
                    return;
                case 2:
                    // 스킬 2번
                    controller.TransactionToState(e_BossState.Skill2);
                    return;
            }
        }

        if(controller.GetPlayerDistance() > detactDistance)
        {
            // 공격 대상 추적 처리
            nav.SetDestination(controller.Player.transform.position);
        }
    }

    // 대기 상태 종료(다른상태로 전이) 동작 처리(상태 정리)
    public override void ExitState()
    {

    }
}
