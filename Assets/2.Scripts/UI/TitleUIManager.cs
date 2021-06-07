using System;
using UnityEngine;


[System.Serializable]
public class TestDictionary : SerializeDictionary<string, int> { }

public class TitleUIManager : MonoBehaviour
{
    private bool isLoading = false;

    [SerializeField]
    private TestDictionary testdic = new TestDictionary();

    private void Start()
    {
        testdic.Add("하이", 1);
        testdic.Add("안녕", 2);
    }


    private void Update()
    {
        if (isLoading == true)
            return;

        if (Input.GetMouseButtonDown(0))
        {

            Animator animator = GetComponentInChildren<Animator>();
            if (animator != null) animator.SetTrigger("Start");

            print("게임 시작");
            Invoke(nameof(GameStart), 1.0f);
            isLoading = true;
        }
    }

    private void GameStart()
    {
        GameManager.instance.LoadScene("MenuScene");
    }
}
