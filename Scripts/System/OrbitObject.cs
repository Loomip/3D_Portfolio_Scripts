using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitObject : MonoBehaviour
{
    [SerializeField] private Transform target; // 캐릭터
    [SerializeField] private float speed; // 속도
    [SerializeField] private float radiusX; // 타원의 X 반지름
    [SerializeField] private float radiusY; // 타원의 Y 반지름
    [SerializeField] private float rotationSpeed = 10f; // 회전 속도
    [SerializeField] private float minY; // 최소 y 위치
    [SerializeField] private float maxY; // 최대 y 위치

    private float angle; // 현재 각도
    private Vector3 rotationAxis; // 회전 축

    private void Start()
    {
        target = GameObject.FindWithTag("Player").GetComponent<Transform>();

        // 랜덤한 회전 축 생성
        rotationAxis = new Vector3(Random.value, Random.value, Random.value);
    }

    private void Update()
    {
        // 각도 업데이트
        angle += speed * Time.deltaTime;

        // 새 위치 계산
        float x = Mathf.Cos(angle) * radiusX;
        float z = Mathf.Sin(angle) * radiusY;
        float y = Mathf.PerlinNoise(Time.time * 0.1f , 0) * (maxY - minY) + minY; // 부드럽게 변경되는 y 위치
        Vector3 newPos = target.position + new Vector3(x, y, z);

        // 오브젝트 위치 업데이트
        transform.position = newPos;

        StartCoroutine(RnmdomAxis());

        // 오브젝트 회전 업데이트
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }

    IEnumerator RnmdomAxis()
    {
        // 랜덤한 회전 축 생성
        rotationAxis = new Vector3(Random.value, Random.value, Random.value);

        yield return new WaitForSeconds(5f);
    }
}
