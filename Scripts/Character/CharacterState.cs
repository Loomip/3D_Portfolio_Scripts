using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CharacterState : MonoBehaviour
{
    [SerializeField] private string characterName; // 캐릭터 이름
    public string CharacterName { get => characterName; set => characterName = value; }

    // 능력치 설정
    private int atk;
    private int def;
    private int spd;
    private int hp;
    private int maxHp;
    private int mp;
    private int maxMp;
    private int exhaustion;
    private int cooldown;

    // 설정된 능력치를 가져오는 프로퍼티
    public int Atk { get => atk; set => atk = value; }
    public int Def { get => def; set => def = value; }
    public int Spd { get => spd; set => spd = value; }
    public int Hp { get => hp; set => hp = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Mp { get => mp; set => mp = value; }
    public int MaxMp { get => maxMp; set => maxMp = value; }
    public int Exhaustion { get => exhaustion; set => exhaustion = value; }
    public int Cooldown { get => cooldown; set => cooldown = value; }

    void Start()
    {
        InitCharacterStats();
    }

    // 스탯 데이터를 사전에 저장하는 메소드
    void InitCharacterStats()
    {
        // DataManager를 사용하여 이 캐릭터의 스탯 데이터를 가져옴
        Data_Character.Param stats = DataManager.instance.GetCharacterData(CharacterName);

        Atk = stats.Atk;
        Def = stats.Def;
        Spd = stats.Spd;
        Hp = stats.Hp;
        MaxHp = stats.MaxHp;
        Mp = stats.Mp;
        MaxMp = stats.MaxMp;
        Exhaustion = 0;
        Cooldown = 0;
    }

    public void AddAtk(int value)
    {
        Atk += value;
    }

    public void AddDef(int value)
    {
        Def += value;
    }

    public void AddSpd(int value)
    {
        Spd += value;
    }

    public void AddHp(int value)
    {
        Hp = Mathf.Min(Hp + value, MaxHp);
    }

    public void AddMaxHp(int value)
    {
        MaxHp += value;
        Hp = Mathf.Min(Hp, MaxHp);
    }

    public void AddMp(int value)
    {
        Mp = Mathf.Min(Mp + value, MaxMp);
    }

    public void AddMaxMp(int value)
    {
        MaxMp += value;
        Mp = Mathf.Min(Mp, MaxMp);
    }

    public void AddExhaustion(int value)
    {
        Exhaustion += value;
    }

    public void AddCooldown(int value)
    {
        Cooldown += value;
    }

    public void RemoveAtk(int value)
    {
        Atk = Mathf.Max(0, Atk - value);
    }

    public void RemoveDef(int value)
    {
        Def = Mathf.Max(0, Def - value);
    }

    public void RemoveSpd(int value)
    {
        Spd = Mathf.Max(0, Spd - value);
    }

    public void RemoveHp(int value)
    {
        Hp = Mathf.Max(0, Hp - value);
    }

    public void RemoveMaxHp(int value)
    {
        MaxHp = Mathf.Max(0, MaxHp - value);
    }

    public void RemoveMp(int value)
    {
        Mp = Mathf.Max(0, Mp - value);
    }

    public void RemoveMaxMp(int value)
    {
        MaxMp = Mathf.Max(0, MaxMp - value);
    }

    public void RemoveExhaustion(int value)
    {
        Exhaustion -= value;
    }

    public void RemoveCooldown(int value)
    {
        Cooldown -= value;
    }
}
