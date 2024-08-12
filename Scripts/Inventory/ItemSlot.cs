using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [Header("인벤토리")]
    //아이템 슬롯 프리팹
    [SerializeField] private GameObject itemslotPrefab;
    //아이템 슬롯들이 들어가는 위치
    [SerializeField] private Transform itemContent;

    [Header("버튼")]
    // 생성 버튼
    [SerializeField] private TextMeshProUGUI btn_Create_Text;
    // 사용/장착 버튼 
    [SerializeField] private TextMeshProUGUI btn_Use_Text;
    // 버리기 버튼
    [SerializeField] private TextMeshProUGUI btn_Discard_Text;

    [Header("팝업")]
    //아이템 정보 관련된 변수
    [SerializeField] private GameObject itemPopup;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemExplanText;
    [SerializeField] private Image tooltip_Icon;

    // 플레이어 스텟 참조
    private CharacterState player;

    // 플레이어 이동 참조
    private MoveController moveController;
    public MoveController MoveController { get => moveController; set => moveController = value; }

    // 스테이터스 참조
    [SerializeField] private Status status;

    //버튼용 Delegate(대리자) : 여러가지 객체들이 받는 방법
    delegate void UseButton();

    UseButton useButton;

    //아이템 버튼을 누르면 호출하는 함수
    public void ItemButton() => useButton.Invoke();

    // 인벤토리 슬롯
    Slot slot;

    // 선택 되어진 아이템
    ItemData selectItem;

    //인벤토리의 슬롯을 초기화 하는 메서드
    public void InitSlots()
    {
        List<Slot> slots = InventoryManager.instance.SlotList;

        for (int i = 0; i < slots.Count; i++)
        {
            slot = Instantiate(itemslotPrefab, itemContent).GetComponent<Slot>();
            slot.SLOTINDEX = i;
            slot.onItemClick += Refresh_Button;
            slot.onItemClick += SelectItem;
            slots[i] = slot;
        }
    }

    // Player 인벤토리 아이템 선택 (슬롯 클릭 시 호출)
    public void SelectItem(ItemData selectedItem)
    {
        selectItem = selectedItem;
        OpenPopUp();
    }

    //아이템의 정보를 표시해주는 함수
    private void OpenPopUp()
    {
        if (selectItem == null)
        {
            itemPopup.SetActive(false);
            return;
        }

        var data = DataManager.instance.GetLacalizeData(selectItem.id);

        if (data != null)
        {
            itemPopup.SetActive(true);
            tooltip_Icon.color = Color.white;
            tooltip_Icon.sprite = Resources.Load<Sprite>("Itemicons/" + data.SpritName);
            itemExplanText.text = data.Explan;
            itemNameText.text = data.TooltipName;
        }
        else
        {
            Debug.Log("Item data is null for id: " + selectItem.id);
        }

    }

    // 장비 장착 기능
    public void Button_Equip(ItemData item)
    {
        // 선택한 장비의 타입
        Data_Item.Param itemData = DataManager.instance.GetWeaponData(item.id);

        if (item == null)
        {
            Debug.LogError("아이템 데이터가 없습니다.");
            return;
        }

        // 무기 타입을 가져옴
        e_Weapon weaponType = GetWeaponType(itemData);

        //애니메이션 설정
        MoveController.Animator.SetInteger("WeaponState", (int)weaponType);

        // 선택한 아이템의 타입과 일치하는 장비 슬롯을 찾음
        foreach (var equipSlot in status.EquipSlotList.Values)
        {
            if (equipSlot.Type.ToString() == itemData.ItemType)
            {
                // 이미 장착한 장비가 있는지 확인
                if (equipSlot.IsEquipped)
                {
                    // 선택한 장비를 해제
                    equipSlot.Detach();

                    Debug.Log(player.Mp);
                    Debug.Log(player.Exhaustion);
                }

                // 선택한 장비를 장착
                equipSlot.Set(item);
            }
        }

        ClosePopup();

        InventoryManager.instance.RefreshIcon();
    }

    private e_Weapon GetWeaponType(Data_Item.Param item)
    {
        // SpritName에 따라 무기 유형을 결정
        switch (item.SpritName)
        {
            case "Weapon":
                return e_Weapon.Weapon;
            case "Spell":
                return e_Weapon.Spell;
            default:
                MoveController.IsWeaponEquipped = false;
                return e_Weapon.None;
        }
    }

    // 소모품 사용 기능
    public void Button_Use(ItemData item)
    {
        var status = DataManager.instance.GetWeaponData(item.id);

        if (status != null)
        {
            switch (status.Name)
            {
                case "Potion":
                    player.AddHp(status.Hp);
                    UIManager.instance.RefreshHp(player.tag, player.GetComponent<Health>());
                    break;
                case "Leaf":
                    player.AddMp(status.Mp);
                    UIManager.instance.RefreshPlayerMp(player.GetComponent<Health>());
                    break;
            }
        }

        InventoryManager.instance.RemoveItem(item);

        // 아이템을 사용한 후에 아이템의 개수를 확인
        if (item.amount < 1)
        {
            slot.ClearSlot();
            ClosePopup();
        }
    }

    // 버튼 변환 기능
    public void Refresh_Button(ItemData item)
    {
        if (item == null)
        {
            btn_Discard_Text.transform.parent.gameObject.SetActive(false);
            return;
        }

        btn_Discard_Text.transform.parent.gameObject.SetActive(true);

        btn_Discard_Text.text = DataManager.instance.GetWordData("Discard");

        if (DataManager.instance.GetWeaponData(item.id) != null)
        {
            string ItemType = DataManager.instance.GetWeaponData(item.id).ItemType;

            switch (ItemType)
            {
                case "Weapon":
                    btn_Use_Text.text = DataManager.instance.GetWordData("Wear");
                    useButton = () => Button_Equip(item);
                    break;

                case "Consumable":
                    btn_Use_Text.text = DataManager.instance.GetWordData("Use");
                    useButton = () => Button_Use(item);
                    break;
            }
        }
        else
        {
            Debug.Log("선택된 데이터가 없습니다. : " + item.id);
        }
    }

    //버리기 버튼 기능 (버튼에 직접 다는 기능)
    private void Button_Discard()
    {
        // 아이템을 인벤토리에서 제거
        InventoryManager.instance.RemoveItem(selectItem);

        // 아이템의 개수가 0이거나 아이템이 null인 경우
        if (selectItem.amount < 1)
        {
            slot.ClearSlot();
            ClosePopup();
        }
    }

    // 닫기 버튼 (버튼에 다는 기능)
    public void ClosePopup()
    {
        tooltip_Icon.color = Color.clear;
        tooltip_Icon.sprite = null;
        itemExplanText.text = string.Empty;
        itemNameText.text = string.Empty;
        slot.ClearSlot();

        itemPopup.SetActive(false);
    }

    // 생성버튼 기능 (버튼에 직접 다는 기능)
    public void Btu()
    {
        // 랜덤한 아이템 데이터 가져오기
        Data_Item.Param randomItemData = DataManager.instance.GetRandomItemDataParams();

        // 가져온 아이템 데이터로 새 아이템 생성
        if (randomItemData != null)
        {
            ItemData newItem = new ItemData();
            newItem.id = randomItemData.ID;
            ++newItem.amount;

            // 새 아이템을 인벤토리에 추가
            InventoryManager.instance.AddItem(newItem);
        }
        else
        {
            Debug.Log("임의 선택에 사용할 수 있는 항목 데이터가 없습니다.");
        }
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<CharacterState>();
        MoveController = GameObject.Find("Player").GetComponent<MoveController>();
        btn_Create_Text.text = DataManager.instance.GetWordData("Create");
    }

    private void OnEnable()
    {
        itemPopup.SetActive(false);
    }

    private void Update()
    {
        InventoryManager.instance.RefreshIcon();
    }
}
