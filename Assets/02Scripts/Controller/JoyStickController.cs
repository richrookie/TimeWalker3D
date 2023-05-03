using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyStickController : MonoBehaviour
{
    [Header("조이스틱 방식")]
    [Tooltip("Fixed : 누른 위치에 고정\nHardFixed : 이미 고정 되어있음\nFollow : 원 밖으로 나가면 원이 따라옴\nSlowFollow : 원 밖으로 나가면 원이 천천히 따라옴\nRunningGame : 런 방식처럼 좌우 오프셋만 인식")]
    public Define.eJoyStickMethod eJoyStickMethod;
    [Header("조이스틱이 돌아다닐 수 있는 반경")]
    public float JoyStickBound;
    [Header("움직일 물체 (Rigidbody필요)")]
    public Rigidbody MoveObjectRig;

    public float Threshold = 10;


    public float Speed = 5;
    private float _curSpeed;
    public float XBound;
    public bool AutoRun;
    public float X_Sensitivity;
    public float X_Acceletor;

    public bool UseAccelerate;
    public float Accelerate = 100;


    private RectTransform _canvasRect;
    private RectTransform _joystick;
    private RectTransform _joystickHandle;
    [SerializeField]
    private Image _joystickImage;
    private Image _joystickHandleImage;

    private Vector3 _oriPos;
    private Vector3 newDir = Vector3.zero;
    private float _oriXPos;

    [HideInInspector]
    public bool CanMove = false;
    private bool isButtonClick = false;
    private bool isMouseDown = false;

    public System.Action DownAction;
    public System.Action<Vector2> JoystickMoveAction;
    public System.Action Moveaction;
    public System.Action UpAction;

    public void Init()
    {
        _curSpeed = 0;
        _canvasRect = this.GetComponent<RectTransform>();
        _joystickImage = this.transform.GetChild(0).GetComponent<Image>();
        _joystick = _joystickImage.GetComponent<RectTransform>();
        _joystickHandle = _joystick.GetChild(0).GetComponent<RectTransform>();
        _joystickHandleImage = _joystickHandle.GetComponent<Image>();
        _joystickImage.enabled = false;
        _joystickHandleImage.enabled = false;

        AddDownEvent(() =>
        {
            isMouseDown = true;
            Managers.Game.player.SeteAnimState(Define.eAnimState.Run);
        });

        AddUpEvent(() =>
        {
            isMouseDown = false;
            Managers.Game.player.CheckTimeback();
        });

        switch (eJoyStickMethod)
        {
            case Define.eJoyStickMethod.DoNotUse:
                _joystick.gameObject.SetActive(false);

                break;
            case Define.eJoyStickMethod.Fixed:
                break;

            case Define.eJoyStickMethod.Follow:
                break;
        }

        CanMove = true;
        isButtonClick = false;

        Managers.Game.JoyStickController = this;

        MoveObjectRig = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    //JoyStickEditor 에서 활용함
    public void SetRigidBody()
    {
        MoveObjectRig.isKinematic = false;
        MoveObjectRig.mass = 100;
        MoveObjectRig.drag = 100;
        MoveObjectRig.angularDrag = 100;
        MoveObjectRig.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;

        if (UseAccelerate)
        {
            if (!isMouseDown)
            {
                _curSpeed = Mathf.Max(0, _curSpeed - Accelerate * Time.deltaTime);
            }

            MoveObjectRig.position += newDir * Time.deltaTime * _curSpeed;
        }


        switch (eJoyStickMethod)
        {
            case Define.eJoyStickMethod.DoNotUse:
                return;

            case Define.eJoyStickMethod.Fixed:
                if (Input.GetMouseButtonDown(0))
                {
                    if (CheckButtonClick()) return;

                    _joystickImage.enabled = true;
                    _joystickHandleImage.enabled = true;

                    _joystick.anchoredPosition = Input.mousePosition * 2688f / Screen.height;
                    _joystickHandle.anchoredPosition = Vector2.zero;
                    _oriPos = _joystick.anchoredPosition;

                    DownAction?.Invoke();
                }
                else if (Input.GetMouseButton(0) && !isButtonClick)
                {
                    _joystickHandle.anchoredPosition = Input.mousePosition * 2688f / Screen.height - _oriPos;
                    if (_joystickHandle.anchoredPosition.magnitude > JoyStickBound)
                    {
                        _joystickHandle.anchoredPosition = _joystickHandle.anchoredPosition.normalized * JoyStickBound;
                    }

                    if (_joystickHandle.anchoredPosition.magnitude < Threshold) return;

                    JoystickMoveAction?.Invoke(_joystickHandle.anchoredPosition);

                    Vector3 dir = new Vector3(_joystickHandle.anchoredPosition.x, 0, _joystickHandle.anchoredPosition.y);

                    if (MoveObjectRig != null)
                    {
                        Move(dir);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && !isButtonClick)
                {
                    _joystickImage.enabled = false;
                    _joystickHandleImage.enabled = false;
                    UpAction?.Invoke();
                }
                break;

            case Define.eJoyStickMethod.Follow:
                if (Input.GetMouseButtonDown(0))
                {
                    if (CheckButtonClick()) return;

                    _joystickImage.enabled = true;
                    _joystickHandleImage.enabled = true;

                    _joystick.anchoredPosition = Input.mousePosition * 2688f / Screen.height;
                    _joystickHandle.anchoredPosition = Vector2.zero;
                    _oriPos = _joystick.anchoredPosition;

                    DownAction?.Invoke();
                }
                else if (Input.GetMouseButton(0) && !isButtonClick)
                {
                    _joystickHandle.anchoredPosition = Input.mousePosition * 2688f / Screen.height - _oriPos;
                    if (_joystickHandle.anchoredPosition.magnitude > JoyStickBound)
                    {
                        _joystick.anchoredPosition = (Vector2)_oriPos + _joystickHandle.anchoredPosition - JoyStickBound * _joystickHandle.anchoredPosition.normalized;
                        _joystickHandle.anchoredPosition = _joystickHandle.anchoredPosition.normalized * JoyStickBound;
                        _oriPos = _joystick.anchoredPosition;
                    }
                    if (_joystickHandle.anchoredPosition.magnitude < Threshold) return;

                    JoystickMoveAction?.Invoke(_joystickHandle.anchoredPosition);

                    Vector3 dir = new Vector3(_joystickHandle.anchoredPosition.x, 0, _joystickHandle.anchoredPosition.y);
                    if (MoveObjectRig != null)
                    {
                        Move(dir);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && !isButtonClick)
                {
                    _joystickImage.enabled = false;
                    _joystickHandleImage.enabled = false;
                    UpAction?.Invoke();
                }
                break;
        }
    }


    public void AddDownEvent(System.Action action)
    {
        DownAction -= action;
        DownAction += action;
    }
    public void AddMoveEvent(System.Action<Vector2> action)
    {
        JoystickMoveAction -= action;
        JoystickMoveAction += action;
    }
    public void AddPlayerMoveEvent(System.Action action)
    {
        Moveaction -= action;
        Moveaction += action;
    }
    public void AddUpEvent(System.Action action)
    {
        UpAction -= action;
        UpAction += action;
    }

    private bool CheckButtonClick()
    {
        if (EventSystem.current?.currentSelectedGameObject?.GetComponent<Button>())
        {
            isButtonClick = true;
            return true;
        }
        else
        {
            isButtonClick = false;
            return false;
        }
    }

    private void Move(Vector3 dir)
    {
        newDir = dir.normalized;

        MoveObjectRig.rotation = Quaternion.LookRotation(newDir);

        if (UseAccelerate)
        {
            _curSpeed = Mathf.Min(Speed, _curSpeed + Accelerate * Time.deltaTime);
        }
        else
        {
            MoveObjectRig.position += newDir * Speed * Time.deltaTime;
        }
    }
}
