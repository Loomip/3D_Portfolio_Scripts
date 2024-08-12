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

    // ������ �����ִ��� Ȯ���ϴ� �޼���
    public override bool IsShopOpen()
    {
        return shopManager.ShopOpen;
    }

    protected override void Start()
    {
        base.Start();
        dialogueMessage = "������ ���� ���� ȯ���մϴ�!";
    }

    public override void OnInteract()
    {
        ShowDialogue(
            dialogueMessage,
            new List<DialogueOption>
            {
                new DialogueOption("���� ����", OpenShop),
                new DialogueOption("������", CloseDialogue)
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
