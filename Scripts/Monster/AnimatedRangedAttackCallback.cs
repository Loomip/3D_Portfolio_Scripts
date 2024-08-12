using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedRangedAttackCallback : MonoBehaviour
{
    [SerializeField] private MonsterRangedAttackState shotAttackState;

    public void ShotAnimationEvent()
    {
        shotAttackState.Shot();
    }
}
