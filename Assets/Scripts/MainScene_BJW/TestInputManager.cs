using AT_RPG;
using AT_RPG.Manager;
using UnityEngine;

public class TestInputManager : MonoBehaviour
{
    private void Awake()
    {
        InputManager.Instance.AddKeyAction(InputManager.MoveLeft, OnTestAKeyDown);
        InputManager.Instance.AddKeyAction(InputManager.Crouch, OnTestControlKeyDown);
        InputManager.Instance.AddKeyAction(InputManager.Aim, OnTestMouseKeyDown);
    }

    private void OnDestroy()
    {
        InputManager.Instance.RemoveKeyAction(InputManager.MoveLeft, OnTestAKeyDown);
        InputManager.Instance.RemoveKeyAction(InputManager.Crouch, OnTestControlKeyDown);
        InputManager.Instance.RemoveKeyAction(InputManager.Aim, OnTestMouseKeyDown);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTestAKeyDown(InputValue value)
    {
        Debug.Log($"InputManager A키 입력 테스트 : {value.GetValue<bool>()}");
    }

    private void OnTestControlKeyDown(InputValue value)
    {
        Debug.Log($"InputManager 컨트롤키 입력 테스트 : {value.GetValue<bool>()}");
    }

    private void OnTestMouseKeyDown(InputValue value)
    {
        Vector2 inputAxis = value.GetValue<Vector2>();
        Debug.Log($"InputManager 마우스 입력 테스트 : {inputAxis.x}, {inputAxis.y}");
    }
}
