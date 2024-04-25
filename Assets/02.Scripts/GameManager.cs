using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //몬스터가 출현할 위치를 저장할 배열
    //public Transform[] points;

    //몬스터가 출현할 위치를 저장할 리스트
    public List<Transform> points = new List<Transform>();

    //몬스터를 미리 생성해 저장할 리스트 자료형
    public List<GameObject> monsterPool = new List<GameObject>();
    //오브젝트풀(Object Pool)에 생성할 몬스터의 최대 개수
    public int maxMonsters = 10;


    //몬스터 프리팹을 연결할 변수
    public GameObject monster;
    //몬스터의 생성간격
    public float createTime = 3.0f;
    //게임의 종료여부를 저장할 멤버변수
    private bool isGameOver;
    //게임의 종료여부를 저장할 프로퍼티 
    //캡슐레이션(Capsulation)
    public bool IsGameOver
    {
        get { return isGameOver; } //읽기
        set  //쓰기
        {
            isGameOver = value;
            if(isGameOver)
            {
                //Invoke 종료
                CancelInvoke("CreateMonster");
            }
        }
    }

    //싱글턴 인스턴스 선언
    public static GameManager instance = null;

    //스크립트가 실행되면 가장먼저 호출되는 유니티 이벤트 함수
    //p122 참고
    void Awake()
    {
        if(instance == null)
        {
            instance = this; //GameManager가 생성됐을때 객체 자신
        }
        // instance에 할당된 클래스의 인스턴스가 다를 경우
        //새로 생성된 클래스를 의미함
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        //다른씬으로 넘어가더라도 삭제하지 않고 유지
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //몬스터 오브젝트풀 생성
        CreateMonsterPool();

        //SpawnPointGroup 게임으보젝트의 Transform컴포넌트 추출 (Parent)
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;
       
        //SpawnPointGroup하위에 있는 모든 차일드 게임오브젝트 Transform 추출 (child)
        //points = spawnPointGroup?.GetComponentsInChildren<Transform>();
        //spawnPointGroup?.GetComponentsInChildren<Transform>(points);

        foreach(Transform point in spawnPointGroup) //(데이터형식 변수명 in 배열)
        {
            points.Add(point);
        }
        //일정한 시간 간격으로 함수를 호출
        //(호출할 함수, 대기시간, 호출간격)
        //게임 시작시 2초뒤에 함수실행, 3초간격으로 몬스터 생성
        //Invoke 시작
        InvokeRepeating("CreateMonster", 2.0f, createTime);
    }

    void CreateMonster()
    {
        //몬스터의 불규칙한 생성 위치 산출
        int idx = Random.Range(0, points.Count);
        //몬스터 프리팹 생성, 공장을 만든다, (생성할 객체, 위치, 회전)
        //Instantiate(monster, points[idx].position, points[idx].rotation);

        //오브젝트 풀에서 몬스터 추출
        GameObject _monster = GetMonsterInPool();
        //추출한 몬스터의 위치와 회전을 설정
        _monster?.transform.SetPositionAndRotation(points[idx].position,
                                                                          points[idx].rotation);
        //추출한 몬스터 활성화
        _monster?.SetActive(true);
    }
    //오브젝트 풀에 몬스터 생성
    void CreateMonsterPool()
    {
        for(int i=0; i<maxMonsters; i++)
        {
            //몬스터 생성// 몬스터를 오브젝트풀에서 가져와 배치할때 사용
            var _monster = Instantiate<GameObject>(monster);
            //몬스터 이름 지정 뒤에 몬스터에 숫자로 이름을 달아줌
            _monster.name = $"Monster_{i:00}";
            //몬스터 비활성화
            _monster.SetActive(false);
            //생성할 몬스터를 오브젝트풀에 추가
            monsterPool.Add(_monster);
        }
    }
    //오브젝트 풀에서 사용 가능한 몬스터를 추출해 반환
    public GameObject GetMonsterInPool()
    {
        //오브젝트 풀의 처음부터 끝까지 순회
        foreach(var _monster in monsterPool)
        //비활성화 여부로 사용가능한 몬스터 판단
        {
            if(_monster.activeSelf == false)
            {
                //몬스터반환
                return _monster;
            }
        }
        return null;
    }
}
