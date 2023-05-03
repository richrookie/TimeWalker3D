using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Define.eGameState _cureGameState = Define.eGameState.Ready;
    public bool GameStateReady => _cureGameState == Define.eGameState.Ready;
    public bool GameStatePlay => _cureGameState == Define.eGameState.Play;
    public bool GameStateEnd => _cureGameState == Define.eGameState.End;


    private Player _player = null;
    private CameraManager _camManager = null;
    private JoyStickController _joystickController = null;

    public Player player { get { CheckNull(); return _player; } }
    public CameraManager camManager { get { CheckNull(); return _camManager; } }
    public JoyStickController joystickController { get { CheckNull(); return _joystickController; } }


    [HideInInspector]
    public JoyStickController JoyStickController;
    public void SetDownAction(System.Action action)
    {
        JoyStickController?.AddDownEvent(action);
    }

    public void SetUpAction(System.Action action)
    {
        JoyStickController?.AddUpEvent(action);
    }

    public void SetMoveAction(System.Action<Vector2> action)
    {
        JoyStickController?.AddMoveEvent(action);
    }

    public void Init()
    {
        CheckNull();

        player.Init();
        camManager.Init();
        joystickController.Init();

        _cureGameState = Define.eGameState.Play;
    }

    private void CheckNull()
    {
        if (_player == null)
            _player = FindObjectOfType<Player>() as Player;


        if (_camManager == null)
            _camManager = FindObjectOfType<CameraManager>() as CameraManager;

        if (_joystickController == null)
            _joystickController = FindObjectOfType<JoyStickController>() as JoyStickController;


        if (_joystickController == null)
        {
            GameObject joystick = Managers.Resource.Instantiate("Canvas_Joystick");
            _joystickController = joystick.GetComponent<JoyStickController>();

            Managers.Resource.Instantiate("EventSystem", joystick.transform);
        }

    }

    public void Clear()
    {
        if (JoyStickController != null)
        {
            JoyStickController.DownAction = null;
            JoyStickController.UpAction = null;
            JoyStickController.JoystickMoveAction = null;
        }
    }
}
