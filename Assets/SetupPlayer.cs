using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupPlayer : NetworkBehaviour {

    GameObject camera;
    CameraUI.CameraFollow cf;
    
	// Use this for initialization
	void Start () {
        if (!isLocalPlayer)
            return;
        camera = GameObject.Find("CameraArm");
        cf = camera.GetComponent<CameraUI.CameraFollow>();
        cf.Player = this.transform;

	}

}
