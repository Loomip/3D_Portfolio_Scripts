using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PHealth : Health
{
    private Animator animator;

    // 히트 되면 바뀔 몸 메터리얼
    protected SkinnedMeshRenderer meshs;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Rigidbody[] rigidbodies;
    [SerializeField] private Transform regdollPosition;

    [SerializeField] private GameObject Ending;

    private void Start()
    {
        meshs = GetComponentInChildren<SkinnedMeshRenderer>();
        animator = GetComponent<Animator>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableRegdoll();
    }

    public override void Hit(int damage)
    {
        if (hp > 0 && CanTakeDamage)
        {
            // 대미지 효과
            StartCoroutine(IsHitCoroutine(damage));
            StartCoroutine(DamagerCoolDoun());
        }
        else if (hp <= 0)
        {
            // 죽음
            animator.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            EnableRegdoll();

            foreach (Rigidbody rb in rigidbodies)
            {
                rb.AddExplosionForce(10, regdollPosition.position, 20f, 5f, ForceMode.Impulse);
            }

            Ending.SetActive(true);
        }
    }

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

        yield return new WaitForSeconds(0.2f);

        materialsCopy = meshs.materials;

        // 각 머티리얼의 색상을 변경
        for (int i = 0; i < materialsCopy.Length; i++)
        {
            materialsCopy[i].color = Color.white;
        }

        meshs.materials = materialsCopy;
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
}
