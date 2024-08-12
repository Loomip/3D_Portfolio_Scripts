using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Shop : NPC_Base
{
    [Header("Shop Manager")]
    [SerializeField] private ShopManager shopManager;

    // 상점이 열려있는지 확인하는 메서드
    public override bool IsShopOpen()
    {
        return shopManager.ShopOpen;
    }

    protected override void Start()
    {
        base.Start();
        dialogueMessage = "상점에 오신 것을 환영합니다!";
    }

    public override void OnInteract()
    {
        ShowDialogue(
            dialogueMessage,
            new List<DialogueOption>
            {
                new DialogueOption("상점 열기", OpenShop),
                new DialogueOption("나가기", CloseDialogue)
            }
        );
    }

    private void OpenShop()
    {
        shopManager.OpenShop(this);
    }

    public override void CloseDialogue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UIManager.instance.Player.GetComponent<NPCInteraction>().CloseDialogue();
    }
}
