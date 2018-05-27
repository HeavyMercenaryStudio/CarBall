using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour {

    [SerializeField] float rotPerMin = 5f;

	// Update is called once per frame
	void Update () {

        var degPerFrame = Time.deltaTime / 60 * 360 * rotPerMin;
        transform.RotateAround (transform.position, transform.up, degPerFrame);
    }
}
