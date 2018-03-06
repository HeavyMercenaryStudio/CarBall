using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalGate : MonoBehaviour {

    public delegate void OnGoalScored();
    public OnGoalScored notifyGoalScored;

    Collider thisCollider;
    void Start(){
        thisCollider = GetComponent<Collider>();
    }

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
