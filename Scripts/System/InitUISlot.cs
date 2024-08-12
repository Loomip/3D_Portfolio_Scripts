using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitUISlot : MonoBehaviour
{
    [SerializeField] private ItemSlot itemSlot;
    [SerializeField] private UpgradeShop upgrade;
    [SerializeField] private ShopManager npcShop;

    private void Start()
    {
        InitializeAllSlots();
    }

    private void InitializeAllSlots()
    {
        if (itemSlot != null)
        {
            itemSlot.InitSlots();
        }

        if (upgrade != null)
        {
            upgrade.InitSlots();
        }

        if (npcShop != null)
        {
            npcShop.PlayerInven();
        }
    }
}
