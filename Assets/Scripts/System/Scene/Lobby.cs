using Base;
using System;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    private void Awake()
    {
        PageMgr.Ins.Push<LobbyPage>();
        Destroy(gameObject);
    }
}