using Game.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Core { 
public class GoalGate : NetworkBehaviour {

    public delegate void OnGoalScored();
    public OnGoalScored notifyGoalScored;

    Collider thisCollider;
    void Start(){
        thisCollider = GetComponent<Collider>();
    }
    [Server]
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == Layers.BALL)
        {
            CheckForIntersection(other);
        }
    }

    private void CheckForIntersection(Collider other)
    {
        if(thisCollider.bounds.Contains(other.bounds.max) && 
           thisCollider.bounds.Contains(other.bounds.min))
        {
            notifyGoalScored();
        }
    }
}
}
