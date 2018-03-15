using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControler : MonoBehaviour {

    public List<WheelCollider> wheels; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    Rigidbody rigibody;
    float m_OldRotation;

    void Start()
    {
        rigibody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        float motor = 1;
        float steering = Input.GetAxis("Horizontal");

        Steer(steering);
        SteerHelper();
        Drive(motor);
    }

    private void Steer(float steering)
    {
        wheels[0].steerAngle = steering * maxSteeringAngle;
        wheels[1].steerAngle = steering * maxSteeringAngle;
    }
    private void SteerHelper()
    {
        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * 0.64f;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            rigibody.velocity = velRotation * rigibody.velocity;
        }
        m_OldRotation = transform.eulerAngles.y;
    }

    bool forward = true;
    private void Drive(float motor)
    {
        var velocity = rigibody.velocity.magnitude;
        if (forward == true)
        {
            if (motor / maxMotorTorque < 0)
            {
                if (velocity > 3)
                {
                    for (int i = 0; i < wheels.Count; i++){
                        wheels[i].brakeTorque = 20000;
                        wheels[i].motorTorque = 0;
                    }
                    return;
                }
            }
            else
            {
                for (int i = 0; i < wheels.Count; i++)
                {
                    wheels[i].motorTorque = maxMotorTorque * motor;
                    wheels[i].brakeTorque = 0;
                }
            }
        }
        else if(forward == false)
        {
            if (motor / maxMotorTorque > 0)
            {
                if (velocity > 3)
                {
                    for (int i = 0; i < wheels.Count; i++)
                    {
                        wheels[i].brakeTorque = 20000;
                        wheels[i].motorTorque = 0;
                    }
                    return;
                }
            }
            else
            {
                for (int i = 0; i < wheels.Count; i++)
                {
                    wheels[i].motorTorque = maxMotorTorque * motor;
                    wheels[i].brakeTorque = 0;
                }
            }
        }
        

        if (motor/maxMotorTorque > 0)
        {
            forward = true;
        }
        else if(motor/maxMotorTorque < 0)
        {
            forward = false;
        }

    }
}
