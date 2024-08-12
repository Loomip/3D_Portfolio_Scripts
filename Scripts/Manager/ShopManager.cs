using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("상점 UI")]
    [SerializeField] private GameObject shopObject;
    [SerializeField] private GameObject shopSlotPrefab;
    [SerializeField] private Transform shopContent;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button closeButton;

    [Header("Player 인벤토리 UI")]
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Transform inventoryContent;
    [SerializeField] private TextMeshProUGUI goldText;

    private List<GameObject> shopSlots = new List<GameObject>(); // 생성된 슬롯을 저장할 리스트
    private List<Slot> inventorySlots = new List<Slot>();
    private ItemData playerSelectedItem; // Player 인벤에서 선택된 아이템
    private Data_Shop.Param selectedItem; // 현재 선택된 아이템
    private ShopSlot selectedSlot;
    private Slot PlayerSlot;

    private bool shopOpen = false;
    private NPC_Shop currentNPC;

    public bool ShopOpen { get => shopOpen; set => shopOpen = value; }

    private void Start()
    {
        buyButton.onClick.AddListener(Buy);
        sellButton.onClick.AddListener(Sell);
        closeButton.onClick.AddListener(Close);
    }

    public void OpenShop(NPC_Shop npc)
    {
        ShopOpen = true;
        shopObject.SetActive(ShopOpen);

        currentNPC = npc;

        // 이전에 생성된 슬롯 제거
        ClearShopSlots();

        buyButton.GetComponentInChildren<TextMeshProUGUI>().text = DataManager.instance.GetWordData("Purchase");
        sellButton.GetComponentInChildren<TextMeshProUGUI>().text = DataManager.instance.GetWordData("Sell");
        closeButton.GetComponentInChildren<TextMeshProUGUI>().text = DataManager.instance.GetWordData("Exit");

        goldText.text = string.Format("{0: #,##0}", InventoryManager.instance.gold);

        RefreshIcon();

        // 상점 아이템 목록을 초기화합니다.
        List<Data_Shop.Param> shopItems = DataManager.instance.GetAllShopItems();

        foreach (Data_Shop.Param shopItem in shopItems)
        {
            // 상점 슬롯 프리팹을 복제하여 슬롯을 생성합니다.
            GameObject shopSlotObject = Instantiate(shopSlotPrefab, shopContent);

            // 슬롯에 상점 아이템 데이터를 설정합니다.
            ShopSlot shopSlot = shopSlotObject.GetComponent<ShopSlot>();
            shopSlot.SetShopData(shopItem, this);

            // 생성된 슬롯을 리스트에 추가
            shopSlots.Add(shopSlotObject);
        }

        currentNPC.DialoguePanel.SetActive(false);
    }

    private void ClearShopSlots()
    {
        foreach (GameObject slot in shopSlots)
        {
            Destroy(slot);
        }
        shopSlots.Clear();
    }

    public void SetSelectedItem(Data_Shop.Param item)  // 선택된 아이템을 설정하는 함수
    {
        if (selectedSlot != null)
        {
            selectedSlot.Deselect();
        }

        selectedItem = item;
        selectedSlot = shopSlots.Find(slot => slot.GetComponent<ShopSlot>().shopData == item).GetComponent<ShopSlot>();

        if (selectedSlot != null)
        {
            selectedSlot.Select();
        }
    }

    public void PlayerInven()
    {
        // 플레이어의 아이템 목록을 가져옵니다.
        List<Slot> playerSlots = InventoryManager.instance.SlotList;

        for (int i = 0; i < playerSlots.Count; ++i)
        {
            // 슬롯에 플레이어의 아이템 데이터를 설정합니다.
            PlayerSlot = Instantiate(inventorySlotPrefab, inventoryContent).GetComponent<Slot>();
            PlayerSlot.SLOTINDEX = i;

            // 생성된 슬롯을 리스트에 추가
            inventorySlots.Add(PlayerSlot);

            PlayerSlot.onItemClick += SelectItem;
        }
    }

    // Player 인벤토리 아이템 선택 (슬롯 클릭 시 호출)
    public void SelectItem(ItemData selectedItem)
    {
        playerSelectedItem = selectedItem;
    }

    void RefreshIcon()
    {
        List<ItemData> dataList = InventoryManager.instance.GetItemList();

        // 모든 슬롯을 클리어합니다.
        foreach (Slot slot in inventorySlots)
        {
            slot.ClearSlot();
        }

        // 아이템 목록을 순회하면서 각 슬롯의 아이콘을 설정합니다.
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i] != null)
            {
                ItemData item = dataList[i];
                inventorySlots[i].Set_Icon(item);
            }
        }
    }

    // 사기 (버튼에 다는 기능)
    public void Buy()
    {
        if (selectedItem != null)
        {
            // 아이템의 가격이 현재 골드보다 많은지 확인
            if (selectedItem.AddPrise <= InventoryManager.instance.gold)
            {
                // 현재 골드에서 아이템의 가격을 차감
                InventoryManager.instance.gold -= selectedItem.AddPrise;
                InventoryManager.instance.Refresh_Gold(); // 골드 UI 업데이트

                // 상점 골드 텍스트 업데이트
                goldText.text = string.Format("{0: #,##0}", InventoryManager.instance.gold);

                // 선택된 아이템 데이터로 새 아이템 생성
                ItemData newItem = new ItemData();
                newItem.id = selectedItem.ID;
                ++newItem.amount;

                // 새 아이템을 인벤토리에 추가
                InventoryManager.instance.AddItem(newItem);

                // 상점 인벤토리 UI를 업데이트합니다.
                RefreshIcon();

                // 플레이어 인벤토리 UI를 업데이트 합니다.
                InventoryManager.instance.RefreshIcon();
            }
            else
            {
                Debug.Log("돈이 없습니다.");
            }
        }
        else
        {
            Debug.Log("상점에서 아이템이 선택되어지지 않았습니다.");
        }
    }

    // 팔기
    public void SellSelected(ItemData pSelectedItem)
    {
        // 선택된 아이템이 있는지 확인합니다.
        if (pSelectedItem != null)
        {
            Data_Shop.Param prise = DataManager.instance.GetShopData(pSelectedItem.id);

            // 현재 골드에 아이템의 가격을 더해줍니다.
            InventoryManager.instance.gold += prise.SalePrise;
            InventoryManager.instance.Refresh_Gold(); // 골드 UI 업데이트

            // 상점 골드 텍스트 업데이트
            goldText.text = string.Format("{0: #,##0}", InventoryManager.instance.gold);

            // 인벤토리에서 아이템을 제거합니다.
            List<ItemData> items = InventoryManager.instance.GetItemList();
            ItemData itemToRemove = items.Find(item => item.id == pSelectedItem.id);
            if (itemToRemove != null)
            {
                // 아이템의 개수 감소
                itemToRemove.amount -= 1;

                // 아이템의 개수가 0이 되면 인벤토리에서 아이템 제거
                if (itemToRemove.amount <= 0)
                {
                    InventoryManager.instance.RemoveItem(itemToRemove);
                    pSelectedItem = null;
                }
            }

            // 상점 인벤토리 UI를 업데이트합니다.
            RefreshIcon();

            // 플레이어 인벤토리 UI를 업데이트 합니다.
            InventoryManager.instance.RefreshIcon();
        }
        else
        {
            Debug.Log("인벤토리에 선택되어진 아이템이 없습니다.");
        }
    }

    // 아이템 판매 (버튼에 달 기능)
    public void Sell()
    {
        SellSelected(playerSelectedItem);
    }

    // 나가기 (버튼에 다는 기능)
    public void Close()
    {
        // 이전에 생성된 슬롯 제거
        ClearShopSlots();

        ShopOpen = false;
        shopObject.SetActive(ShopOpen);

        currentNPC.CloseDialogue();
    }
}
