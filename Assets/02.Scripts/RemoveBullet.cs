using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    //����ũ ��ƼŬ �������� ������ ����
    public GameObject sparkEffect;

    //�浹�� �����ҋ� �߻��ϴ� �̺�Ʈ
    void OnCollisionEnter(Collision coll)
    {
        //�浹�� ���� ������Ʈ�� �±װ��� ��
        if(coll.collider.CompareTag("BULLET"))
        {
            //ù��° �浹 ������ ������ ����
            ContactPoint cp = coll.GetContact(0);
            //�浹�� �Ѿ��� ���� ���͸� ���ʹϾ� Ÿ������ ��ȯ
            //�Ѿ��� ���� ������ �ݴ�������� �������Ѵ�.
            Quaternion rot = Quaternion.LookRotation(-cp.normal);

            //����ũ ��ƼŬ�� �������� ����-> ������ �����
            //Instantiate(������ ��ü, ��ġ, ȸ��)
            GameObject spark = Instantiate(sparkEffect, cp.point, rot);
            //�����ð��� ������ ����ũ ��ƼŬ ����
            Destroy(spark, 0.5f);

            //�浹�� ���� ������Ʈ�� ������-->�Ѿ� ����
            //���װ�
            Destroy(coll.gameObject);

            //���װ�
            //Destroy(gameObject);

        }
    }
}
