using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("���� UI")]
    [SerializeField] private GameObject shopObject;
    [SerializeField] private GameObject shopSlotPrefab;
    [SerializeField] private Transform shopContent;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button closeButton;

    [Header("Player �κ��丮 UI")]
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Transform inventoryContent;
    [SerializeField] private TextMeshProUGUI goldText;

    private List<GameObject> shopSlots = new List<GameObject>(); // ������ ������ ������ ����Ʈ
    private List<Slot> inventorySlots = new List<Slot>();
    private ItemData playerSelectedItem; // Player �κ����� ���õ� ������
    private Data_Shop.Param selectedItem; // ���� ���õ� ������
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

        // ������ ������ ���� ����
        ClearShopSlots();

        buyButton.GetComponentInChildren<TextMeshProUGUI>().text = DataManager.instance.GetWordData("Purchase");
        sellButton.GetComponentInChildren<TextMeshProUGUI>().text = DataManager.instance.GetWordData("Sell");
        closeButton.GetComponentInChildren<TextMeshProUGUI>().text = DataManager.instance.GetWordData("Exit");

        goldText.text = string.Format("{0: #,##0}", InventoryManager.instance.gold);

        RefreshIcon();

        // ���� ������ ����� �ʱ�ȭ�մϴ�.
        List<Data_Shop.Param> shopItems = DataManager.instance.GetAllShopItems();

        foreach (Data_Shop.Param shopItem in shopItems)
        {
            // ���� ���� �������� �����Ͽ� ������ �����մϴ�.
            GameObject shopSlotObject = Instantiate(shopSlotPrefab, shopContent);

            // ���Կ� ���� ������ �����͸� �����մϴ�.
            ShopSlot shopSlot = shopSlotObject.GetComponent<ShopSlot>();
            shopSlot.SetShopData(shopItem, this);

            // ������ ������ ����Ʈ�� �߰�
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

    public void SetSelectedItem(Data_Shop.Param item)  // ���õ� �������� �����ϴ� �Լ�
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
        // �÷��̾��� ������ ����� �����ɴϴ�.
        List<Slot> playerSlots = InventoryManager.instance.SlotList;

        for (int i = 0; i < playerSlots.Count; ++i)
        {
            // ���Կ� �÷��̾��� ������ �����͸� �����մϴ�.
            PlayerSlot = Instantiate(inventorySlotPrefab, inventoryContent).GetComponent<Slot>();
            PlayerSlot.SLOTINDEX = i;

            // ������ ������ ����Ʈ�� �߰�
            inventorySlots.Add(PlayerSlot);

            PlayerSlot.onItemClick += SelectItem;
        }
    }

    // Player �κ��丮 ������ ���� (���� Ŭ�� �� ȣ��)
    public void SelectItem(ItemData selectedItem)
    {
        playerSelectedItem = selectedItem;
    }

    void RefreshIcon()
    {
        List<ItemData> dataList = InventoryManager.instance.GetItemList();

        // ��� ������ Ŭ�����մϴ�.
        foreach (Slot slot in inventorySlots)
        {
            slot.ClearSlot();
        }

        // ������ ����� ��ȸ�ϸ鼭 �� ������ �������� �����մϴ�.
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i] != null)
            {
                ItemData item = dataList[i];
                inventorySlots[i].Set_Icon(item);
            }
        }
    }

    // ��� (��ư�� �ٴ� ���)
    public void Buy()
    {
        if (selectedItem != null)
        {
            // �������� ������ ���� ��庸�� ������ Ȯ��
            if (selectedItem.AddPrise <= InventoryManager.instance.gold)
            {
                // ���� ��忡�� �������� ������ ����
                InventoryManager.instance.gold -= selectedItem.AddPrise;
                InventoryManager.instance.Refresh_Gold(); // ��� UI ������Ʈ

                // ���� ��� �ؽ�Ʈ ������Ʈ
                goldText.text = string.Format("{0: #,##0}", InventoryManager.instance.gold);

                // ���õ� ������ �����ͷ� �� ������ ����
                ItemData newItem = new ItemData();
                newItem.id = selectedItem.ID;
                ++newItem.amount;

                // �� �������� �κ��丮�� �߰�
                InventoryManager.instance.AddItem(newItem);

                // ���� �κ��丮 UI�� ������Ʈ�մϴ�.
                RefreshIcon();

                // �÷��̾� �κ��丮 UI�� ������Ʈ �մϴ�.
                InventoryManager.instance.RefreshIcon();
            }
            else
            {
                Debug.Log("���� �����ϴ�.");
            }
        }
        else
        {
            Debug.Log("�������� �������� ���õǾ����� �ʾҽ��ϴ�.");
        }
    }

    // �ȱ�
    public void SellSelected(ItemData pSelectedItem)
    {
        // ���õ� �������� �ִ��� Ȯ���մϴ�.
        if (pSelectedItem != null)
        {
            Data_Shop.Param prise = DataManager.instance.GetShopData(pSelectedItem.id);

            // ���� ��忡 �������� ������ �����ݴϴ�.
            InventoryManager.instance.gold += prise.SalePrise;
            InventoryManager.instance.Refresh_Gold(); // ��� UI ������Ʈ

            // ���� ��� �ؽ�Ʈ ������Ʈ
            goldText.text = string.Format("{0: #,##0}", InventoryManager.instance.gold);

            // �κ��丮���� �������� �����մϴ�.
            List<ItemData> items = InventoryManager.instance.GetItemList();
            ItemData itemToRemove = items.Find(item => item.id == pSelectedItem.id);
            if (itemToRemove != null)
            {
                // �������� ���� ����
                itemToRemove.amount -= 1;

                // �������� ������ 0�� �Ǹ� �κ��丮���� ������ ����
                if (itemToRemove.amount <= 0)
                {
                    InventoryManager.instance.RemoveItem(itemToRemove);
                    pSelectedItem = null;
                }
            }

            // ���� �κ��丮 UI�� ������Ʈ�մϴ�.
            RefreshIcon();

            // �÷��̾� �κ��丮 UI�� ������Ʈ �մϴ�.
            InventoryManager.instance.RefreshIcon();
        }
        else
        {
            Debug.Log("�κ��丮�� ���õǾ��� �������� �����ϴ�.");
        }
    }

    // ������ �Ǹ� (��ư�� �� ���)
    public void Sell()
    {
        SellSelected(playerSelectedItem);
    }

    // ������ (��ư�� �ٴ� ���)
    public void Close()
    {
        // ������ ������ ���� ����
        ClearShopSlots();

        ShopOpen = false;
        shopObject.SetActive(ShopOpen);

        currentNPC.CloseDialogue();
    }
}
