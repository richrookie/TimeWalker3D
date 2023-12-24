using System.Collections;
using UnityEngine;

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
        StartCoroutine(CorFadeClock());
    }

    private IEnumerator CorFadeClock()
    {
        yield return Util.WaitGet(.75f);

        Color color0 = _spriteRdr[0].color;
        Color color1 = _spriteRdr[1].color;
        Color color2 = _spriteRdr[2].color;

        while (true)
        {
            if (color0.a <= 0)
                break;

            color0.a -= .025f;
            color1.a -= .025f;
            color2.a -= .025f;

            _spriteRdr[0].color = color0;
            _spriteRdr[1].color = color1;
            _spriteRdr[2].color = color2;

            yield return Util.WaitGet(.01f);
        }
    }
}
