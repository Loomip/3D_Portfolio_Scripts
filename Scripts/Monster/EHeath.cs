using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EHeath : Health
{
    [SerializeField] private GameObject healthBarPrefab;
    private GameObject healthBarInstance; // 체력바 인스턴스를 추적하기 위한 변수

    public GameObject HealthBarPrefab { get => healthBarPrefab; set => healthBarPrefab = value; }

    public override void Hit(int damage)
    {
        if (hp > 0 && CanTakeDamage)
        {
            // 대미지 효과
            StartCoroutine(IsHitCoroutine(damage));
        }
        else if (hp <= 0)
        {
            Destroy(healthBarInstance);
        }
    }

    void Start()
    {
        if (gameObject.tag == "Enemy")
        {
            healthBarInstance = Instantiate(HealthBarPrefab, transform.position + Vector3.up * 2, Quaternion.identity, transform);
            Slider healthBar = healthBarInstance.GetComponentInChildren<Slider>();
            TextMeshProUGUI name = healthBarInstance.GetComponentInChildren<TextMeshProUGUI>();
            name.text = State.CharacterName;
            UIManager.instance.RegisterEnemyHealthBar(this, healthBar);
        }
    }
}
