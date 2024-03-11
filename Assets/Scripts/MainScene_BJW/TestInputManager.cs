using AT_RPG;
using AT_RPG.Manager;
using UnityEngine;

public class TestInputManager : MonoBehaviour
{
    private void Awake()
    {
        InputManager.AddKeyAction("Move Left", OnTestAKeyDown);
        InputManager.AddKeyAction("Crouch", OnTestControlKeyDown);
        InputManager.AddKeyAction("Aim", OnTestMouseKeyDown);
    }

    private void OnDestroy()
    {
        InputManager.RemoveKeyAction("Move Left", OnTestAKeyDown);
        InputManager.RemoveKeyAction("Crouch", OnTestControlKeyDown);
        InputManager.RemoveKeyAction("Aim", OnTestMouseKeyDown);
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
