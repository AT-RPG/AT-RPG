using Fusion;
using UnityEngine;

public class TestNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TestBall _prefabBall;
    [SerializeField] private TestPhysxBall _prefabPhysxBall;

    [Networked] private TickTimer delay { get; set; }

    private NetworkCharacterControllerPrototype _cc;
    private CharacterController cc;
    private Vector3 _forward;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
        cc = GetComponent<CharacterController>();
        _forward = transform.forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
            cc.Move(5 * data.direction * Runner.DeltaTime);



            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;

            if (delay.ExpiredOrNotRunning(Runner))
            {
                if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabBall,
                    transform.position + _forward, Quaternion.LookRotation(_forward),
                    Object.InputAuthority, (runner, o) =>
                    {
                        // Initialize the Ball before synchronizing it
                        o.GetComponent<TestBall>().Init();
                    });
                }
            }
            else if ((data.buttons & NetworkInputData.MOUSEBUTTON2) != 0)
            {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(_prefabPhysxBall,
                  transform.position + _forward,
                  Quaternion.LookRotation(_forward),
                  Object.InputAuthority, (runner, o) =>
                  {
                      o.GetComponent<TestPhysxBall>().Init(10 * _forward);
                  });
            }
        }
    }

}