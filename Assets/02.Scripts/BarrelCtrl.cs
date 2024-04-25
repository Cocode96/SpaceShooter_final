using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    //폭발 효과 파티클을 연결할 변수
    public GameObject expEffect;
    //무작위로 적용할 텍스쳐
    public Texture[] textures;
    //폭발반경
    public float radius = 10.0f;

    //하위에 있는 Mesh Renderer 컴포넌트를 저장할 변수
    //새로운 MeshRenderer 멤버변수를 생성
    private new MeshRenderer renderer;

    //컴포넌트를 저장할  변수
    private Transform tr;
    private Rigidbody rb;
    //총알 맞은 횟수를 누적시킬 변수
    private int hitCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        //하위에 있는 MeshRenderer 컴포넌트를 추출
        renderer = GetComponentInChildren<MeshRenderer>();

        //난수 발생
        int idx = Random.Range(0, textures.Length);
        //텍스쳐 지정
        renderer.material.mainTexture = textures[idx];
    }

    //충돌 시 발생하는 콜백함수
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            //총알 맞은 횟수를 증가시키고 3회 이상이면 폭발 처리
            if(++hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        //폭발효과 파티클 생성, 공장
        GameObject exp = Instantiate(expEffect,
                                                     tr.position,
                                                     Quaternion.identity);
        //폭발효과 파티클 5초후에 제거
        Destroy(exp, 5.0f);

        //Rigidbody 컴포넌트의 mass을 1.0로 수정-> 무게를 가볍게 해서
        //위로 솟구치도록 하기 위해
        //rb.mass = 1.0f;
        //위로 솟구치게함
        //rb.AddForce(Vector3.up * 1000.0f);

        //간접 폭발력을 전달
        IndirectDamage(tr.position);

        //3초 후에 드럼통 제거
        Destroy(gameObject, 3.0f);
    }
    void IndirectDamage(Vector3 pos)
    {
        //주변에 있는 드럼통을 모두 추출
        Collider[] colls = Physics.OverlapSphere(pos, radius, 1 << 3);
        //foreach(데이터 형식 변수명 in 배열)
        foreach (var coll in colls)
        {
            //폭발 범위에 포함된 드럼통의 Rigidbody 컴포넌트 추출
            rb = coll.GetComponent<Rigidbody>();
            //드럼통 무게를 가볍게
            rb.mass = 1.0f;
            //freezeRotation 제한값을 해제
            rb.constraints = RigidbodyConstraints.None;
            //폭발력전달
            rb.AddExplosionForce(1000.0f, pos, radius, 1200.0f);
        }
    }

}
