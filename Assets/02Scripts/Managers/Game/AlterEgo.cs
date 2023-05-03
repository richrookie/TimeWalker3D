using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterEgo : MonoBehaviour
{
    private Vector3 _targetVec = Vector3.zero;
    private Rigidbody _rigid = null;

    private void OnEnable()
    {
        if (_rigid == null) _rigid = this.GetComponent<Rigidbody>();

        this.transform.position = Managers.Game.player.MovePosList[0];
        if (Managers.Game.player.RemovePos())
            _targetVec = Managers.Game.player.MovePosList[0];
        else
            Managers.Game.player.DestroyAlterEgo();
    }

    private void FixedUpdate()
    {
        if (Managers.Game.GameStatePlay)
        {
            if (Managers.Game.player.CanTimeback)
            {
                float distance = (this.transform.position - _targetVec).sqrMagnitude;
                if (distance <= 5f)
                {
                    if (Managers.Game.player.RemovePos())
                        _targetVec = Managers.Game.player.MovePosList[0];
                }
                else
                {
                    Vector3 dir = _targetVec - this.transform.position;
                    this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10);
                }

                _rigid.position += this.transform.forward * 35 * Time.deltaTime;
            }
        }
    }
}
