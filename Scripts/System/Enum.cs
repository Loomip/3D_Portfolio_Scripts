using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum e_StatType
{
    Atk,
    Def,
    Spd,
    Hp,
    MaxHp,
    Mp,
    MaxMp,
    Exhaustion,
    Cooldown
}

public enum e_MenuType
{
    None,
    Status,
    ItemSlot,
    Settings,
    Length
}

public enum e_EquipType
{
    Weapon,     //무기슬롯
    Length
}

enum e_Weapon
{
    None,
    Weapon,
    Spell
}

public enum e_MonsterState
{
    Idle,       // 평시상태
    Run,        // 걸어다니는 상태
    Attack,     // 공격 상태
    Hit,        // 히트 상태
    Die         // 죽음 상태
}

public enum e_BossState
{
    Idle,       // 평시상태
    Run,        // 걸어다니는 상태
    Attack,     // 공격 상태
    Skill1,     // 패턴1
    Skill2,     // 패턴2
    Hit,        // 히트 상태
    Die         // 죽음 상태
}

public enum e_SkillSound
{
    melee,
    Ranged
}

public enum e_Bgm
{
    Title,
    Start,
}

public enum e_Sfx
{
    Bullet,
    Sword,
    SwordSkill = 4,
    BulletSkill,
    Explosion,
    BossAtteck,
    BossSkill,
    BossSkill2,
    EnemyDie,
    EnemyDieEffectSound,
    Hit
}

