using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOption
{
    public string ButtonText { get; }
    public System.Action ButtonAction { get; }

    public DialogueOption(string buttonText, System.Action buttonAction)
    {
        ButtonText = buttonText;
        ButtonAction = buttonAction;
    }
}

public class NPC_Base : MonoBehaviour
{
    [Header("��ȭ UI")]
    [SerializeField] protected GameObject dialoguePanel;
    [SerializeField] protected TextMeshProUGUI dialogueText;
    [SerializeField] protected Transform buttonContainer; // ��ư�� ��� �θ� ��ü
    [SerializeField] protected GameObject buttonPrefab; // ��ư ������
    protected string dialogueMessage;

    public GameObject DialoguePanel { get => dialoguePanel; set => dialoguePanel = value; }

    // ������ �����ִ��� Ȯ���ϴ� �޼���
    public virtual bool IsShopOpen()
    {
        return false;
    }

    protected virtual void Start()
    {
        // �⺻ ��ȭ �޽��� ����
        dialogueMessage = "�ȳ��ϼ���!";
    }

    public virtual void OnInteract()
    {
        ShowDialogue(
            dialogueMessage,
            new List<DialogueOption>
            {
                new DialogueOption("��ư 1", () => { }),
                new DialogueOption("��ư 2", CloseDialogue)
            }
        );
    }

    protected void ShowDialogue(string message, List<DialogueOption> options)
    {
        DialoguePanel.SetActive(true);
        StartCoroutine(TypeDialogue(message));
        UpdateButtons(options);
    }

    protected IEnumerator TypeDialogue(string message)
    {
        dialogueText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f); // ���� ������ ������
        }
    }

    protected void UpdateButtons(List<DialogueOption> options)
    {
        // ���� ��ư ����
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // ���ο� ��ư ����
        foreach (var option in options)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = option.ButtonText;

            Button button = buttonObj.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { option.ButtonAction.Invoke(); });
        }
    }

    public virtual void CloseDialogue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UIManager.instance.Player.GetComponent<NPCInteraction>().CloseDialogue();
    }
}
