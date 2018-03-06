using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel =1,sendInterval =0.033f)]
public class NetSending : NetworkBehaviour {

    Vector3 lastPos;
    Quaternion lastRot;
    public float stepMove=1f;
    public float stepRotation = 1f;
    [ClientRpc]
    public void RpcSyncedPos(Vector3 syncedPos, Quaternion syncedRotation)
    {
        //transform.position = syncedPos;
        lastPos = transform.position;
        lastRot = transform.rotation;
        //transform.position = Vector3.Lerp(lastPos,syncedPos,lerpMove*Time.deltaTime);
        transform.position = Vector3.MoveTowards(lastPos, syncedPos, stepMove*Time.deltaTime);
        transform.rotation = Quaternion.Lerp(lastRot, syncedRotation, stepRotation * Time.deltaTime);
    }

    [Server]
    private void UpdateClientsPos()
    {
        RpcSyncedPos(transform.position, transform.rotation);
    }

    void Update()
    {
        UpdateClientsPos();
    }
}
