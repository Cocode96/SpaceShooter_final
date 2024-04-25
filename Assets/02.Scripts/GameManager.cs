using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //���Ͱ� ������ ��ġ�� ������ �迭
    //public Transform[] points;

    //���Ͱ� ������ ��ġ�� ������ ����Ʈ
    public List<Transform> points = new List<Transform>();

    //���͸� �̸� ������ ������ ����Ʈ �ڷ���
    public List<GameObject> monsterPool = new List<GameObject>();
    //������ƮǮ(Object Pool)�� ������ ������ �ִ� ����
    public int maxMonsters = 10;


    //���� �������� ������ ����
    public GameObject monster;
    //������ ��������
    public float createTime = 3.0f;
    //������ ���Ῡ�θ� ������ �������
    private bool isGameOver;
    //������ ���Ῡ�θ� ������ ������Ƽ 
    //ĸ�����̼�(Capsulation)
    public bool IsGameOver
    {
        get { return isGameOver; } //�б�
        set  //����
        {
            isGameOver = value;
            if(isGameOver)
            {
                //Invoke ����
                CancelInvoke("CreateMonster");
            }
        }
    }

    //�̱��� �ν��Ͻ� ����
    public static GameManager instance = null;

    //��ũ��Ʈ�� ����Ǹ� ������� ȣ��Ǵ� ����Ƽ �̺�Ʈ �Լ�
    //p122 ����
    void Awake()
    {
        if(instance == null)
        {
            instance = this; //GameManager�� ���������� ��ü �ڽ�
        }
        // instance�� �Ҵ�� Ŭ������ �ν��Ͻ��� �ٸ� ���
        //���� ������ Ŭ������ �ǹ���
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        //�ٸ������� �Ѿ���� �������� �ʰ� ����
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //���� ������ƮǮ ����
        CreateMonsterPool();

        //SpawnPointGroup ����������Ʈ�� Transform������Ʈ ���� (Parent)
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;
       
        //SpawnPointGroup������ �ִ� ��� ���ϵ� ���ӿ�����Ʈ Transform ���� (child)
        //points = spawnPointGroup?.GetComponentsInChildren<Transform>();
        //spawnPointGroup?.GetComponentsInChildren<Transform>(points);

        foreach(Transform point in spawnPointGroup) //(���������� ������ in �迭)
        {
            points.Add(point);
        }
        //������ �ð� �������� �Լ��� ȣ��
        //(ȣ���� �Լ�, ���ð�, ȣ�Ⱓ��)
        //���� ���۽� 2�ʵڿ� �Լ�����, 3�ʰ������� ���� ����
        //Invoke ����
        InvokeRepeating("CreateMonster", 2.0f, createTime);
    }

    void CreateMonster()
    {
        //������ �ұ�Ģ�� ���� ��ġ ����
        int idx = Random.Range(0, points.Count);
        //���� ������ ����, ������ �����, (������ ��ü, ��ġ, ȸ��)
        //Instantiate(monster, points[idx].position, points[idx].rotation);

        //������Ʈ Ǯ���� ���� ����
        GameObject _monster = GetMonsterInPool();
        //������ ������ ��ġ�� ȸ���� ����
        _monster?.transform.SetPositionAndRotation(points[idx].position,
                                                                          points[idx].rotation);
        //������ ���� Ȱ��ȭ
        _monster?.SetActive(true);
    }
    //������Ʈ Ǯ�� ���� ����
    void CreateMonsterPool()
    {
        for(int i=0; i<maxMonsters; i++)
        {
            //���� ����// ���͸� ������ƮǮ���� ������ ��ġ�Ҷ� ���
            var _monster = Instantiate<GameObject>(monster);
            //���� �̸� ���� �ڿ� ���Ϳ� ���ڷ� �̸��� �޾���
            _monster.name = $"Monster_{i:00}";
            //���� ��Ȱ��ȭ
            _monster.SetActive(false);
            //������ ���͸� ������ƮǮ�� �߰�
            monsterPool.Add(_monster);
        }
    }
    //������Ʈ Ǯ���� ��� ������ ���͸� ������ ��ȯ
    public GameObject GetMonsterInPool()
    {
        //������Ʈ Ǯ�� ó������ ������ ��ȸ
        foreach(var _monster in monsterPool)
        //��Ȱ��ȭ ���η� ��밡���� ���� �Ǵ�
        {
            if(_monster.activeSelf == false)
            {
                //���͹�ȯ
                return _monster;
            }
        }
        return null;
    }
}
