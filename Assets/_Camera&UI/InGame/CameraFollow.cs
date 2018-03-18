using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.UI { 

    //CameraArm
    //  Camera

    /// <summary>
    /// Follow target by arm of camera
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] float smoothTime = 0.2f; // smoothness when interpolating
        [SerializeField] Transform player; // list of players

        Vector3 moveVelocity; //Veclocity of camera
        public Transform Player
        {
            get {
                return player;
            }

            set {
                player = value;
            }
        }

        void FixedUpdate(){
            Move ();
	    }

        public void Move()
        {
            //follow this position
                transform.position = Vector3.SmoothDamp (transform.position, Player.position, ref moveVelocity, smoothTime);
        }
    }
}