using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneEntity : EntityBase
{
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.OnPlayerOneEnterEvent += SetCanSelect;
    }
}
