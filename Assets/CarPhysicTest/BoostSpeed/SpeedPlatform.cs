using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPlatform : MonoBehaviour {

    [SerializeField] float speedPercentageAmount;
    [SerializeField] float speedBoostRespawnTime;

    bool isReady = true;

    ParticleSystem particle;

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
            StartCoroutine(RespawnBoost());
        }
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
