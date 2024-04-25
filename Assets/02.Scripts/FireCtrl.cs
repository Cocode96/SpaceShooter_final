using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//목표1. 총알 공장을 만든다(Instantiate)
//목표2. 총알발사 사운드를 추가한다

 //반드시 필요한 컴포넌트를 명시해 해당 컴포넌트가 삭제되는 것을 방지
 [RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    //총알 프리팹
    public GameObject bullet;
    //총알 발사좌표
    public Transform firePos;

    //총소리에 사용될 음원
    public AudioClip fireSfx;
    //AudioSouce 컴포넌트 변수
    private new AudioSource audio;
    //Muzzle Flash의 MeshRenderer 컴포넌트
    private MeshRenderer muzzlFlash;


    void Start()
    {
        audio = GetComponent<AudioSource>();

        //FirePos 하위에 있는 Muzzleflash의 Material 컴포넌트 추출
        muzzlFlash = firePos.GetComponentInChildren<MeshRenderer>();
        //처음 시작할때 비활성화
        muzzlFlash.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //마우스 왼쪽 버튼을 클릭했을때 Fire함수 호출
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }
    void Fire()
    {
        //Bullet 프리팹을 동적으로 생성(생성할 객체, 위치, 회전)
        //총알공장을 만듬
        Instantiate(bullet, firePos.position, firePos.rotation);
        //총소리 발생
        audio.PlayOneShot(fireSfx, 0.5f);
        //총구 화염효과 코루틴  함수 호출
        StartCoroutine(ShowMuzzleFlash());
    }
    IEnumerator ShowMuzzleFlash()
    {
        //오프셋 좌표값을 랜덤 함수로 생성
        Vector2 offset = new Vector2(Random.Range(0, 2),
                                                    Random.Range(0, 2) * 0.5f);
        //텍스쳐 오프셋 값 설정
        muzzlFlash.material.mainTextureOffset = offset;
        //Muzzleflash의 회전 변경
        float angle = Random.Range(0, 360);
        muzzlFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);

        //Muzzleflash의 크기 조절
        float scale = Random.Range(1.0f, 2.0f);
        muzzlFlash.transform.localScale = Vector3.one * scale;
        //MuzzleFlash 활성화
        muzzlFlash.enabled = true;

        //0.2초 동안 대기(정지)하는 동안 메세지 루프로 제어권을 양보
        yield return new WaitForSeconds(0.2f);

        //MuzzleFlash 비활성화
        muzzlFlash.enabled = false;
    }
}
