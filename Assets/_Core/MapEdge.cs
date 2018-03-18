using Game.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core { 
    public class MapEdge : MonoBehaviour {

	    void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == Layers.PLAYER)
            {
                //Reset position
            }

        }
    }
}
