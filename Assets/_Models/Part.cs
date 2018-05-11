using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Pause());
    }

    private IEnumerator Pause()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<ParticleSystem>().Pause();
    }



    // Update is called once per frame
    void Update () {
		
	}
}
