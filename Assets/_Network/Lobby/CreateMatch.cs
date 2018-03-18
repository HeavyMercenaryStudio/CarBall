using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CreateMatch : NetworkBehaviour {

    private NetworkStartPosition[] spawnPoints;
    private NetworkManager networkManager;
    // Use this for initialization
    void Start () {
        networkManager = this.GetComponent<NetworkManager>();
        if (Network.isServer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void CollectPlayers()
    {

    }

}
