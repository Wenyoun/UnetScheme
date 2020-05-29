using Base;
using System;
using UnityEngine;

public class Battle : MonoBehaviour
{
    private void Awake()
    {
        PageMgr.Ins.Push<BattlePage>();
        Destroy(gameObject);
    }
}