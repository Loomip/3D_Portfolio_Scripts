using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] GroundController GC;
    [SerializeField] GameObject dialoguePanel; // 대화창 UI
    [SerializeField] CinemachineFreeLook freeLookCamera; // 시네머신 가상 카메라
    [SerializeField] Transform originalCameraTarget; // 원래의 카메라 목표
    [SerializeField] Transform originalLookAt;
    [SerializeField] float cameraMoveDuration = 1.0f;

    Transform npcTransform; // NPC의 위치

    private bool isDialogueActive = false;

    public bool IsDialogueActive { get => isDialogueActive; set => isDialogueActive = value; }

    //상호작용 키
    void NPCInteract()
    {
        if (Input.GetKeyUp(KeyCode.R) && !IsDialogueActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            IsDialogueActive = true;
            InteractWithCurrentTarget();
        }

        //if (Input.GetKeyUp(KeyCode.E) && !GC.HuntiogGround.IsGroundStart && !GC.BossGround.IsGroundStart)
        //{
        //    OpenDoorControl();
        //}
    }

    //상호 작용
    public void InteractWithCurrentTarget()
    {
        float interactRange = 1.5f;

        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out NPC_Base npc))
            {
                npcTransform = npc.transform; // npcTransform 할당
                StartCoroutine(MoveCameraToNPC(npcTransform));
                npc.OnInteract();
            }
        }
    }

    //상호작용 오브젝트
    public NPC_Base GetInterctableObject()
    {
        float interactRange = 1.5f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out NPC_Base npc))
            {
                return npc;
            }
        }
        return null;
    }

    IEnumerator MoveCameraToNPC(Transform targetTransform)
    {
        float elapsedTime = 0;
        Transform originalTarget = freeLookCamera.Follow;
        Transform originalLookAt = freeLookCamera.LookAt;

        freeLookCamera.Follow = targetTransform;
        freeLookCamera.LookAt = targetTransform;

        while (elapsedTime < cameraMoveDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        IsDialogueActive = false;
        UIManager.instance.Player.GetComponent<MoveController>().enabled = true;
        StartCoroutine(MoveCameraBack());
    }

    IEnumerator MoveCameraBack()
    {
        float elapsedTime = 0;
        freeLookCamera.Follow = originalCameraTarget;
        freeLookCamera.LookAt = originalLookAt;

        while (elapsedTime < cameraMoveDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }



    //void OpenDoorControl()
    //{
    //    float interactRange = 1.5f;

    //    Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

    //    foreach (Collider collider in colliderArray)
    //    {
    //        if (collider.TryGetComponent(out Door door))
    //        {
    //            door.OpenDoor();
    //        }
    //    }
    //}

    //public Door GetDoorableObject()
    //{
    //    float interactRange = 1.5f;
    //    Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
    //    foreach (Collider collider in colliderArray)
    //    {
    //        if (collider.TryGetComponent(out Door door))
    //        {
    //            return door;
    //        }
    //    }
    //    return null;
    //}

    private void Update()
    {
        NPCInteract();
    }
}
