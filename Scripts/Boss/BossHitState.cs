using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitState : BossAttackableState
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

    public override void EnterState(e_BossState monsterState)
    {
        if (monsterHp.CanTakeDamage)
        {
            StartCoroutine(DamagerCoolDoun());
        }

        if (monsterHp.hp <= 0)
        {
            // 사망 상태로 전환
            controller.TransactionToState(e_BossState.Die);
            return;
        }
    }

    public override void UpdateState()
    {
        if (state.Hp <= 0)
        {
            controller.TransactionToState(e_BossState.Die);
            return;
        }

        if (monsterHp.CanTakeDamage == false) return;

        if (controller.IsSkill == false)
        {
            if (controller.GetPlayerDistance() <= attackDistance)
            {
                controller.TransactionToState(e_BossState.Run);
            }


            if (controller.GetPlayerDistance() > detactDistance)
            {
                controller.TransactionToState(e_BossState.Run);
            }
        }


    }

    public override void ExitState()
    {
        StopCoroutine(DamagerCoolDoun());
    }
}
