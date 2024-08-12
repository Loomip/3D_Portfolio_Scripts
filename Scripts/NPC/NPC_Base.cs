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
    [Header("대화 UI")]
    [SerializeField] protected GameObject dialoguePanel;
    [SerializeField] protected TextMeshProUGUI dialogueText;
    [SerializeField] protected Transform buttonContainer; // 버튼을 담는 부모 객체
    [SerializeField] protected GameObject buttonPrefab; // 버튼 프리팹
    protected string dialogueMessage;

    public GameObject DialoguePanel { get => dialoguePanel; set => dialoguePanel = value; }

    // 상점이 열려있는지 확인하는 메서드
    public virtual bool IsShopOpen()
    {
        return false;
    }

    protected virtual void Start()
    {
        // 기본 대화 메시지 설정
        dialogueMessage = "안녕하세요!";
    }

    public virtual void OnInteract()
    {
        ShowDialogue(
            dialogueMessage,
            new List<DialogueOption>
            {
                new DialogueOption("버튼 1", () => { }),
                new DialogueOption("버튼 2", CloseDialogue)
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
            yield return new WaitForSeconds(0.05f); // 글자 사이의 딜레이
        }
    }

    protected void UpdateButtons(List<DialogueOption> options)
    {
        // 기존 버튼 제거
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // 새로운 버튼 생성
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
