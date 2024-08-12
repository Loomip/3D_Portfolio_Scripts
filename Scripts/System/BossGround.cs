using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossGround : MonoBehaviour
{
    // ���� ��ȯ ����Ʈ
    [SerializeField] private GameObject bossEffectPrefab;
    // ���� ����
    [SerializeField] private GameObject bossPrefabs;
    // �� ������Ʈ
    [SerializeField] private GameObject doorin;
    // Player�� Ž���� BoxCollider
    [SerializeField] private BoxCollider zone;
    // ���� �� ���Խ� ������ HP
    [SerializeField] private GameObject bossHp;

    // ���� ��ȯ ��ġ
    [SerializeField] private Vector3 spawnAreaCenter;

    // ���� óġ ��
    [SerializeField] private GameObject Ending;

    // �� ����� ���� 
    private bool isGroundStart = false;

    public bool IsGroundStart { get => isGroundStart; set => isGroundStart = value; }

    public void MonsterDied()
    {
        doorin.SetActive(false); // ���� ��
        IsGroundStart = false;
        // Ŭ���� �ɶ� ���� ���
        Ending.SetActive(true);
    }

    IEnumerator AppearEffectAndSpawn()
    {
        //����Ʈ ��ȯ
        GameObject SummonsEffect = Instantiate(bossEffectPrefab, spawnAreaCenter, bossEffectPrefab.transform.rotation);

        yield return new WaitForSeconds(1f);

        //���� ��ȯ
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
