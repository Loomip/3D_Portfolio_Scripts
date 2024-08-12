using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHitState : MonsterAttackableState
{
    // 피격 파티클
    [SerializeField] protected ParticleSystem hitParticle;

    IEnumerator DamagerCoolDoun()
    {
        Material[] materialsCopy = meshs.materials;

        // 각 머티리얼의 색상을 변경
        for (int i = 0; i < materialsCopy.Length; i++)
        {
            materialsCopy[i].color = Color.red;
        }

        meshs.materials = materialsCopy;

        // 맞는 사운드
        SoundManager.instance.PlaySfx(e_Sfx.Hit);

        // 피격효과 재생
        hitParticle.Play();

        yield return new WaitForSeconds(0.2f);

        materialsCopy = meshs.materials;

        // 각 머티리얼의 색상을 변경
        for (int i = 0; i < materialsCopy.Length; i++)
        {
            materialsCopy[i].color = Color.white;
        }

        meshs.materials = materialsCopy;
    }

    public override void EnterState(e_MonsterState monsterState)
    {
        if (monsterHp.CanTakeDamage)
        {
            // 이동 중지
            nav.isStopped = true;

            StartCoroutine(DamagerCoolDoun());
        }

        if (monsterHp.hp <= 0)
        {
            // 사망 상태로 전환
            controller.TransactionToState(e_MonsterState.Die);
            return;
        }
    }

    public override void UpdateState()
    {
        if (state.Hp <= 0)
        {
            controller.TransactionToState(e_MonsterState.Die);
            return;
        }

        if (monsterHp.CanTakeDamage == false) return;

        // 플레이어가 공격 가능 거리안에 들어왔다면
        if (controller.GetPlayerDistance() <= attackDistance)
        {
            // 공격 상태로 전환
            controller.TransactionToState(e_MonsterState.Attack);
            return;
        }

        if (controller.GetPlayerDistance() <= detactDistance)
        {
            // 인식 범위에 들어온 경우
            nav.speed = state.Spd;

            nav.SetDestination(controller.Player.transform.position);
            return;
        }

        if (controller.GetPlayerDistance() > attackDistance)
        {
            controller.TransactionToState(e_MonsterState.Run);
            return;
        }
    }

    public override void ExitState()
    {
        StopCoroutine(DamagerCoolDoun());
    }
}
