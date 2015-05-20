using UnityEngine;
using System.Collections;


namespace Assets._Scripts.TestsNFun
{
    public class DelegateTests : MonoBehaviour
    {
        public delegate void GameEventsDelegate(GameObject player, GameObject computer);

        public static event GameEventsDelegate PlayerInRangeEvent;
        public static event GameEventsDelegate PlayerOutOfRangeEvent;


        void Awake()
        {
            PlayerInRangeEvent += Enter;
            PlayerOutOfRangeEvent += Exit;
        }

        void Start()
        {

        }


        private static void Enter(GameObject player, GameObject computer)
        {
            //        Debug.Log("Connection between " + player.name + " and " + computer.name + " established");
        }

        private static void Exit(GameObject player, GameObject computer)
        {
            //        Debug.Log("Connection between " + player.name + " and " + computer.name + " disabled");
        }

        public static void OnPlayerInRangeEvent(GameObject player, GameObject computer)
        {
            var handler = PlayerInRangeEvent;
            if (handler != null) handler(player, computer);
        }

        public static void OnPlayerOutOfRangeEvent(GameObject player, GameObject computer)
        {
            var handler = PlayerOutOfRangeEvent;
            if (handler != null) handler(player, computer);
        }
    }
}
