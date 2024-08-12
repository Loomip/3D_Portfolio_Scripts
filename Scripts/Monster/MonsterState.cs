using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class MonsterState : MonoBehaviour
{
    // 애니메이터 참조
    protected Animator animator;

    // 네비 컴포넌트 참조
    protected NavMeshAgent nav;

    // 캐릭터 스텟 참조
    protected CharacterState state;

    // 몬스터 유한상태기계 컨트롤러
    protected MonsterFSMController controller;

    // 히트 되면 바뀔 몸 메터리얼
    protected SkinnedMeshRenderer meshs;

    // 체력 컴포넌트
    protected EHeath monsterHp;

    // 사냥터 컴포넌트
    protected HuntingGround huntingGround;

    public HuntingGround Ground { get => huntingGround; set => huntingGround = value; }

    // 몬스터 상태 관련 인터페이스(문법아님) 메소드 선언

    // 몬스터 상태 시작 (다른상태로 전이됨) 메소드
    public abstract void EnterState(e_MonsterState state);

    // 몬스터 상태 업데이트 추상 메소드 (상태 동작 수행)
    public abstract void UpdateState();

    // 몬스터 상태 종료 (다른상태로 전이됨) 메소드
    public abstract void ExitState();

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        state = GetComponent<CharacterState>();
        controller = GetComponent<MonsterFSMController>();
        monsterHp = GetComponent<EHeath>();
        animator = GetComponent<Animator>();
        meshs = GetComponentInChildren<SkinnedMeshRenderer>();
        huntingGround = FindObjectOfType<HuntingGround>();
    }

    // ClassroomController 설정
    public void SetHuntingGroundController(HuntingGround controller)
    {
        huntingGround = controller;
    }
}
