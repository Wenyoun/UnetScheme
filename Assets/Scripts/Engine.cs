﻿using UnityEngine;

public class Engine : MonoBehaviour
{
    private void Awake()
    {
        GameMgr.Ins.Config();
    }

    private void Start()
    {
        GameMgr.Ins.OnInit();
    }

    private void Update()
    {
        GameMgr.Ins.OnUpdate(Time.deltaTime);
    }

    private void LateUpdate()
    {
        GameMgr.Ins.OnLateUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        GameMgr.Ins.OnFixedUpdate(Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
        GameMgr.Ins.OnRemove();
    }
}