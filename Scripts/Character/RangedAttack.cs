using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RangedAttack : AttackController
{
    // 총알이 발사될 위치
    [SerializeField] private Transform bulletPos;
    // 총알 프리펩
    [SerializeField] private GameObject bullet;

    protected override void Update()
    {
        if (Input.GetMouseButtonDown(1) && isSkillCooldown)
        {
            // UI가 켜져있으면 공격하지 못하게 막음
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (health.mp >= 0)
            {
                StartCoroutine(isSkill());
            }
        }
    }

    public void Shot()
    {
        // 총알 생성
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        Bullet bullets = intantBullet.GetComponent<Bullet>();

        SoundManager.instance.PlaySfx(e_Sfx.Bullet);

        // 총알 발사 속도
        bulletRigid.velocity = bulletPos.forward * 20f;

        // 총알에게 공격력을 전해줌
        bullets.Atk = attackPower;
    }
}
