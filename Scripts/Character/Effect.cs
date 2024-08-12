using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Effect : MonoBehaviour
{
    // 스킬 공격력을 전달받을 계수
    private int atk;
    public int Atk { get => atk; set => atk = value; }

    // 효과음을 나눌 Enum 타입
    [SerializeField] e_SkillSound skillSound;

    // 스킬 지속시간
    [SerializeField] private float duration;

    private void Start()
    {
        if (skillSound == e_SkillSound.melee)
        {
            SoundManager.instance.PlaySfx(e_Sfx.SwordSkill);
        }
        else if (skillSound == e_SkillSound.Ranged)
        {
            SoundManager.instance.PlaySfx(e_Sfx.BulletSkill);
        }

        // 일정시간뒤 시라짐
        StartCoroutine(DestroyAfterTime(duration));
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            var monsterController = other.GetComponent<MonsterFSMController>();
            var health = other.GetComponent<Health>();
            if (monsterController != null && health != null)
            {
                monsterController.Hit();
                health.Hit(Atk);
            }
        }

        if (other.tag == "Boss")
        {
            var bossController = other.GetComponent<BossFSMController>();
            var health = other.GetComponent<Health>();
            if (bossController != null && health != null)
            {
                bossController.Hit();
                health.Hit(Atk);
            }
        }
    }

    IEnumerator DestroyAfterTime(float time)
    {
        
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
