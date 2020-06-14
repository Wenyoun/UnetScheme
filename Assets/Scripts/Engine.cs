using UnityEngine;

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
        GameMgr.Ins.OnLateUpdate();
    }

    private void FixedUpdate()
    {
        GameMgr.Ins.OnFixedUpdate(Time.deltaTime);
    }

    private void OnDestroy()
    {
        GameMgr.Ins.OnRemove();
    }
}