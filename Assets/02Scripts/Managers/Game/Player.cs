using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> _movePosList = new List<Vector3>();
    public List<Vector3> MovePosList
    {
        get { return _movePosList; }
    }
    private bool _canTimeback = false;
    public bool CanTimeback
    {
        get { return _canTimeback; }
    }

    private Animator _animCtrl = null;
    private SkinnedMeshRenderer _skinMeshRdr = null;
    private TrailRenderer[] _trailRdr = null;
    private Rigidbody _rigid = null;

    public void Init()
    {
        _animCtrl = this.GetComponent<Animator>();
        _skinMeshRdr = Util.FindChild<SkinnedMeshRenderer>(this.gameObject, "Sphere", true);
        _trailRdr = this.transform.GetComponentsInChildren<TrailRenderer>();
        _rigid = this.GetComponent<Rigidbody>();
        foreach (var trail in _trailRdr)
        {
            trail.time = 0f;
        }
    }


    private Coroutine _corCheckPlayerPos = null;
    private IEnumerator CorCheckPlayerPos()
    {
        yield return Util.WaitGet(.75f);

        _corAlterEgo = StartCoroutine(CorAlterEgo());

        while (true)
        {
            _movePosList.Add(this.transform.position);

            yield return Util.WaitGet(.1f);
        }
    }

    private Coroutine _corAlterEgo = null;
    private GameObject _alterEgo = null;
    private IEnumerator CorAlterEgo()
    {
        yield return Util.WaitGet(1f);

        _canTimeback = true;
        SetMaterial(true);

        foreach (var trail in _trailRdr)
        {
            trail.time = 1f;
        }

        _alterEgo = Managers.Resource.Instantiate("AlterEgo");
        _alterEgo.transform.position = _movePosList[0];
    }
    public bool RemovePos()
    {
        if (_movePosList.Count > 0)
        {
            _movePosList.Remove(_movePosList[0]);
            return true;
        }

        return false;
    }

    private Coroutine _corTimeback = null;
    private IEnumerator CorTimeback()
    {
        _canTimeback = false;

        Managers.Sound.Play(Define.eSound.TimeBack.ToString(), Define.eSound.TimeBack);
        Managers.Sound.Play(Define.eSound.TickTock.ToString(), Define.eSound.TickTock);

        _movePosList.Reverse();

        yield return Util.WaitGet(.05f);

        Managers.Resource.Instantiate("Time", this.transform);

        for (int n = 0; n < _movePosList.Count; n += 1)
        {
            this.transform.DOMove(_movePosList[n], .01f);

            yield return Util.WaitGet(.011f);
        }

        SetMaterial(false);

        foreach (var trail in _trailRdr)
        {
            trail.time = 0f;
        }

        DestroyAlterEgo();
        _movePosList.Clear();
        SeteAnimState(Define.eAnimState.Idle);
    }


    public void CheckTimeback()
    {
        if (_canTimeback)
            SeteAnimState(Define.eAnimState.Timeback);
        else
            SeteAnimState(Define.eAnimState.Idle);
    }

    public void SeteAnimState(Define.eAnimState eAnimState)
    {
        switch (eAnimState)
        {
            case Define.eAnimState.Idle:
                SetIdle();
                break;

            case Define.eAnimState.Run:
                SetRun();
                break;

            case Define.eAnimState.Timeback:
                SetTimeback();
                break;

            case Define.eAnimState.Death:
                _animCtrl.SetTrigger("Death");
                break;
        }
    }

    private void SetIdle()
    {
        _animCtrl.ResetTrigger("Run");
        _animCtrl.SetTrigger("Idle");

        if (_corCheckPlayerPos != null)
        {
            StopCoroutine(_corCheckPlayerPos);
            _corCheckPlayerPos = null;
        }

        if (_corAlterEgo != null)
        {
            DestroyAlterEgo();
            StopCoroutine(_corAlterEgo);
            _corAlterEgo = null;
        }

        if (_corTimeback != null)
        {
            StopCoroutine(_corTimeback);
            _corTimeback = null;
        }

        _movePosList.Clear();
    }

    private void SetRun()
    {
        _animCtrl.ResetTrigger("Idle");
        _animCtrl.SetTrigger("Run");

        if (_corCheckPlayerPos == null)
            _corCheckPlayerPos = StartCoroutine(CorCheckPlayerPos());
    }

    private void SetTimeback()
    {
        _animCtrl.ResetTrigger("Run");
        _animCtrl.SetTrigger("Idle");

        if (_corCheckPlayerPos != null)
        {
            StopCoroutine(_corCheckPlayerPos);
            _corCheckPlayerPos = null;
        }

        if (_corAlterEgo != null)
        {
            DestroyAlterEgo();
            StopCoroutine(_corAlterEgo);
            _corAlterEgo = null;
        }

        if (_corTimeback == null)
            _corTimeback = StartCoroutine(CorTimeback());
    }

    public void SetMaterial(bool timeback)
    {
        if (timeback)
            _skinMeshRdr.material = Managers.Resource.Load<Material>("Timeback");
        else
            _skinMeshRdr.material = Managers.Resource.Load<Material>("Origin");
    }

    public void DestroyAlterEgo()
    {
        if (_alterEgo != null)
        {
            Destroy(_alterEgo);
            _alterEgo = null;
        }
    }
}
