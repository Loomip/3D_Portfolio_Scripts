using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackEvent : MonoBehaviour
{
    // 근접 스킬
    [SerializeField] private GameObject meleeSkill;
    // 원거리 스킬
    [SerializeField] private GameObject rangedSkill;
    // 스킬이 나갈 위치
    [SerializeField] private Transform skillPos;

    //플레이어 애니메이션 이벤트
    void MeleeSkill()
    {
        GameObject rangeInstant = Instantiate(meleeSkill, skillPos.position, meleeSkill.transform.rotation);
        Effect effect = rangeInstant.GetComponent<Effect>();
        CharacterState state = GetComponentInParent<CharacterState>();
        effect.Atk = state.Atk;
    }

    void RangedSkill()
    {
        GameObject rangeInstant = Instantiate(rangedSkill, skillPos.position, rangedSkill.transform.rotation);
        Rigidbody bulletRigid = rangeInstant.GetComponent<Rigidbody>();
        bulletRigid.velocity = skillPos.forward * 10f;
        Effect effect = rangeInstant.GetComponent<Effect>();
        CharacterState state = GetComponentInParent<CharacterState>();
        effect.Atk = state.Atk;
    }
}
