using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControler : MonoBehaviour {

    public List<AxleInfo> axleInfos; // the information about each individual axle
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
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
        Debug.Log(rigibody.velocity.magnitude);

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - m_OldRotation) * 0.64f;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            rigibody.velocity = velRotation * rigibody.velocity;
        }
        m_OldRotation = transform.eulerAngles.y;

        if (motor < 0 && rigibody.velocity.magnitude > 10) {
            axleInfos[0].leftWheel.brakeTorque = 200000;
            axleInfos[0].rightWheel.brakeTorque = 200000;
            axleInfos[1].leftWheel.brakeTorque = 200000;
            axleInfos[1].rightWheel.brakeTorque = 200000;
        }
        else
        {
            axleInfos[0].leftWheel.brakeTorque = 0;
            axleInfos[0].rightWheel.brakeTorque = 0;
            axleInfos[1].leftWheel.brakeTorque = 0;
            axleInfos[1].rightWheel.brakeTorque = 0;
        }
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}