using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossGround : MonoBehaviour
{
    // 보스 소환 이팩트
    [SerializeField] private GameObject bossEffectPrefab;
    // 보스 몬스터
    [SerializeField] private GameObject bossPrefabs;
    // 문 오브젝트
    [SerializeField] private GameObject doorin;
    // Player를 탐지할 BoxCollider
    [SerializeField] private BoxCollider zone;
    // 보스 방 진입시 등장할 HP
    [SerializeField] private GameObject bossHp;

    // 보스 소환 위치
    [SerializeField] private Vector3 spawnAreaCenter;

    // 보스 처치 후
    [SerializeField] private GameObject Ending;

    // 맵 재시작 방지 
    private bool isGroundStart = false;

    public bool IsGroundStart { get => isGroundStart; set => isGroundStart = value; }

    public void MonsterDied()
    {
        doorin.SetActive(false); // 문을 염
        IsGroundStart = false;
        // 클리어 될때 나올 기능
        Ending.SetActive(true);
    }

    IEnumerator AppearEffectAndSpawn()
    {
        //이팩트 소환
        GameObject SummonsEffect = Instantiate(bossEffectPrefab, spawnAreaCenter, bossEffectPrefab.transform.rotation);

        yield return new WaitForSeconds(1f);

        //보스 소환
        GameObject BossSummons = Instantiate(bossPrefabs, spawnAreaCenter, Quaternion.Euler(0, -90, 0));

        Destroy(SummonsEffect, 5f);

        doorin.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !IsGroundStart)
        {
            string CharacterName = bossPrefabs.GetComponent<CharacterState>().CharacterName;

            StartCoroutine(AppearEffectAndSpawn());
            IsGroundStart = true;
            bossHp.SetActive(true);
            bossHp.GetComponentInChildren<TextMeshProUGUI>().text = CharacterName;
        }
    }
}
