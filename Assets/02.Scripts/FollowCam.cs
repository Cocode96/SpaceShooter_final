using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    //���󰡾� �� ����� ������ ����
    public Transform targetTr;
    //Main Camera �ڽ��� Transform ������Ʈ
    private Transform camTr;
    //���� ������� ���� ������ ���� �Ÿ�
    [Range(2.0f, 20.0f)]
    public float distance = 10.0f;

    //Y������ �̵��� ����
    [Range(0.0f, 10.0f)]
    public float height = 2.0f;

    //�����ӵ�
    public float damping = 10.0f;

    //SmoothDamp���� ����� ����
    private Vector3 velocity = Vector3.zero;

    //ī�޶� LookAt�� offset��
    public float targetOffset = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Main Camera �ڽ��� Transform ������Ʈ ����
        camTr = GetComponent<Transform>();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        //�����ؾ��� ����� �������� distance��ŭ �̵�
        //���̸� height ��ŭ �̵�
        Vector3 pos = targetTr.position + (-targetTr.forward * distance) + (Vector3.up * height);

        //���� ���� �����Լ��� ����� �ε巴�� ��ġ�� ����
        //Slerp(������ġ, ��ǥ��ġ, �ð� t)
        //camTr.position = Vector3.Slerp(camTr.position,
        //                                                pos,
        //                                                Time.deltaTime * damping);
       
        //SmoothDamp(������ġ, ��ǥ��ġ, ����ӵ�, ��ǥ��ġ���� ���� �ð�)
        camTr.position = Vector3.SmoothDamp(camTr.position,
                                                                    pos,
                                                                    ref velocity,
                                                                    damping);

        //Camera�� �ǹ� ��ǥ�� ���� ȸ��
        camTr.LookAt(targetTr.position + (targetTr.up*targetOffset));
    }
}
