using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    //스파크 파티클 프리팹을 연결할 변수
    public GameObject sparkEffect;

    //충돌이 시작할떄 발생하는 이벤트
    void OnCollisionEnter(Collision coll)
    {
        //충돌한 게임 오브젝트의 태그값을 비교
        if(coll.collider.CompareTag("BULLET"))
        {
            //첫번째 충돌 지점의 정보를 추출
            ContactPoint cp = coll.GetContact(0);
            //충돌한 총알의 법선 벡터를 쿼터니언 타입으로 변환
            //총알은 법선 벡터의 반대방향으로 나오게한다.
            Quaternion rot = Quaternion.LookRotation(-cp.normal);

            //스파크 파티클을 동적으로 생성-> 공장을 만든다
            //Instantiate(생성할 객체, 위치, 회전)
            GameObject spark = Instantiate(sparkEffect, cp.point, rot);
            //일정시간이 지난후 스파크 파티클 삭제
            Destroy(spark, 0.5f);

            //충돌한 게임 오브젝트를 삭제해-->총알 삭제
            //너죽고
            Destroy(coll.gameObject);

            //나죽고
            //Destroy(gameObject);

        }
    }
}
