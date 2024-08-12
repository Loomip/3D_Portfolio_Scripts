using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] Image img_Frame;
    [SerializeField] Image img_Selected;
    [SerializeField] protected Image img_Icon;
    [SerializeField] TextMeshProUGUI txt_Amount;

    private bool isSelect;

    public bool SELECT
    {
        get => isSelect;
    }

    private int slotIndex;

    //슬롯의 인덱스를 반환하거나 설정하는 속성
    public int SLOTINDEX
    {
        get => slotIndex;
        set => slotIndex = value;
    }

    //아이템 클릭 이벤트
    public event Action<ItemData> onItemClick;

    //현재 슬롯의 아이템 데이터를 저장
    protected ItemData currentItem;
    public ItemData CurrentItem { get => currentItem; set => currentItem = value; }

    private Color iconColor;

    //슬롯의 아이콘을 설정하는 메서드
    public void Set_Icon(ItemData item)
    {
        CurrentItem = item;

        Data_Item.Param data = DataManager.instance.GetWeaponData(item.id);

        if (data != null)
        {
            img_Icon.sprite = Resources.Load<Sprite>("Itemicons/" + data.SpritName);
            ChangeAmount(item.amount);
            txt_Amount.enabled = true;
            iconColor = img_Icon.color;
            iconColor.a = 1f;
            img_Icon.color = iconColor;
        }
        else
        {
            Debug.Log("Item data is null for id: " + item.id);
        }
    }

    //슬롯을 비우는 메서드
    public void ClearSlot()
    {
        img_Selected.enabled = false;
        txt_Amount.enabled = false;
        iconColor = img_Icon.color;
        iconColor.a = 0f;
        img_Icon.color = iconColor;
        currentItem = null;
    }

    //슬롯의 아이템 개수를 변경하는 메서드
    public void ChangeAmount(int newAmount)
    {
        txt_Amount.text = newAmount.ToString();
    }

    //슬롯 선택 처리
    public void ItemSelect()
    {
        onItemClick?.Invoke(CurrentItem);
        if (CurrentItem == null) return;

        if (!isSelect)
        {
            img_Selected.enabled = true;
            isSelect = true;
        }
    }

    private void Start()
    {
        ClearSlot();
    }
}
