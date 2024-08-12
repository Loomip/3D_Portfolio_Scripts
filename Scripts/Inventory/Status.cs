using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Status : MonoBehaviour
{
    [Header("장비")]
    //장비 슬롯 프리팹
    [SerializeField] private GameObject equipslotPrefab;
    //아이템 장비 슬롯들이 들어가는 위치
    [SerializeField] private Transform equiplist;

    [Header("플레이어 스텟")]
    // 체력
    [SerializeField] private TextMeshProUGUI hpText;
    // 마나
    [SerializeField] private TextMeshProUGUI mpText;
    // 공격력
    [SerializeField] private TextMeshProUGUI atkText;
    // 방어력
    [SerializeField] private TextMeshProUGUI defText;

    [Header("버튼")]
    [SerializeField] private TextMeshProUGUI but_Unbind_Text;

    // 인벤토리 참조
    [SerializeField] private ItemSlot itemSlot;

    // 플레이어 스텟 참조
    CharacterState player;

    // 장비 슬롯 참조
    EquipSlot equipslot;

    // 장비 타입에 대한 프리펩들을 담을 딕셔너리
    Dictionary<e_EquipType, EquipSlot> equipSlotList = new Dictionary<e_EquipType, EquipSlot>();

    public Dictionary<e_EquipType, EquipSlot> EquipSlotList { get => equipSlotList; set => equipSlotList = value; }

    //장비 슬롯 생성
    private void InitEquipSlots()
    {
        for (int i = 0; i < (int)e_EquipType.Length; ++i)
        {
            equipslot = Instantiate(equipslotPrefab, equiplist).GetComponent<EquipSlot>();
            equipslot.ClearSlot();
            EquipSlotList.Add((e_EquipType)i, equipslot);

            equipslot.onItemClick += OnItemClick;
        }
    }

    // 캐릭터 스텟 리프레쉬
    public void Refresh_Stat()
    {
        hpText.text = "HP: " + player.Hp + " / " + player.MaxHp;
        mpText.text = "MP: " + player.Mp + " / " + player.MaxMp;
        atkText.text = "Attack : " + player.Atk;
        defText.text = "Defense: " + player.Def;
    }

    // 아이템 클릭 이벤트 핸들러
    public void OnItemClick(ItemData item)
    {
        // 버튼 활성화
        but_Unbind_Text.transform.parent.gameObject.SetActive(item != null);

        // 버튼 이름 설정
        but_Unbind_Text.text = DataManager.instance.GetWordData("Unbind");

        // 현재 선택된 아이템 설정
        equipslot.CurrentItem = item;
    }

    // 장비 해제하기 (버튼에 직접 달 메소드)
    public void UnbindItem()
    {
        if (equipslot.CurrentItem != null)
        {
            string item = DataManager.instance.GetWeaponData(equipslot.CurrentItem.id).ItemType;

            if (item != null && item == "Weapon")
            {
                // 슬롯이 EquipSlot인 경우에만 아이템 해제
                if (equipslot != null)
                {
                    equipslot.Detach();
                    itemSlot.MoveController.Animator.SetInteger("WeaponState", (int)e_Weapon.None);
                    UIManager.instance.RefreshHp(player.tag, player.GetComponent<Health>());
                    UIManager.instance.RefreshPlayerMp(player.GetComponent<Health>());
                }

                // 현재 선택된 아이템 초기화
                equipslot.CurrentItem = null;
            }
        }
        else
        {
            Debug.Log("아이템이 존재하지 않습니다.");
        }

    }

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<CharacterState>();
        InitEquipSlots();
        OnItemClick(equipslot.CurrentItem);
    }

    private void Update()
    {
        Refresh_Stat();
    }

    private void OnEnable()
    {
        but_Unbind_Text.transform.parent.gameObject.SetActive(false);
    }
}
