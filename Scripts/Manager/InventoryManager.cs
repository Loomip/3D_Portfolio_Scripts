using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//아이템의 고유 ID와 개수를 저장
public class ItemData
{
    public int id; //아이템 고유 ID
    public int amount; //아이템 갯수
    public int enhanceLevel; // 아이템의 강화 레벨
}

public class InventoryManager : Singleton<InventoryManager>
{
    //골드
    [SerializeField] TextMeshProUGUI goldText;

    // 현재 골드 를 저장하는 변수
    public int gold;

    //골드 UI를 리프레쉬 해주는 함수
    public void Refresh_Gold()
    {
        if (goldText != null)
            goldText.text = string.Format("{0: #,##0}", gold);
    }

    public void StartGold()
    {
        gold = Consts.START_GOLD;
    }

    //============================================================================================================

    [Header("인벤토리 최대 개수")]
    private int maxSlotCount = Consts.MAX_SLOT_COUNT;
    public int MAXSLOTCOUNT
    {
        get => maxSlotCount;
    }

    [Header("인벤토리 현재 개수")]
    private int curSlotCount;
    public int CUR_SLOT_COUNT
    {
        get => curSlotCount;
        set => curSlotCount = value;
    }

    public bool ItemExist(ItemData item)
    {
        return GetItemList().Contains(item);
    }

    // 장착된 무기의 참조
    protected GameObject equippedWeapon;

    public GameObject EquippedWeapon { get => equippedWeapon; set => equippedWeapon = value; }

    // 슬롯들이 저장되는 리스트
    List<Slot> slotList = new List<Slot>();

    public List<Slot> SlotList { get => slotList; set => slotList = value; }

    //현재 인벤토리에 있는 아이템 목록을 저장하는 리스트
    private List<ItemData> items = new List<ItemData>();

    public List<ItemData> GetItemList()
    {
        CUR_SLOT_COUNT = items.Count;
        return items;
    }

    //인벤토리의 슬롯을 초기화 하는 메서드
    public void InitSlots()
    {
        for (int i = 0; i < MAXSLOTCOUNT; i++)
        {
            Slot slot = new Slot();
            slotList.Add(slot);
        }
    }

    //인벤토리의 아이템 아이콘을 갱신하는 메서드
    public void RefreshIcon()
    {
        CUR_SLOT_COUNT = items.Count;

        for (int i = 0; i < MAXSLOTCOUNT; i++)
        {
            if (i < CUR_SLOT_COUNT)
            {
                if (items[i] != null)
                {
                    ItemData item = items[i];
                    SlotList[i].Set_Icon(item);
                }
                else
                {
                    SlotList[i].ClearSlot();
                }
            }
            else
            {
                SlotList[i].ClearSlot();
            }
        }
    }

    //주어진 아이템이 인벤토리에 있는 위치를 찾는 메서드
    private int FindItemIndex(ItemData newItem)
    {
        int result = -1;
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].id == newItem.id)
            {
                result = i;
                break;
            }
        }
        return result;
    }

    //인벤토리에 새 아이템을 추가하는 메서드
    public void AddItem(ItemData newItem)
    {
        int index = FindItemIndex(newItem);

        Data_Item.Param item = DataManager.instance.GetWeaponData(newItem.id);

        if (item != null)
        {
            if (-1 < index) //인벤토리에 있던 아이템
            {
                items[index].amount += 1;
            }
            else // 기존 인벤토리에 없던 아이템
            {
                newItem.id = item.ID;
                newItem.amount = 1;
                items.Add(newItem);
                curSlotCount++;
            }
        }

        RefreshIcon();
    }

    // 인벤토리에서 아이템을 제거하고 남은 아이템의 개수를 반환
    public void RemoveItem(ItemData newItem)
    {
        if (newItem == null)
        {
            Debug.Log("제거하려는 아이템이 null입니다.");
            return;
        }

        int index = FindItemIndex(newItem);

        if (index >= 0) // 인벤토리에 아이템이 존재할 경우
        {
            // 아이템의 개수 감소
            items[index].amount -= 1;

            // 아이템의 개수가 0이 되면 인벤토리에서 아이템 제거
            if (items[index].amount <= 0)
            {
                items.RemoveAt(index);
                curSlotCount--;
            }
        }
        else
        {
            Debug.Log("인벤토리에 없는 아이템을 제거하려고 시도했습니다.");
        }

        RefreshIcon();
    }

    protected override void DoAwake()
    {
        // 골드 초기화
        StartGold();
        InitSlots();
    }
}
