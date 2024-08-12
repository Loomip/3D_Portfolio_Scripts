using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterDeathState : MonsterState
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Rigidbody[] rigidbodies;
    [SerializeField] private Transform regdollPosition;

    // 사망 완료 처리 시간
    [SerializeField] protected float time;
    [SerializeField] protected float deathDelayTime;

    // 사망 처리 이펙트
    [SerializeField] protected GameObject destroyParticlePrefab;

    // 이펙트 생성 위치
    [SerializeField] protected Transform effectPos;


    private void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableRegdoll();
    }

    public void EnableRegdoll()
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = false;
            rigidbodies[i].velocity = Vector3.zero;
        }
    }

    public void DisableRegdoll()
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = true;
        }
    }

    public override void EnterState(e_MonsterState monsterState)
    {
        // 이동 중지
        nav.isStopped = true;

        gameObject.layer = 0;

        SoundManager.instance.PlaySfx(e_Sfx.EnemyDie);

        animator.enabled = false;

        EnableRegdoll();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.AddExplosionForce(10, regdollPosition.position, 20f, 5f, ForceMode.Impulse);
        }

        InventoryManager.instance.gold += 100;
        InventoryManager.instance.Refresh_Gold();
    }

    public override void UpdateState()
    {
        time += Time.deltaTime;

        // 사망 처리 지연시간이 지났다면
        if (time >= deathDelayTime)
        {
            huntingGround.MonsterDied(this);
            Instantiate(destroyParticlePrefab, effectPos.position, destroyParticlePrefab.transform.rotation);
            Destroy(gameObject);
        }
    }

    public override void ExitState()
    {
        
    }
}
