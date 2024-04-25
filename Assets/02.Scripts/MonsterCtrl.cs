using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//네비게이션 기능을 사용하기 위해 추가해야 하는 네임 스페이스
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //몬스터의 상태 정보 1 기본파트
    public enum State
    {
        IDLE,
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    //1 기본파트
    //몬스터의 현재 상태
    public State state = State.IDLE;
    //추적사정거리
    public float traceDist = 10.0f;
    //공격사정거리
    public float attackDist = 2.0f;
    //몬스터의 사망 여부
    public bool isDie = false;

    //컴포넌트 캐시를 처리할 변수
    private Transform monterTR;
    private Transform playerTR;
    private NavMeshAgent agent;
    private Animator anim; //3 애니메이션

    //Animator의 파라미터를 추출 //3 애니메이션
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");

    //혈흔효과 프리팹
    private GameObject bloodEffect;
    //몬스터 생명 변수
    private int hp = 100;

    //델리게이트 3
    //스크립트가 활성화 될때마다 호출되는 함수
    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;

        //1.몬스터의 상태를 체크하는 코루틴 1 기본파트
        StartCoroutine(CheckMonsterState());
        //2.상태에 따라 몬스터의 행동을 수행하는 코루틴 함수 호출
        //2 행동구현 파트 
        StartCoroutine(MonsterAction());
    }
    //스크립트가 비활성화 될때마다 호출되는 함수
    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    // Start is called before the first frame update
    void Awake() //p122, 컴포넌트에 할당 로직수행, 스크립트를 미리 활성화 하여 수행
    {
        //몬스터의 Transform 할당
        monterTR = GetComponent<Transform>();
        //추적대상자인 Player의 Transform 할당
        playerTR = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        //NavMeshagent 컴포넌트 할당 
        agent = GetComponent<NavMeshAgent>();

        //Animator 컴포넌트 할당 //3 애니메이션
        anim = GetComponent<Animator>();

        //BloodSprayEffect 프리팹 로드
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");

        //추적 대상의 위치를 파악하면 바로 추적
        //agent.destination = playerTR.position;        
    }
    //일정 간격으로 몬스터의 행동 상태를 체크 1 기본파트
    IEnumerator CheckMonsterState() 
    {
        while(!isDie)
        {
            //0.3초동안 중지하는 동안 제어권 양보
            yield return new WaitForSeconds(0.3f);

            //몬스터의 상태가 DIE일때 코루틴을 종료
            if (state == State.DIE)
                yield break;

            //몬스터와 주인공 캐릭터 사이의 거리 측정, 몬스터 기준
            float distance = Vector3.Distance(playerTR.position,
                                                             monterTR.position);
            //공격 사정거리 범위로 들어왔는지 확인
            if(distance <= attackDist)
            {
                state = State.ATTACK;
            }
            //추적 사정거리 범위로 들어왔는지 확인
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }
        }
    }
    //몬스터의 상태에 따라 몬스터의 동작을 수행 2 행동구현 파트 
    IEnumerator MonsterAction()
    {
        while(!isDie)
        {
            switch(state)
            {
                //IDLE 상태
                case State.IDLE:
                    //추적중지
                    //isStopped= NavMeshagent의 속성변수
                    agent.isStopped = true;

                    //Animator의 IsTrace 변수를 false로 설정
                    anim.SetBool(hashTrace, false); //3 애니메이션
                    break;

                //추적 상태
                case State.TRACE:
                    //추적 대상의 좌표로 이동                   
                    agent.SetDestination(playerTR.position);
                    agent.isStopped = false;
                    //Animator의 IsTrace 변수를 true로 설정
                    anim.SetBool(hashTrace, true); //3 애니메이션

                    //Animator의 IsAttack 변수를 false로 설정
                    anim.SetBool(hashAttack, false); //3 애니메이션 
                    break;

                //공격상태
                case State.ATTACK:
                    //Animator의 IsAttack 변수를 true로 설정
                    anim.SetBool(hashAttack, true); //3 애니메이션
                    break;

                //사망
                case State.DIE:
                    isDie = true;
                    //추적중지
                    agent.isStopped = true; // NavMeshAgent 내부 프로퍼티 true:멈춘다
                    //사망애니메이션실행
                    anim.SetTrigger(hashDie);
                    //몬스터의 Collider 컴포넌트 비활성화
                    GetComponent<CapsuleCollider>().enabled = false;

                    //일정시간 대기후 오브젝트 풀링으로 환원
                    yield return new WaitForSeconds(3.0f);

                    //몬스터를 비활성화
                    this.gameObject.SetActive(false);
                    //사망후 다시사용할 때를 위해 hp값을 초기화
                    hp = 100;
                    isDie = false;
                    //몬스터의 Collider를 활성화
                    GetComponent<CapsuleCollider>().enabled = true;


                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.CompareTag("BULLET"))
        {
            //충돌한 게임오브젝트 삭제 너죽고
            Destroy(coll.gameObject);
            //피격 리액션 애니메이션 실행
            anim.SetTrigger(hashHit);

            //총알의 충돌지점
            Vector3 pos = coll.GetContact(0).point;
            //총알의 충돌 지점의 법선벡터
            Quaternion rot = Quaternion.LookRotation(-coll.GetContact(0).normal);
            //혈흔효과를 생성하는 함수 호출
            ShowBloodEffect(pos, rot);

            //몬스터의 hp차감
            hp -= 50;
            if(hp <=0)
            {
                state = State.DIE;
            }
        }
    }
    void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        //혈흔효과 생성 공장을 만들어 동적 생성
        GameObject blood = Instantiate<GameObject>(bloodEffect,
                                                                              pos,
                                                                              rot,
                                                                              monterTR);
        Destroy(blood, 1.0f);
    }
    void OnDrawGizmos()
    {
        //추적 사정거리 표시
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position,
                                                 traceDist);
        }
        //공격 사정거리 표시
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,
                                                 attackDist);
        }
    }
    void OnPlayerDie()
    {
        //몬스터의 상태를 체크하는 코루틴을 모두 정지
        StopAllCoroutines();
        //추적을 중지하고 애니메이션 실행
        agent.isStopped = true;
        anim.SetFloat(hashSpeed, Random.Range(0.5f, 1.5f));
        anim.SetTrigger(hashPlayerDie);
    }
}
