using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFSMController : MonoBehaviour
{
    // 보스의 현재 동작 중인 상태 컴포넌트
    [SerializeField] private BossState currentState;

    // 보스의 모든 상태 컴포넌트들
    [SerializeField] private BossState[] monsterStatas;

    // 플레이어 참조
    protected GameObject player;

    public GameObject Player { get => player; set => player = value; }

    // 스킬 사용 중인지
    protected bool isSkill = false;

    public bool IsSkill { get => isSkill; set => isSkill = value; }

    // 상태 전환 메소드
    public void TransactionToState(e_BossState state)
    {
        currentState?.ExitState(); // 이전 상태 정리
        currentState = monsterStatas[(int)state]; // 상태 전환 처리
        currentState.EnterState(state); // 세로운 상태 전이
    }

    // 보조 컨트롤러 기능들

    // 플레이어와 몬스터간의 거리 측정
    public float GetPlayerDistance()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    // 플레이어에게 공격을 받음
    public void Hit()
    {
        // 현재 상태가 이미 사망한 상태면 피격 처리하지 않음
        if (currentState == monsterStatas[(int)e_MonsterState.Die]) return;

        // 피격 상태로 전환
        TransactionToState(e_BossState.Hit);
    }

    // 공격 에니메이션이 끝나면 알아서 공격 형태를 바꿀 메소드
    public void OnAnimationEnd()
    {
        TransactionToState(e_BossState.Idle);
        IsSkill = false;
    }


    void Start()
    {
        player = GameObject.FindWithTag("Player");

        // 대기 상태로 시작
        TransactionToState(e_BossState.Idle);
    }

    private void Update()
    {
        // 현재 설정된 상태의 기능을 동작
        currentState?.UpdateState();
    }
}
