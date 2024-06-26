using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;
using Util.EventSystem;
using EventType = Util.EventSystem.EventType;

public class EnemyController : MonoBehaviour, IEventListener
{
    private void Start()
    {
        EventManager.Instance.AddListener(EventType.EnemyAction, this);
    }

    public void OnEvent(EventType eventType, Component sender, object param = null)
    {
        if (param == null)
            return;
        if (eventType != EventType.EnemyAction)
            return;
        var arg = ((Message)param).ArgToByte();
        transform.position = Vector3ByteConversion.BytesToVector3(arg[..12]);
        transform.rotation = Quaternion.Euler(Vector3ByteConversion.BytesToVector3(arg[12..24]));
    }
}
