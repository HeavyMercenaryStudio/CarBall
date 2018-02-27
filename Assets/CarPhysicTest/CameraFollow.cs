using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraUI { 

    //CameraArm
    //  Camera

    /// <summary>
    /// Follow target by arm of camera
    /// </summary>
    public class CameraFollow : MonoBehaviour {

        [SerializeField] float smoothTime = 0.3F; // smoothness when interpolating
        [SerializeField] Transform[] players; // list of players

        Vector3 moveVelocity; //Veclocity of camera
        Vector3 avaragePosition; // avarage position of targets

        void FixedUpdate(){
            Move ();
	    }

        public void Move()
        {

            //Calculate acarage position of players
            avaragePosition = new Vector3 ();
            for (int i = 0; i < players.Length; i++)
                avaragePosition += players[i].position;

            avaragePosition /= players.Length;
            avaragePosition.y = 0f;

            //follow this position
            transform.position = Vector3.SmoothDamp (transform.position, avaragePosition, ref moveVelocity, smoothTime);
        }

        public Transform[] GetPlayers()
        {
            return players;
        }
        public void SetTransformTargets(List<Transform> targets)
        {
            players = targets.ToArray ();
        }
    }
}