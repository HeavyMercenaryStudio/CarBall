using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SpeedPack : NetworkBehaviour {

    [SerializeField] float speedPercentageAmount;
    [SerializeField] float speedBoostRespawnTime;

    bool isReady = true;

    ParticleSystem particle;

    [Server]
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == Layers.PLAYER)
        {
            AddSpeedBoost(other);
        }
    }
    
    private void AddSpeedBoost(Collider other)
    {
        if (!isReady) return;

        var car = other.GetComponent<CarController>();
        if (car)
        {
            //car.CurrentBoost += (speedPercentageAmount/car.MaxBoost) * car.MaxBoost;
            //Debug.Log(car.GetComponent<NetworkIdentity>().netId);
            RpcUpdateBoost(car.netId);
            RpcStartCoroutine();
            StartCoroutine(RespawnBoost());
        }
    }
 
    [ClientRpc]
    private void RpcUpdateBoost(NetworkInstanceId netId)
    {
        //if()
        var list = FindObjectsOfType<CarController>();
        for (int i = 0; i < list.Length; i++)
        {
            if(list[i].netId==netId)
            {
                list[i].CurrentBoost += (speedPercentageAmount / list[i].MaxBoost) * list[i].MaxBoost;
                break;
            }
        }
    }

    [ClientRpc]
    private void RpcStartCoroutine()
    {
        Debug.Log("Odpalony");
        StartCoroutine(RespawnBoost());
    }

    private IEnumerator RespawnBoost()
    {
        isReady = false;
        particle.gameObject.SetActive(false);
        yield return new WaitForSeconds(speedBoostRespawnTime);
        particle.gameObject.SetActive(true);
        isReady = true;
    }

    void Start(){
        particle = GetComponentInChildren<ParticleSystem>();
    }
}
