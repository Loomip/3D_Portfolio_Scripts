using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    // 캐릭터의 능력치
    [SerializeField] private CharacterState state;

    public CharacterState State { get => state; set => state = value; }

    // 체력을 담을 변수
    public int hp
    {
        get => State.Hp;
        set => State.Hp = value;
    }

    public int mp
    {
        get => State.Mp;
        set => State.Mp = value;
    }

    // 방어력 계산
    private int def
    {
        get => State.Def; 
        set => State.Def = value;
    }

    // 데미지 쿨시간 
    [SerializeField] private float damageCooldown;

    // 맞았는지 
    private bool canTakeDamage = true;

    public bool CanTakeDamage { get => canTakeDamage; set => canTakeDamage = value; }

    public abstract void Hit(int damage);


    public IEnumerator IsHitCoroutine(int damage)
    {
        CanTakeDamage = false;

        // 대미지가 들어온 만큼 체력을 깍음
        int realDamage = Mathf.Max(0, damage - def);
        hp -= realDamage;

        // UImanager에서 체력 리프레쉬
        UIManager.instance.RefreshHp(gameObject.tag, this);

        yield return new WaitForSeconds(damageCooldown);

        CanTakeDamage = true;
    }
}
