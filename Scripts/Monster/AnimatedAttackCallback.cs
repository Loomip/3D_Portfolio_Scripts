using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedAttackCallback : MonoBehaviour
{
    [SerializeField] MonsterMeleeAttackState meleeAttackState;

    public void MeleeAnimationEvent()
    {
        meleeAttackState.Attack();
    }
}
