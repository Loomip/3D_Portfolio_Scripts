using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingGround : MonoBehaviour
{
    // ���Ͱ� ��ȸ�� ��ġ�� �Ѱ��� ��ȸ��ġ
    [SerializeField] private Transform[] wanderPoints;
    public Transform[] WanderPoints { get => wanderPoints; set => wanderPoints = value; }

    // ���� ��ȯ ����Ʈ
    [SerializeField] private List<GameObject> effectPrefabs;
    // �������� ������ ����
    [SerializeField] private List<GameObject> monsterPrefabs;
    // Player�� Ž���� BoxCollider
    [SerializeField] private BoxCollider zone;
    // �Ա� ���� �ݶ��̴�
    [SerializeField] private GameObject incollider;
    [SerializeField] private GameObject outcollider;
    // ��ȯ ����Ʈ�� ��Ÿ���� �ð�
    [SerializeField] private float effectAppearTime = 2.0f;

    // ���� ���� ���� �߽� ��ġ
    [SerializeField] private Vector3 spawnAreaCenter;
    // ���� ���� ���� ũ��
    [SerializeField] private Vector3 spawnAreaSize;

    // Ŭ�����ϱ� ���� �ʿ��� ���� ��
    [SerializeField] private int monstersToClear = 10; 

    // ��ȯ �� ���͸� ��� Ŭ��� �Ǵ��� ����Ʈ
    [SerializeField] private List<GameObject> enemyList = new List<GameObject>();

    // �� ����� ���� 
    private bool isGroundStart = false;
    public bool IsGroundStart { get => isGroundStart; set => isGroundStart = value; }

    // ���� Ŭ����� ���¸� ����
    private bool isCleared = false;

 


    // ���Ϳ��� ��ȸ��ġ�� �Ѱ��� �޼ҵ�
    public Transform[] GetWanderPoints()
    {
        return wanderPoints;
    }

    // ��� �� ��ȯ ����
    Vector3 GetRandomSpawnPosition()
    {
        float radius = spawnAreaSize.x; // ���� �������� spawnAreaSize�� ����
        float randomRadius = UnityEngine.Random.Range(0f, radius); // 0���� ������ ������ ������ ������ �Ÿ�
        float randomAngle = UnityEngine.Random.Range(0f, 2 * Mathf.PI); // 0���� 2�� ������ ������ ����

        // ���� ������ ��ġ ��� (y ��ǥ�� ����)
        float x = randomRadius * Mathf.Cos(randomAngle);
        float z = randomRadius * Mathf.Sin(randomAngle);

        // ���� ��ǥ�� ���� ��ǥ�� ��ȯ
        Vector3 localPosition = new Vector3(x, spawnAreaCenter.y, z);
        Vector3 worldPosition = transform.TransformPoint(localPosition);

        return worldPosition;
    }

    IEnumerator SpawnEffectAndMonster()
    {
        while (GetEnemyCount() < monstersToClear)
        {
            // ���� ����Ʈ ���� �ڵ�
            GameObject selectedEffectPrefab = effectPrefabs[UnityEngine.Random.Range(0, effectPrefabs.Count)];

            // ���� ���� ���� �ڵ�
            GameObject selectedMonsterPrefab = monsterPrefabs[UnityEngine.Random.Range(0, monsterPrefabs.Count)];

            // ���� ���� ��ġ ���
            Vector3 randomSpawnPosition = GetRandomSpawnPosition();

            // ����Ʈ ����
            GameObject effectInstance = Instantiate(selectedEffectPrefab, randomSpawnPosition, selectedEffectPrefab.transform.rotation);

            // ���� ����
            GameObject monsterInstance = Instantiate(selectedMonsterPrefab, randomSpawnPosition, Quaternion.identity);

            Destroy(effectInstance, 5f);

            // MonsterState ��ũ��Ʈ�� SetHuntingGroundController �޼��带 ȣ���Ͽ� HuntingGround �ν��Ͻ� ����
            MonsterState enemy = monsterInstance.GetComponent<MonsterState>();
            if (enemy != null)
            {
                enemy.SetHuntingGroundController(this);

                enemyList.Add(enemy.gameObject);

                Debug.Log("currentMonsterCount : " + GetEnemyCount());
            }

            if (GetEnemyCount() == monstersToClear)
            {
                StopCoroutine(AppearEffectAndSpawn());
            }

            yield return null;
        }
    }

    public void MonsterDied(MonsterState _enemy)
    {
        if (enemyList.Contains(_enemy.gameObject))
        {
            enemyList.Remove(_enemy.gameObject);
        }
        else
        {
            Debug.Log("The enemy is not in the list.");
        }

        if (GetEnemyCount() <= 0)
        {
            // ��� ���Ͱ� ����Ͽ� Ŭ���� ����
            isGroundStart = false;

            incollider.SetActive(false);
            outcollider.SetActive(false);
            isCleared = true;
        }

        Debug.Log("DiedcurrentMonsterCount : " + GetEnemyCount());
    }

    public int GetEnemyCount() => enemyList.Count;

    IEnumerator AppearEffectAndSpawn()
    {
        incollider.SetActive(true);
        outcollider.SetActive(true);

        float elapsedTime = 0f;
        float startScale = 0.1f;
        float targetScale = 1f;

        // ����Ʈ ��Ÿ���� �ִϸ��̼�
        while (elapsedTime < effectAppearTime)
        {
            float t = elapsedTime / effectAppearTime;
            float scale = Mathf.Lerp(startScale, targetScale, t);

            foreach (GameObject effectPrefab in effectPrefabs)
            {
                effectPrefab.transform.localScale = new Vector3(scale, scale, scale);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(SpawnEffectAndMonster());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !IsGroundStart && !isCleared)
        {
            StartCoroutine(AppearEffectAndSpawn());
            IsGroundStart = true;
        }
    }

    // �� ��ȯ ���� ǥ��
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float theta = 0;
        float x = spawnAreaSize.x * Mathf.Cos(theta);
        float z = spawnAreaSize.x * Mathf.Sin(theta);

        // ���� ��ǥ�� ���� ��ǥ�� ��ȯ
        Vector3 pos = transform.TransformPoint(spawnAreaCenter + new Vector3(x, 0, z));
        Vector3 newPos = pos;
        Vector3 lastPos = pos;

        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = spawnAreaSize.x * Mathf.Cos(theta);
            z = spawnAreaSize.x * Mathf.Sin(theta);

            // ���� ��ǥ�� ���� ��ǥ�� ��ȯ
            newPos = transform.TransformPoint(spawnAreaCenter + new Vector3(x, 0, z));
            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }

        // ���� �������� ������ ����
        Gizmos.DrawLine(pos, lastPos);
    }
}
