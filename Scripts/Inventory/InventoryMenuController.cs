using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryMenuController : MonoBehaviour
{
    // 인벤토리 메뉴 버튼 리스트
    List<UIInventoryMenuButton> m_menuList = new List<UIInventoryMenuButton>();

    // 인벤토리 메뉴 게임 오브젝트 리스트
    [SerializeField] List<GameObject> m_menu = new List<GameObject>();

    // 기본적으로 활성화된 메뉴 타입
    private e_MenuType CurMenu = e_MenuType.Status;

    // 현재 메뉴 타입을 반환하는 프로퍼티
    public e_MenuType CURMENU
    {
        get => CurMenu;
    }

    // 버튼이 배치될 위치
    [SerializeField] Transform buttonTransform;

    // 버튼 프리팹
    [SerializeField] GameObject button;

    // 메뉴 프리팹이 배치될 위치
    [SerializeField] Transform meunTransform;

    // 인벤토리를 표시하는 메서드
    public void InvenShow()
    {
        Initialize();
    }

    // 초기화 메서드
    private void Initialize()
    {
        // 메뉴 버튼 초기화
        InitializeMenuButton();

        // 메뉴 초기화
        InitializeMenu(CurMenu);

        // 골드 초기화
        InventoryManager.instance.Refresh_Gold();
    }

    // 메뉴 버튼을 초기화하는 메서드
    private void InitializeMenuButton()
    {
        // 메뉴 리스트가 비어있으면 버튼을 생성
        if (m_menuList.Count <= 0)
        {
            for (var i = e_MenuType.None + 1; i < e_MenuType.Length; ++i)
            {
                // 버튼을 생성하고 UIInventoryMenuButton 컴포넌트를 가져옴
                UIInventoryMenuButton b = Instantiate(button, buttonTransform).GetComponent<UIInventoryMenuButton>();
                // 생성된 버튼을 리스트에 추가
                m_menuList.Add(b);
            }
        }

        // 각 버튼을 초기화
        for (var i = e_MenuType.None + 1; i < e_MenuType.Length; ++i)
        {
            var index = (int)i - 1;
            // 버튼 초기화
            m_menuList[index].InitButton(i, OnClickButtonCallback);
        }
    }

    // 메뉴를 초기화하는 메서드
    private void InitializeMenu(e_MenuType menuType)
    {
        // 모든 메뉴를 비활성화
        SetActiveAll(false);

        // 메뉴 인덱스 계산
        int menuIndex = (int)menuType - 1;

        // 메뉴 인덱스가 유효한 범위 내에 있으면
        if (menuIndex >= 0 && menuIndex < m_menu.Count)
        {
            // 메뉴 인스턴스 참조
            GameObject menuInstance = null;
            switch (menuType)
            {
                case e_MenuType.Status:
                    menuInstance = m_menu[menuIndex];
                    break;
                case e_MenuType.ItemSlot:
                    menuInstance = m_menu[menuIndex];
                    break;
                case e_MenuType.Settings:
                    menuInstance = m_menu[menuIndex];
                    break;

            }

            // 인스턴스가 존재하면 활성화
            if (menuInstance != null)
            {
                menuInstance.SetActive(true);
            }
            else
            {
                Debug.Log("메뉴 유형 " + menuType.ToString() + "에 대한 메뉴 인스턴스를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.Log("메뉴 유형 " + menuType.ToString() + "에 대한 메뉴 인덱스가 올바르지 않습니다.");
        }
    }

    // 버튼 클릭 콜백 메서드
    public void OnClickButtonCallback(e_MenuType menuType)
    {
        // 현재 메뉴를 menuType으로 설정
        CurMenu = menuType;
        // 메뉴 초기화
        Initialize();
    }

    // 모든 메뉴를 활성화 또는 비활성화하는 메서드
    private void SetActiveAll(bool enable)
    {
        // m_menu가 null이 아니면
        if (m_menu != null)
        {
            // m_menu의 모든 요소에 대해
            for (int i = m_menu.Count - 1; i >= 0; i--)
            {
                // 요소가 null이 아니면
                if (m_menu[i] != null)
                {
                    m_menu[i].SetActive(enable);
                }
                else
                {
                    // 요소가 null이면 리스트에서 제거
                    m_menu.RemoveAt(i);
                }
            }
        }

        // 현재 메뉴가 None이 아니면
        if (CurMenu != e_MenuType.None)
        {
            // 메뉴 인덱스 계산
            int menuIndex = (int)CurMenu;
            // 메뉴 인덱스가 유효한 범위 내에 있으면
            if (menuIndex >= 0 && menuIndex < m_menu.Count)
            {
                // 메뉴 인스턴스 참조
                GameObject menuinstanceance = m_menu[menuIndex];
                menuinstanceance.SetActive(enable);
            }
            else
            {
                Debug.Log("메뉴 유형 " + CurMenu.ToString() + "에 대한 메뉴 인스턴스를 찾을 수 없습니다.");
            }
        }
    }

    private void Start()
    {
        SetActiveAll(true);
        Initialize();
    }
}