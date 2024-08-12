using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("인벤토리 UI")]
    [SerializeField] private GameObject inven;

    //===================================================================================================

    [Header("카메라 컴포넌트")]
    [SerializeField] private CinemachineFreeLook camera;
    public CinemachineFreeLook Camera { get => camera; set => camera = value; }
    // 카메라 회전 속도
    [SerializeField] private float xSpeed; // m_XAxis 속도
    [SerializeField] private float ySpeed; // m_YAxis 속도
    // 줌인 줌아웃 속도
    [SerializeField] private float zoomSpeed; // 줌 속도
    [SerializeField] private float minFOV; // 최소 FOV
    [SerializeField] private float maxFOV; // 최대 FOV

    //===================================================================================================

    [Header("대화창 UI")]
    [SerializeField] private GameObject InteractText;
    [SerializeField] private NPC_Shop Npc;
    [SerializeField] private GameObject player;
    public GameObject Player { get => player; set => player = value; }

    //상호작용
    void InteractableObject()
    {
        if (Player != null && Player.GetComponent<NPCInteraction>().GetInterctableObject() != null && !Player.GetComponent<NPCInteraction>().IsDialogueActive)
            InteractableShow();
        else
            InteractableHide();
    }

    // 상호작용 오브젝트를 켬
    private void InteractableShow()
    {
        InteractText.SetActive(true);
    }

    // 상호작용 오브젝트를 끔
    public void InteractableHide()
    {
        InteractText.SetActive(false);
    }

    //===================================================================================================

    [Header("상호작용 UI")]
    // 열기 UI
    [SerializeField] private GameObject openDoorUI;

    //private void DoorableObject()
    //{
    //    if (Player != null && Player.GetComponent<NPCInteraction>().GetDoorableObject() != null)
    //        OpenDoor();
    //    else
    //        CloseDoor();
    //}

    // 상호작용 오브젝트를 켬
    //private void OpenDoor()
    //{
    //    openDoorUI.SetActive(true);
    //}

    //// 상호작용 오브젝트를 끔
    //private void CloseDoor()
    //{
    //    openDoorUI.SetActive(false);
    //}

    //===================================================================================================

    [Header("UI")]
    [SerializeField] private Slider playerHp;
    [SerializeField] private Slider playerMp;
    [SerializeField] private Slider bossHp;

    // 생성된 각 몬스터의 체력바를 저장할 딕셔너리
    [SerializeField] private Dictionary<Health, Slider> enemyHealthBars = new Dictionary<Health, Slider>();

    // 몬스터와 그에 해당하는 체력바를 딕셔너리에 등록
    public void RegisterEnemyHealthBar(Health enemyHealth, Slider healthBar)
    {
        enemyHealthBars[enemyHealth] = healthBar;
    }

    // 체력 리프레쉬
    public void RefreshHp(string tag, Health entity)
    {
        switch (tag)
        {
            case "Player":
                playerHp.value = (float)entity.hp / entity.State.MaxHp;
                break;
            case "Enemy":
                if (enemyHealthBars.TryGetValue(entity, out Slider enemyHp))
                {
                    enemyHp.value = (float)entity.hp / entity.State.MaxHp;
                }
                break;
            case "Boss":
                bossHp.value = (float)entity.hp / entity.State.MaxHp;
                break;
        }
    }

    // 마나 리프레쉬
    public void RefreshPlayerMp(Health entity)
    {
        playerMp.value = (float)entity.mp / entity.State.MaxMp;
    }

    //============================================================================================================

    private void Update()
    {
        InteractableObject();
        //DoorableObject();

        // 상점이 열려있는지 확인
        if (Npc.IsShopOpen() == true || Player.GetComponent<NPCInteraction>().IsDialogueActive)
        {
            // 상점이 열려있으면 인벤토리를 비활성화
            inven.SetActive(false);
            Player.GetComponent<MoveController>().Animator.SetFloat("isRun", 0f);
            Player.GetComponent<MoveController>().enabled = false;
        }

        else if (Input.GetKeyDown(KeyCode.I) && Npc.IsShopOpen() == false)
        {
            // 상점이 닫혀있고 'I' 키가 눌렸으면 인벤토리의 활성 상태를 토글
            inven.SetActive(!inven.activeInHierarchy);

            var isShow = inven.activeInHierarchy;

            // 인벤토리가 켜지면
            if (isShow)
            {
                // 인벤토리 초기화
                inven.GetComponentInChildren<InventoryMenuController>().InvenShow();

                // 마우스를 보여줌
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // 인벤토리가 꺼지면 
            else
            {
                // 마우스 삭제
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // 마우스 휠 버튼을 누르면 
        if (Input.GetMouseButton(2))
        {
            // 카메라 회전 시작
            Camera.m_XAxis.m_MaxSpeed = xSpeed;
            Camera.m_YAxis.m_MaxSpeed = ySpeed;
        }
        else
        {
            // 카메라 회전 멈춤
            Camera.m_XAxis.m_MaxSpeed = 0;
            Camera.m_YAxis.m_MaxSpeed = 0;

            // 마우스 휠의 움직임에 따라 FOV를 조정하여 줌 인/아웃을 구현
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            Camera.m_Lens.FieldOfView -= scrollInput * zoomSpeed;
            Camera.m_Lens.FieldOfView = Mathf.Clamp(Camera.m_Lens.FieldOfView, minFOV, maxFOV);
        }
    }
}
