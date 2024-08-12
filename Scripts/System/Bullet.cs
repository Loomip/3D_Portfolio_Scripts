using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int atk;
    public int Atk { get => atk; set => atk = value; }

    // 리지드 바디 컴포넌트
    Rigidbody rigid;

    // 타겟 레이어
    [SerializeField] LayerMask layerMask = 0;

    // 타겟 레이어 판단 코드
    [SerializeField] List<string> hitLayerNames;

    // 인식할 타겟 트렌스폼
    Transform tfTarget;

    //최고 스피드
    public float speed = 0f;

    //현재 스피드
    float currentSpeed = 0f;

    // 타겟이 설정되었는지 여부를 나타내는 플래그
    public bool isTargetSet = false;

    float targetSetTime; // 타겟이 설정된 시간

    // 총알이 사라질 시작 위치 저장
    private Vector3 launchPosition;

    // 총알이 사라져야될 최대 길이
    private float maxTravelDistance = 50f;

    //유도탄
    void SearchEnemy(Vector3 bulletDirection)
    {
        Collider[] searchRange = Physics.OverlapSphere(transform.position, 5f, layerMask);

        float closestAngle = 360f; // 총알이 인식할 수 있는 최대 각도

        foreach (var collider in searchRange)
        {
            // Player가 발사하는 총알은 유도탄
            if (collider.tag == "Enemy" || collider.tag == "Boss")
            {
                // 총알이 타격을 향한 방향 벡터를 구함
                Vector3 toTarget = collider.transform.position - transform.position;

                // 총알이 타격 대상과의 시선 각도를 구함
                float angle = Vector3.Angle(bulletDirection, toTarget);

                if (angle < closestAngle)
                {
                    closestAngle = angle;

                    // 타격 대상이 결정됨
                    tfTarget = collider.transform;
                    isTargetSet = true;
                    targetSetTime = Time.time;
                    break;
                }
            }
        }
    }

    IEnumerator LaunchDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Vector3 bulletDirection = rigid.velocity.normalized; // 총알의 방향을 구합니다

        while (true)
        {
            if (!isTargetSet)
            {
                SearchEnemy(bulletDirection);

                if (Time.time - targetSetTime >= 1f)
                {
                    // 타겟이 설정되지 않았을 때 일정 시간이 지나면 파괴
                    Destroy(gameObject);
                    yield break;
                }
            }
            else
            {
                yield break; // 타겟이 설정되었으므로 코루틴 종료
            }

            yield return null; // 다음 프레임까지 대기
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // hitLayerName에 해당하는 레이어에 있는 오브젝트가 있는지 확인
        if ((other != null && hitLayerNames.Contains(LayerMask.LayerToName(other.gameObject.layer))) || other.tag == "Ground")
        {
            // 해당 오브젝트의 Health 컴포넌트를 가져옴
            Health targetHealth = other.gameObject.GetComponent<Health>();
            if (targetHealth != null)
            {
                if (other.tag == "Enemy")
                {
                    other.GetComponent<MonsterFSMController>().Hit();
                }
                if (other.tag == "Boss")
                {
                    other.GetComponent<BossFSMController>().Hit();
                }
                other.GetComponent<Health>().Hit(Atk);
            }

            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (tfTarget != null)
        {
            // 총알의 방향을 구해줌
            Vector3 tfTargets = tfTarget.position;
            tfTargets.y += 1f;
            Vector3 Direction = (tfTargets - transform.position).normalized;

            if (currentSpeed <= speed)
                currentSpeed += speed * Time.deltaTime;

            transform.position += Direction * currentSpeed * Time.deltaTime;

            transform.forward = Vector3.Lerp(transform.position, Direction, 0.25f);
        }

        // 버그로 총알이 빗맞앗을 경우 총알이 계속 살아있기 때문에 일정 길이 이상으로 벌어지면
        // 총알을 삭제 해줘야함
        if (Vector3.Distance(transform.position, launchPosition) > maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        targetSetTime = Time.time; // 초기화
        launchPosition = transform.position;
        StartCoroutine(LaunchDelay());
    }
}
