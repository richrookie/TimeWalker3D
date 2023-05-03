using UnityEngine;
using DG.Tweening;

public class Clock : MonoBehaviour
{
    private SpriteRenderer[] _spriteRdr = null;
    private Transform _hourTf = null;
    private Transform _minTf = null;

    private void OnEnable()
    {
        if (_spriteRdr == null)
        {
            _spriteRdr = new SpriteRenderer[3];
            _spriteRdr[0] = this.GetComponent<SpriteRenderer>();
            _spriteRdr[1] = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
            _spriteRdr[2] = this.transform.GetChild(1).GetComponent<SpriteRenderer>();
        }
        if (_hourTf == null) _hourTf = Util.FindChild<Transform>(this.gameObject, "Hour", true);
        if (_minTf == null) _minTf = Util.FindChild<Transform>(this.gameObject, "Min", true);

        Timeback();
    }

    void Update()
    {
        _hourTf.transform.Rotate(Vector3.forward * 5);
        _minTf.transform.Rotate(Vector3.forward * 20);

        this.transform.LookAt(Camera.main.transform);
    }

    private void Timeback()
    {
        DOTween.Sequence()
                .Append(_spriteRdr[0].DOFade(0, .5f).SetDelay(1f))
                .Join(_spriteRdr[1].DOFade(0, .5f))
                .Join(_spriteRdr[2].DOFade(0, .5f))
                .OnComplete(() =>
                {
                    Destroy(this.gameObject);
                });
    }
}
