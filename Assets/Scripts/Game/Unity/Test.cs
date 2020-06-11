using UnityEngine;
using Zyq.Game.Client;

namespace Game {
    public class Test : MonoBehaviour {
        private int index = 0;

        private void Start() {
            if (Client.Ins.Connection != null) {
                InvokeRepeating("Login", 0, 5);
            }
        }

        private void Login() {
            index++;
            Sender.Login(Client.Ins.Connection, 1, true, 3, 4, 5, 6, 7, 8, 9, 10, "yinhuayong", new Vector2(2, 2), new Vector3(3, 3, 3), new Vector4(4, 4, 4, 4), Quaternion.Euler(9, 9, 9));
        }
    }
}