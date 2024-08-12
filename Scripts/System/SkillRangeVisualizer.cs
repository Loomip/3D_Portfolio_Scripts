using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRangeVisualizer : MonoBehaviour
{
    public Transform skillPos;
    public float skillAttackRadius;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(skillPos.position, skillAttackRadius);
    }
}
