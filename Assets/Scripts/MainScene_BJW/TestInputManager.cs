using AT_RPG;
using AT_RPG.Manager;
using System.Collections;
using UnityEngine;

public class TestInputManager : MonoBehaviour
{
    private void Awake()
    {
        InputManager.Instance.AddKeyAction(InputManager.MoveLeft, OnTestAKeyDown);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OnEraseTestAKeyDown());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator OnEraseTestAKeyDown()
    {
        float time = 4;
        Debug.Log($"{time}초 뒤에 A키가 블락됩니다.");

        yield return new WaitForSeconds(time);

        InputManager.Instance.EraseKeyAction(InputManager.MoveLeft, OnTestAKeyDown);
    }

    private void OnTestAKeyDown(InputValue value)
    {
        Debug.Log("InputManager A키 입력 테스트");
    }
}
