using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControll : MonoBehaviour {


    [SerializeField] float minPitch = 1.0f;
    [SerializeField] float maxPitch = 2.0f;
    [SerializeField] float maxSpeed = 100f;
    Rigidbody rb;
    AudioSource audiosrc;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audiosrc = GetComponent<AudioSource>();
    }
    private void FixedUpdate()
    {
         float pitchModifier;
        var currentSpeed = rb.velocity.magnitude;
        pitchModifier = maxPitch - minPitch;
        audiosrc.pitch = minPitch + (currentSpeed/maxSpeed)*pitchModifier;
    }
}
