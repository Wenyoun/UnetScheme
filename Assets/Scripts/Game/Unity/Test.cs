using UnityEngine;
using Zyq.Game.Client;

namespace Game {
    public class Test : MonoBehaviour {
        private int index = 0;

        private void Start() {
            InvokeRepeating("Login", 0, 5);
        }

        private void Login() {
            index++;
            SendServer.LoginReq(Client.Ins.Connection, "yinhuayong" + index, "huayong" + index);
        }
    }
}