using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��ǥ1. �Ѿ� ������ �����(Instantiate)
//��ǥ2. �Ѿ˹߻� ���带 �߰��Ѵ�

 //�ݵ�� �ʿ��� ������Ʈ�� ����� �ش� ������Ʈ�� �����Ǵ� ���� ����
 [RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    //�Ѿ� ������
    public GameObject bullet;
    //�Ѿ� �߻���ǥ
    public Transform firePos;

    //�ѼҸ��� ���� ����
    public AudioClip fireSfx;
    //AudioSouce ������Ʈ ����
    private new AudioSource audio;
    //Muzzle Flash�� MeshRenderer ������Ʈ
    private MeshRenderer muzzlFlash;


    void Start()
    {
        audio = GetComponent<AudioSource>();

        //FirePos ������ �ִ� Muzzleflash�� Material ������Ʈ ����
        muzzlFlash = firePos.GetComponentInChildren<MeshRenderer>();
        //ó�� �����Ҷ� ��Ȱ��ȭ
        muzzlFlash.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //���콺 ���� ��ư�� Ŭ�������� Fire�Լ� ȣ��
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }
    void Fire()
    {
        //Bullet �������� �������� ����(������ ��ü, ��ġ, ȸ��)
        //�Ѿ˰����� ����
        Instantiate(bullet, firePos.position, firePos.rotation);
        //�ѼҸ� �߻�
        audio.PlayOneShot(fireSfx, 0.5f);
        //�ѱ� ȭ��ȿ�� �ڷ�ƾ  �Լ� ȣ��
        StartCoroutine(ShowMuzzleFlash());
    }
    IEnumerator ShowMuzzleFlash()
    {
        //������ ��ǥ���� ���� �Լ��� ����
        Vector2 offset = new Vector2(Random.Range(0, 2),
                                                    Random.Range(0, 2) * 0.5f);
        //�ؽ��� ������ �� ����
        muzzlFlash.material.mainTextureOffset = offset;
        //Muzzleflash�� ȸ�� ����
        float angle = Random.Range(0, 360);
        muzzlFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);

        //Muzzleflash�� ũ�� ����
        float scale = Random.Range(1.0f, 2.0f);
        muzzlFlash.transform.localScale = Vector3.one * scale;
        //MuzzleFlash Ȱ��ȭ
        muzzlFlash.enabled = true;

        //0.2�� ���� ���(����)�ϴ� ���� �޼��� ������ ������� �纸
        yield return new WaitForSeconds(0.2f);

        //MuzzleFlash ��Ȱ��ȭ
        muzzlFlash.enabled = false;
    }
}
