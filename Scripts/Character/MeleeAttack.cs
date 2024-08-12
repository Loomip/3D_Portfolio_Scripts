using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeleeAttack : AttackController
{
    // 공격 타겟 중심점 위치
    [SerializeField] private Transform attackTransfom;

    // 공격 범위
    [SerializeField] private float attackRadius;

    // 공격 범위 각도
    [SerializeField] private float hitAngle;

    protected override void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // UI가 켜져있으면 공격하지 못하게 막음
            if (EventSystem.current.IsPointerOverGameObject()) return;
            animator.SetTrigger("isAttack");
        }
    }

    // 공격 애니메이션 피격 이벤트
    public void AttackHitAnimationEvent()
    {
        // Physics.OverlapSphere(충돌체크중심점위치, 충돌체크범위, 대상레이어);
        //  - 레이캐스트처럼 해당 메소드가 실행되는 순간 설정 영역안에 있는 충돌 대상들을 검출함
        Collider[] hits = Physics.OverlapSphere(attackTransfom.position, attackRadius, targetLayer);

        SoundManager.instance.PlaySfx(e_Sfx.Sword);

        // 피격된 대상들 중 지정된 각도 안에 있는 대상을 타격함
        foreach (Collider hit in hits)
        {
            // 플레이어가 타격을 향한 방향 벡터를 구함
            Vector3 directionToTargert = hit.transform.position - transform.position;

            // 타격 대상과의 시선 각도를 구함
            float angleToTarget = Vector3.Angle(transform.forward, directionToTargert);

            if (angleToTarget < hitAngle)
            {
                if (hit.tag == "Enemy")
                {
                    hit.GetComponent<MonsterFSMController>().Hit();
                    hit.GetComponent<Health>().Hit(attackPower);
                }

                if (hit.tag == "Boss")
                {
                    hit.GetComponent<BossFSMController>().Hit();
                    hit.GetComponent<Health>().Hit(attackPower);
                }
            }

        }
    }


}
