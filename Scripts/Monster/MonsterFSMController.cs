using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFSMController : MonoBehaviour
{
    // 몬스터의 현재 동작 중인 상태 컴포넌트
    [SerializeField] private MonsterState currentState;

    // 몬스터의 모든 상태 컴포넌트들
    [SerializeField] private MonsterState[] monsterStatas;

    // 플레이어 참조
    protected GameObject player;

    public GameObject Player { get => player; set => player = value; }

    // 배회 기준 위치 포인트들
    [SerializeField] private Transform[] wanderPoints;
    public Transform[] WanderPoints { get => wanderPoints; set => wanderPoints = value; }

    // 상태 전환 메소드
    public void TransactionToState(e_MonsterState state)
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
        TransactionToState(e_MonsterState.Hit);
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        // 대기 상태로 시작
        TransactionToState(e_MonsterState.Idle);

        wanderPoints = currentState.Ground.GetWanderPoints();
    }

    private void Update()
    {
        // 현재 설정된 상태의 기능을 동작
        currentState?.UpdateState();
    }
}
