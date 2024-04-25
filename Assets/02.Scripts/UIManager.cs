using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Unity-UI를 사용하기위한 네임스페이스
using UnityEngine.Events;// Unity-Events 관련 API


public class UIManager : MonoBehaviour
{
    //버튼을 연결할 변수 사용할 버튼을 전부 선언
    public Button startButton;
    public Button optionButton;
    public Button shopButton;

    private UnityAction action; // 1번 방식

    void Start()
    {
        //UnityAction을 사용한 이벤트 연결 1번 방식
        action = () => OnButtonClick(startButton.name);
        startButton.onClick.AddListener(action);

        //무명 메서드를 이용한 이벤트 방식 2번 방식
        optionButton.onClick.AddListener(delegate { OnButtonClick(optionButton.name); });

        //람다식을 이용한 이벤트 방식 3번 방식 중요!! 꼭 기억@!
        shopButton.onClick.AddListener(() => OnButtonClick(shopButton.name));
    }

    public void OnButtonClick(string msg)
    {
        Debug.Log($"Click Button! : {msg}");
        //$ = string.format 문자열 보간
        //=> 뒤에 전달인자 없이 바로 대입
        //ex. print("Hello World %d", i);
    }
}
