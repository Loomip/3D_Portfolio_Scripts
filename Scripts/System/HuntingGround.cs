using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingGround : MonoBehaviour
{
    // 몬스터가 배회할 위치를 넘겨줄 배회위치
    [SerializeField] private Transform[] wanderPoints;
    public Transform[] WanderPoints { get => wanderPoints; set => wanderPoints = value; }

    // 랜덤 소환 이팩트
    [SerializeField] private List<GameObject> effectPrefabs;
    // 랜덤으로 등장할 몬스터
    [SerializeField] private List<GameObject> monsterPrefabs;
    // Player를 탐지할 BoxCollider
    [SerializeField] private BoxCollider zone;
    // 입구 막기 콜라이더
    [SerializeField] private GameObject incollider;
    [SerializeField] private GameObject outcollider;
    // 소환 이팩트가 나타나는 시간
    [SerializeField] private float effectAppearTime = 2.0f;

    // 몬스터 스폰 영역 중심 위치
    [SerializeField] private Vector3 spawnAreaCenter;
    // 몬스터 스폰 영역 크기
    [SerializeField] private Vector3 spawnAreaSize;

    // 클리어하기 위해 필요한 몬스터 수
    [SerializeField] private int monstersToClear = 10; 

    // 소환 될 몬스터를 담아 클리어를 판단할 리스트
    [SerializeField] private List<GameObject> enemyList = new List<GameObject>();

    // 맵 재시작 방지 
    private bool isGroundStart = false;
    public bool IsGroundStart { get => isGroundStart; set => isGroundStart = value; }

    // 방이 클리어된 상태를 저장
    private bool isCleared = false;

 


    // 몬스터에게 배회위치를 넘겨줄 메소드
    public Transform[] GetWanderPoints()
    {
        return wanderPoints;
    }

    // 평면 원 소환 범위
    Vector3 GetRandomSpawnPosition()
    {
        float radius = spawnAreaSize.x; // 원의 반지름을 spawnAreaSize로 설정
        float randomRadius = UnityEngine.Random.Range(0f, radius); // 0에서 지정된 반지름 사이의 랜덤한 거리
        float randomAngle = UnityEngine.Random.Range(0f, 2 * Mathf.PI); // 0에서 2π 사이의 랜덤한 각도

        // 원의 랜덤한 위치 계산 (y 좌표는 고정)
        float x = randomRadius * Mathf.Cos(randomAngle);
        float z = randomRadius * Mathf.Sin(randomAngle);

        // 로컬 좌표를 월드 좌표로 변환
        Vector3 localPosition = new Vector3(x, spawnAreaCenter.y, z);
        Vector3 worldPosition = transform.TransformPoint(localPosition);

        return worldPosition;
    }

    IEnumerator SpawnEffectAndMonster()
    {
        while (GetEnemyCount() < monstersToClear)
        {
            // 랜덤 이팩트 선택 코드
            GameObject selectedEffectPrefab = effectPrefabs[UnityEngine.Random.Range(0, effectPrefabs.Count)];

            // 랜덤 몬스터 선택 코드
            GameObject selectedMonsterPrefab = monsterPrefabs[UnityEngine.Random.Range(0, monsterPrefabs.Count)];

            // 몬스터 스폰 위치 계산
            Vector3 randomSpawnPosition = GetRandomSpawnPosition();

            // 이팩트 생성
            GameObject effectInstance = Instantiate(selectedEffectPrefab, randomSpawnPosition, selectedEffectPrefab.transform.rotation);

            // 몬스터 생성
            GameObject monsterInstance = Instantiate(selectedMonsterPrefab, randomSpawnPosition, Quaternion.identity);

            Destroy(effectInstance, 5f);

            // MonsterState 스크립트의 SetHuntingGroundController 메서드를 호출하여 HuntingGround 인스턴스 전달
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
            // 모든 몬스터가 사망하여 클리어 상태
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

        // 이팩트 나타나는 애니메이션
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

    // 원 소환 영역 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float theta = 0;
        float x = spawnAreaSize.x * Mathf.Cos(theta);
        float z = spawnAreaSize.x * Mathf.Sin(theta);

        // 로컬 좌표를 월드 좌표로 변환
        Vector3 pos = transform.TransformPoint(spawnAreaCenter + new Vector3(x, 0, z));
        Vector3 newPos = pos;
        Vector3 lastPos = pos;

        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = spawnAreaSize.x * Mathf.Cos(theta);
            z = spawnAreaSize.x * Mathf.Sin(theta);

            // 로컬 좌표를 월드 좌표로 변환
            newPos = transform.TransformPoint(spawnAreaCenter + new Vector3(x, 0, z));
            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }

        // 원의 시작점과 끝점을 연결
        Gizmos.DrawLine(pos, lastPos);
    }
}
