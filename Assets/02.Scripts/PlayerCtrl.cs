using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    //컴포넌트 캐시를 처리할 변수
    private Transform tr;
    //이동 속도 변수(public으로 선언되어 인스펙터뷰에 노출됨)
    public float moveSpeed = 10.0f;
    //회전속도 변수
    public float turnSpeed = 80.0f;
    //Animation 컴포넌트를 저장할 변수
    private Animation anim;

    //초기 생명값
    private readonly float initHp = 100.0f;
    //현재 생명값
    public float currHp;
    //Hpbar 연결할 변수
    private Image hpBar;

    //델리게이트 선언 1
    public delegate void PlayerDieHandler();
    //델리게이트 이벤트 선언 1
    public static event PlayerDieHandler OnPlayerDie;

    //제일 먼저 실행되고 한번만 실행
    // Start is called before the first frame update
    IEnumerator Start()
    {
        //Hpbar연결
        hpBar = GameObject.FindGameObjectWithTag("HP_BAR")?.GetComponent<Image>();

        //HP 초기화
        currHp = initHp;
        DisplayHealth();

        //컴포넌트를 추출해 변수에 대입
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        //애니메이션 실행
        anim.Play("Idle");

        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.3f);
        turnSpeed = 80.0f;
    }
    //1초당 60개 프레임으로 계속 업데이트
    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");//Input아 니가 가진 기능중에 GetAxis함수좀 불러와봐
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

        //전후좌우 이동방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //Translate를 사용한 이동로직(이동방향*속도*시간정규화)
        tr.Translate(moveDir.normalized* moveSpeed * Time.deltaTime);

        //Vector3.up 축을 기준으로 turnSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);

        //주인공 캐릭터 애니메이션 설정
        PlayerAnim(h, v);
      }
    void PlayerAnim(float h, float v)
    {
        //키보드 입력값을 기준으로 동작할 애니메이션 수행
        if(v >= 0.1f)
        {
            anim.CrossFade("RunF", 0.25f);// 전진 애니메이션 실행
        }
        else if (v <= -0.1f)
        {
            anim.CrossFade("RunB", 0.25f);// 후진 애니메이션 실행
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade("RunR", 0.25f);// 오른쪽 이동 애니메이션 실행
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade("RunL", 0.25f);// 왼쪽 이동 애니메이션 실행
        }
        else
        {
            anim.CrossFade("Idle", 0.25f);  //정지시 Idle 애니메이션 실행
        }
    }
    //충돌한 한의 IsTigger 옵션이 체크됐을때 발생
    void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider가 몬스터의 PUNCH이면 Player의 Hp차감
        if(currHp >= 0.0f && coll.CompareTag("PUNCH"))
        {
            currHp -= 10.0f;
            DisplayHealth();

            Debug.Log($"Player HP = { currHp / initHp}");

            //player의 생명이 0이하면 사망 처리
            if(currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }
    //Player의 사망처리
    void PlayerDie()
    {
        Debug.Log("Player Die!");

        //MONSTER 태그를 가진 모든 게임오브젝트를 찾아옴
        //GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        //모든 몬스터의 OnPlayerDie 함수를 순차적으로 호출
        // foreach(GameObject monster in monsters) //(데이터 형식 변수명 in 배열)
        //{
        //    monster.SendMessage("OnPlayerDie",
        //                                        SendMessageOptions.DontRequireReceiver);
        //}

        //델리게이트 2 주인공 사망 이벤트 호출 발생
        OnPlayerDie();

        //GameManager 스크립트 IsGameOver 프로퍼티를 true로 만든다.
        //                          Object명                        컴포넌트(스크립트명)   멤버변수
        //GameObject.Find("GameMgr").GetComponent<GameManager>().IsGameOver = true;
        GameManager.instance.IsGameOver = true;// 값을 간단히 넘겨줌, 코드간결, GetComponent로 계속 부를 필요 없음

    }

    void DisplayHealth()
    {
        hpBar.fillAmount = currHp / initHp;
    }
}
