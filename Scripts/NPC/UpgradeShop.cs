using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShop : NPC_Base
{
    [Header("위치")]
    // 강화 UI
    [SerializeField] private GameObject upgradeUI;
    //아이템 슬롯들이 들어가는 위치
    [SerializeField] private Transform enhanceContent;
    //선택된 아이템의 이미지
    [SerializeField] private Image tooltip_Icon;
    // 시스템 메시지
    [SerializeField] private TextMeshProUGUI systemMessageText;

    [Header("프리팹")]
    //아이템 슬롯 프리팹
    [SerializeField] private GameObject slotPrefab;

    [Header("버튼")]
    //강화 버튼 
    [SerializeField] private TextMeshProUGUI btn_Enhance_Text;

    // 강화 단계별 확률
    private float[] enhanceChances = new float[] { 0.8f, 0.7f, 0.6f, 0.5f, 0.4f };

    // 강화 단계 (0 ~ 4: 0이면 1강, 4면 5강)
    private int enhanceLevel = 0;

    Slot slot;

    Data_Item.Param m_Item;

    List<Slot> slotList = new List<Slot>();

    //버튼용 Delegate(대리자)
    delegate void UseButton();

    UseButton useButton;

    //아이템 버튼을 누르면 호출하는 함수
    public void ItemButton() => useButton.Invoke();

    protected override void Start()
    {
        base.Start();
        dialogueMessage = "강화가 필요하신가요?";
    }

    public override void OnInteract()
    {
        ShowDialogue(
            dialogueMessage,
            new List<DialogueOption>
            {
                new DialogueOption("강화창 열기", OpenUpgradeUI),
                new DialogueOption("나가기", CloseDialogue)
            }
        );
    }

    private void OpenUpgradeUI()
    {
        upgradeUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DialoguePanel.SetActive(false);

        tooltip_Icon.enabled = true;
        Refresh_Tooltip(slot.CurrentItem);
        Refresh_Button(slot.CurrentItem);
    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UIManager.instance.Player.GetComponent<NPCInteraction>().CloseDialogue();
    }

    //강화탭 인벤토리
    public void InitSlots()
    {
        List<Slot> slots = InventoryManager.instance.SlotList;

        for (int i = 0; i < slots.Count; i++)
        {
            slot = Instantiate(slotPrefab, enhanceContent).GetComponent<Slot>();
            slot.SLOTINDEX = i;
            slot.onItemClick += Refresh_Tooltip;
            slot.onItemClick += Refresh_Button;
            slotList.Add(slot);
        }
    }

    //인벤토리의 아이템 아이콘을 갱신하는 메서드
    private void RefreshIcon()
    {
        List<ItemData> dataList = InventoryManager.instance.GetItemList();

        // 모든 슬롯을 클리어합니다.
        foreach (Slot slot in slotList)
        {
            slot.ClearSlot();
        }

        int weaponIndex = 0; // 무기 아이템만을 카운트하는 새로운 인덱스 변수

        for (int i = 0; i < dataList.Count; i++)
        {
            string ItemType = DataManager.instance.GetWeaponData(dataList[i].id).ItemType;
            int baseItemId = (dataList[i].id / 1000) * 1000;
            enhanceLevel = dataList[i].id - baseItemId;

            if (ItemType == e_EquipType.Weapon.ToString() && enhanceLevel < Consts.MAX_ENHANCE_LEVEL)
            {
                if (weaponIndex < slotList.Count)
                {
                    slotList[weaponIndex].Set_Icon(dataList[i]);
                    weaponIndex++;
                }
            }
        }
    }

    //아이템의 정보를 표시해주는 함수
    private void Refresh_Tooltip(ItemData item)
    {
        if (item == null)
        {
            tooltip_Icon.color = Color.clear;
            tooltip_Icon.sprite = null;
        }
        else
        {
            m_Item = DataManager.instance.GetWeaponData(item.id);
            tooltip_Icon.color = Color.white;
            tooltip_Icon.sprite = Resources.Load<Sprite>("Itemicons/" + m_Item.SpritName);
        }
    }

    //버튼을 바꿔주는 함수
    private void Refresh_Button(ItemData item)
    {
        if (item == null)
        {
            btn_Enhance_Text.transform.parent.gameObject.SetActive(false);
            return;
        }

        btn_Enhance_Text.transform.parent.gameObject.SetActive(true);

        if (DataManager.instance.GetWeaponData(item.id) != null)
        {
            string ItemType = DataManager.instance.GetWeaponData(item.id).ItemType;

            switch (ItemType)
            {
                case "Weapon":
                    btn_Enhance_Text.text = DataManager.instance.GetWordData("Enhance");
                    useButton = () => Button_Enhance(item);
                    break;
            }
        }
    }

    // 강화 기능 (버튼에 다는 기능)
    private void Button_Enhance(ItemData itme)
    {
        if (itme == null)
        {
            btn_Enhance_Text.transform.parent.gameObject.SetActive(false);
            return;
        }

        // 아이템이 아직 인벤토리에 있는지 확인
        if (!InventoryManager.instance.ItemExist(itme))
        {
            Debug.Log("The item does not exist in the inventory!");
            return;
        }

        // 아이템 종류에 따라 기본 아이템 ID를 가져옴
        int baseItemId = (itme.id / 1000) * 1000;

        // 아이템 ID를 기반으로 강화 레벨을 계산
        int itemEnhanceLevel = itme.id - baseItemId;

        // 강화 레벨이 enhanceChances 배열의 범위를 벗어나면 강화를 시도하지 않음
        if (itemEnhanceLevel >= enhanceChances.Length)
        {
            StartCoroutine(ShowSystemMessage(DataManager.instance.GetWordData("Max")));
            return;
        }

        // 강화 확률을 구함
        float enhanceChance = enhanceChances[itemEnhanceLevel];

        // 랜덤한 숫자를 구해서 강화 확률과 비교
        if (Random.value <= enhanceChance)
        {
            // 강화 성공 시 로직
            EnhanceItem(itme);

            // 강화 성공 메시지 표시
            StartCoroutine(ShowSystemMessage(DataManager.instance.GetWordData("Success")));

            // 강화 성공 후 툴팁 비활성화
            Refresh_Tooltip(null);
            Refresh_Button(null);

            // 아이템 리스트 업데이트
            RefreshIcon();
        }
        else
        {
            // 강화 실패 메시지 표시
            StartCoroutine(ShowSystemMessage(DataManager.instance.GetWordData("Failed")));
        }
    }

    private IEnumerator ShowSystemMessage(string message)
    {
        // 메시지와 이미지를 표시
        systemMessageText.text = message;
        systemMessageText.enabled = true;

        yield return null;
    }

    private void EnhanceItem(ItemData item)
    {
        // 강화 레벨 증가
        enhanceLevel++;

        // 강화된 아이템을 새로운 슬롯에 추가
        AddEnhancedItemToNewSlot(item);

        // 원본 아이템 제거
        InventoryManager.instance.RemoveItem(item);

        // 아이템 초기화
        slot.ClearSlot();
    }

    private void AddEnhancedItemToNewSlot(ItemData item)
    {
        // 강화된 아이템 ID를 구함
        int enhancedItemId = item.id + 1;

        // 강화된 아이템 데이터를 불러옴
        Data_Item.Param enhancedItemData = DataManager.instance.GetWeaponData(enhancedItemId);

        // 불러온 아이템 데이터가 없다면 강화된 아이템이 없는 것이므로 종료
        if (enhancedItemData == null)
        {
            Debug.Log("No item data for the enhanced item ID: " + enhancedItemId);
            return;
        }

        // 새로운 아이템 데이터를 생성
        ItemData newItem = new ItemData
        {
            id = enhancedItemId
        };

        // 새로운 슬롯에 아이템 추가
        InventoryManager.instance.AddItem(newItem);

        // 새롭게 추가된 아이템 아이콘 리프레쉬
        RefreshIcon();
    }

    private void OnEnable()
    {
        systemMessageText.enabled = false;
    }

    private void Update()
    {
        RefreshIcon();
    }
}
