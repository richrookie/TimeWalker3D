using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Vector3 _offset = new Vector3(0, 70, -40);
    private float _speed = 10f;
    private Transform _target = null;

    public void Init()
    {
        if (_target == null)
            _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (Managers.Game.GameStatePlay)
        {
            this.transform.position = Vector3.Lerp(this.transform.position,
                                                                 _target.transform.position + _offset,
                                                                 _speed * Time.smoothDeltaTime);
        }
    }
}
