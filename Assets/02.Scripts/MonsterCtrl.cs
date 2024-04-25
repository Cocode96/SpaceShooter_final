using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//�׺���̼� ����� ����ϱ� ���� �߰��ؾ� �ϴ� ���� �����̽�
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //������ ���� ���� 1 �⺻��Ʈ
    public enum State
    {
        IDLE,
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    //1 �⺻��Ʈ
    //������ ���� ����
    public State state = State.IDLE;
    //���������Ÿ�
    public float traceDist = 10.0f;
    //���ݻ����Ÿ�
    public float attackDist = 2.0f;
    //������ ��� ����
    public bool isDie = false;

    //������Ʈ ĳ�ø� ó���� ����
    private Transform monterTR;
    private Transform playerTR;
    private NavMeshAgent agent;
    private Animator anim; //3 �ִϸ��̼�

    //Animator�� �Ķ���͸� ���� //3 �ִϸ��̼�
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");

    //����ȿ�� ������
    private GameObject bloodEffect;
    //���� ���� ����
    private int hp = 100;

    //��������Ʈ 3
    //��ũ��Ʈ�� Ȱ��ȭ �ɶ����� ȣ��Ǵ� �Լ�
    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;

        //1.������ ���¸� üũ�ϴ� �ڷ�ƾ 1 �⺻��Ʈ
        StartCoroutine(CheckMonsterState());
        //2.���¿� ���� ������ �ൿ�� �����ϴ� �ڷ�ƾ �Լ� ȣ��
        //2 �ൿ���� ��Ʈ 
        StartCoroutine(MonsterAction());
    }
    //��ũ��Ʈ�� ��Ȱ��ȭ �ɶ����� ȣ��Ǵ� �Լ�
    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    // Start is called before the first frame update
    void Awake() //p122, ������Ʈ�� �Ҵ� ��������, ��ũ��Ʈ�� �̸� Ȱ��ȭ �Ͽ� ����
    {
        //������ Transform �Ҵ�
        monterTR = GetComponent<Transform>();
        //����������� Player�� Transform �Ҵ�
        playerTR = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        //NavMeshagent ������Ʈ �Ҵ� 
        agent = GetComponent<NavMeshAgent>();

        //Animator ������Ʈ �Ҵ� //3 �ִϸ��̼�
        anim = GetComponent<Animator>();

        //BloodSprayEffect ������ �ε�
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");

        //���� ����� ��ġ�� �ľ��ϸ� �ٷ� ����
        //agent.destination = playerTR.position;        
    }
    //���� �������� ������ �ൿ ���¸� üũ 1 �⺻��Ʈ
    IEnumerator CheckMonsterState() 
    {
        while(!isDie)
        {
            //0.3�ʵ��� �����ϴ� ���� ����� �纸
            yield return new WaitForSeconds(0.3f);

            //������ ���°� DIE�϶� �ڷ�ƾ�� ����
            if (state == State.DIE)
                yield break;

            //���Ϳ� ���ΰ� ĳ���� ������ �Ÿ� ����, ���� ����
            float distance = Vector3.Distance(playerTR.position,
                                                             monterTR.position);
            //���� �����Ÿ� ������ ���Դ��� Ȯ��
            if(distance <= attackDist)
            {
                state = State.ATTACK;
            }
            //���� �����Ÿ� ������ ���Դ��� Ȯ��
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
    //������ ���¿� ���� ������ ������ ���� 2 �ൿ���� ��Ʈ 
    IEnumerator MonsterAction()
    {
        while(!isDie)
        {
            switch(state)
            {
                //IDLE ����
                case State.IDLE:
                    //��������
                    //isStopped= NavMeshagent�� �Ӽ�����
                    agent.isStopped = true;

                    //Animator�� IsTrace ������ false�� ����
                    anim.SetBool(hashTrace, false); //3 �ִϸ��̼�
                    break;

                //���� ����
                case State.TRACE:
                    //���� ����� ��ǥ�� �̵�                   
                    agent.SetDestination(playerTR.position);
                    agent.isStopped = false;
                    //Animator�� IsTrace ������ true�� ����
                    anim.SetBool(hashTrace, true); //3 �ִϸ��̼�

                    //Animator�� IsAttack ������ false�� ����
                    anim.SetBool(hashAttack, false); //3 �ִϸ��̼� 
                    break;

                //���ݻ���
                case State.ATTACK:
                    //Animator�� IsAttack ������ true�� ����
                    anim.SetBool(hashAttack, true); //3 �ִϸ��̼�
                    break;

                //���
                case State.DIE:
                    isDie = true;
                    //��������
                    agent.isStopped = true; // NavMeshAgent ���� ������Ƽ true:�����
                    //����ִϸ��̼ǽ���
                    anim.SetTrigger(hashDie);
                    //������ Collider ������Ʈ ��Ȱ��ȭ
                    GetComponent<CapsuleCollider>().enabled = false;

                    //�����ð� ����� ������Ʈ Ǯ������ ȯ��
                    yield return new WaitForSeconds(3.0f);

                    //���͸� ��Ȱ��ȭ
                    this.gameObject.SetActive(false);
                    //����� �ٽû���� ���� ���� hp���� �ʱ�ȭ
                    hp = 100;
                    isDie = false;
                    //������ Collider�� Ȱ��ȭ
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
            //�浹�� ���ӿ�����Ʈ ���� ���װ�
            Destroy(coll.gameObject);
            //�ǰ� ���׼� �ִϸ��̼� ����
            anim.SetTrigger(hashHit);

            //�Ѿ��� �浹����
            Vector3 pos = coll.GetContact(0).point;
            //�Ѿ��� �浹 ������ ��������
            Quaternion rot = Quaternion.LookRotation(-coll.GetContact(0).normal);
            //����ȿ���� �����ϴ� �Լ� ȣ��
            ShowBloodEffect(pos, rot);

            //������ hp����
            hp -= 50;
            if(hp <=0)
            {
                state = State.DIE;
            }
        }
    }
    void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        //����ȿ�� ���� ������ ����� ���� ����
        GameObject blood = Instantiate<GameObject>(bloodEffect,
                                                                              pos,
                                                                              rot,
                                                                              monterTR);
        Destroy(blood, 1.0f);
    }
    void OnDrawGizmos()
    {
        //���� �����Ÿ� ǥ��
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position,
                                                 traceDist);
        }
        //���� �����Ÿ� ǥ��
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,
                                                 attackDist);
        }
    }
    void OnPlayerDie()
    {
        //������ ���¸� üũ�ϴ� �ڷ�ƾ�� ��� ����
        StopAllCoroutines();
        //������ �����ϰ� �ִϸ��̼� ����
        agent.isStopped = true;
        anim.SetFloat(hashSpeed, Random.Range(0.5f, 1.5f));
        anim.SetTrigger(hashPlayerDie);
    }
}
