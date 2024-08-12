using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterRunState : MonsterAttackableState
{
    // 랜덤으로 돌아다닐 기준 위치점
    public Vector3 targetPosition = Vector3.positiveInfinity;

    // 기준 위치점으로 랜덤으로 돌아 다닐 위치
    private Vector3 randomDestination;

    // 이동 간격
    [SerializeField] private float moveInterval;

    // Player를 인식 했는지
    [SerializeField] private bool hasPlayerRecognition = false;

    Transform FindClosest(Transform currentPosition, Transform[] transforms)
    {
        Transform closestTransform = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform transform in transforms)
        {
            float distance = Vector3.Distance(currentPosition.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTransform = transform;
            }
        }

        return closestTransform;
    }


    // NavMesh 위의 무작위 위치를 반환하는 메소드
    Vector3 RandomNavmeshLocation(Vector3 center)
    {
        // center를 중심으로 반경 10f 내의 무작위 방향을 생성
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += center;

        // NavMeshHit 구조체를 초기화
        NavMeshHit navHit;

        // 무작위 방향의 위치에서 가장 가까운 NavMesh의 위치를 찾음
        // 찾은 위치는 navHit에 저장되며, 최대 거리는 10f, 모든 NavMesh 레이어를 대상
        NavMesh.SamplePosition(randomDirection, out navHit, 10f, NavMesh.AllAreas);

        // 찾은 NavMesh의 위치를 반환
        return navHit.position;
    }

    private void NewDerection()
    {
        Transform[] transforms = controller.WanderPoints;
        Transform currentPosition = transform;
        Transform closestTransform = FindClosest(currentPosition, transforms);

        // 가장 가까운 배회 포인트를 기준으로 무작위 위치를 선택
        randomDestination = RandomNavmeshLocation(closestTransform.position);

        // 선택된 위치로 이동
        nav.SetDestination(randomDestination);
    }

    public override void EnterState(e_MonsterState monsterState)
    {
        nav.speed = state.Spd;

        animator.SetInteger("State", (int)monsterState);

        nav.isStopped = false;

        NewDerection();
    }

    public override void UpdateState()
    {
        if (state.Hp <= 0)
        {
            controller.TransactionToState(e_MonsterState.Die);
            return;
        }

        // 플레이어가 공격 가능 거리안에 들어왔다면
        if (controller.GetPlayerDistance() <= attackDistance)
        {
            // 공격 상태로 전환
            controller.TransactionToState(e_MonsterState.Attack);
            return;
        }

        if (controller.GetPlayerDistance() <= detactDistance)
        {
            // 공격 대상 추적 처리
            nav.isStopped = false;
            nav.SetDestination(controller.Player.transform.position);
            hasPlayerRecognition = false;
            return;
        }

        if (controller.GetPlayerDistance() > detactDistance)
        {
            if (!hasPlayerRecognition)
            {
                NewDerection();
                hasPlayerRecognition = true;
            }

            if (Vector3.Distance(transform.position, randomDestination) < 0.1f)
            {
                controller.TransactionToState(e_MonsterState.Idle);
                return;
            }
        }
    }

    public override void ExitState()
    {

    }


}
