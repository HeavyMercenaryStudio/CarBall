using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utility;
using UnityEngine.Networking;
using Game.Car;

namespace Game.Core { 
    public class MapEdge : NetworkBehaviour {

        [Server]
        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == Layers.PLAYER){
                RpcUpdatePlayerPosition(other.GetComponent<CarController>().netId);
            }
        }

        [ClientRpc]
        public void RpcUpdatePlayerPosition(NetworkInstanceId netId)
        {
            var list = FindObjectsOfType<CarController>();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].netId == netId)
                {
                    var positions = FindObjectsOfType<NetworkStartPosition>();
                    var pos = positions[Random.Range(0, positions.Length - 1)];
                    list[i].transform.position = pos.transform.position;
                    break;
                }
            }
        }
    }
}
