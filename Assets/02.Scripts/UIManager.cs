using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Unity-UI�� ����ϱ����� ���ӽ����̽�
using UnityEngine.Events;// Unity-Events ���� API


public class UIManager : MonoBehaviour
{
    //��ư�� ������ ���� ����� ��ư�� ���� ����
    public Button startButton;
    public Button optionButton;
    public Button shopButton;

    private UnityAction action; // 1�� ���

    void Start()
    {
        //UnityAction�� ����� �̺�Ʈ ���� 1�� ���
        action = () => OnButtonClick(startButton.name);
        startButton.onClick.AddListener(action);

        //���� �޼��带 �̿��� �̺�Ʈ ��� 2�� ���
        optionButton.onClick.AddListener(delegate { OnButtonClick(optionButton.name); });

        //���ٽ��� �̿��� �̺�Ʈ ��� 3�� ��� �߿�!! �� ���@!
        shopButton.onClick.AddListener(() => OnButtonClick(shopButton.name));
    }

    public void OnButtonClick(string msg)
    {
        Debug.Log($"Click Button! : {msg}");
        //$ = string.format ���ڿ� ����
        //=> �ڿ� �������� ���� �ٷ� ����
        //ex. print("Hello World %d", i);
    }
}
