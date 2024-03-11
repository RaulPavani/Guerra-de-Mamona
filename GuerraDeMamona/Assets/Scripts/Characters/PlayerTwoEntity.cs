using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTwoEntity : EntityBase
{
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.OnPlayerTwoEnterEvent += SetCanSelect;
    }
}
