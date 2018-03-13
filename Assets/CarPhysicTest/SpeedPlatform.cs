using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SpeedPlatform : NetworkBehaviour {

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
            car.CurrentBoost += (speedPercentageAmount/car.MaxBoost) * car.MaxBoost;
            RpcStartCoroutine();
            StartCoroutine(RespawnBoost());
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
