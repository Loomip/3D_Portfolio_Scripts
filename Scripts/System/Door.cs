using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // 문이 열리는 속도
    public float openSpeed = 2f; 
    // 문을 열 수 있는 최대 거리
    public float activationDistance = 3f; 

    // 문이 닫힌 위치
    private Vector3 closedPosition; 
    // 문이 열린 위치
    private Vector3 openPosition; 



    private void Start()
    {
        closedPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        openPosition = new Vector3(transform.position.x, 10f, transform.position.z);
    }

    // 문이 닫혀 있는지
    private bool isDoorClosed = true;
    [SerializeField] private bool isOpening = false;
    [SerializeField] private bool isClosing = false;

    public void CloseDoor()
    {
        if (!isDoorClosed)
        {
            isClosing = true;
            isDoorClosed = true;
        }
    }

    public void OpenDoor()
    {
        if (isDoorClosed)
        {
            isOpening = true;
            isDoorClosed = false;
        }
    }

    private void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.Lerp(transform.position, openPosition, openSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, openPosition) <= 0.01f)
            {
                transform.position = openPosition;
                isOpening = false;
            }
        }
        else if (isClosing)
        {
            transform.position = Vector3.Lerp(transform.position, closedPosition, openSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, closedPosition) <= 0.01f)
            {
                transform.position = closedPosition;
                isClosing = false;
            }
        }
    }
}
