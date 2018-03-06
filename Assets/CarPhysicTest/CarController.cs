using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    [SerializeField] float maxBoost;
    [SerializeField] float carSpeed;
    [SerializeField] float carBoostedSpeed;
    [SerializeField] float rotSpeed;
    [SerializeField] float kickForce;

    Rigidbody rigibody;
    float currentBoost;
    float currentCarSpeed;
    public float CurrentBoost
    {
        get {
            return currentBoost;
        }
        set {
            currentBoost = Mathf.Clamp(value, 0, maxBoost);
            UI.SetEnergyBarValue(currentBoost / maxBoost);
        }
    }
    public float MaxBoost
    { get { return maxBoost; } }

    CarUI UI;
    // Use this for initialization
    void Start () {
        rigibody = GetComponent<Rigidbody>();
        UI = GetComponent<CarUI>();

        currentBoost = maxBoost;
        currentCarSpeed = carSpeed;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Move();
        SpeedBoost();
    }

    private void Move()
    {
        //Get input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //Move position
        var move = v * transform.forward * currentCarSpeed * Time.deltaTime;
        move += transform.position;
        rigibody.MovePosition(move);

        //Rotate 
        var angle = transform.rotation.eulerAngles.y;
        var rot = Quaternion.Euler(0, h * rotSpeed * Time.deltaTime + angle, 0);
        rigibody.MoveRotation(rot);
    }

    private void SpeedBoost()
    {
        //If boost button click
        if (Input.GetKey(KeyCode.Space))
        { //TODO make this as crossplatform 
            if (CurrentBoost == 0){  // if no more boost
                currentCarSpeed = carSpeed; 
                return;
            }
            currentCarSpeed = carBoostedSpeed; //incrementSpeedBy boost speed
            CurrentBoost -= 0.25f; // boost lost per frame
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            currentCarSpeed = carSpeed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == Layers.BALL)
        {
            var ball = collision.gameObject.GetComponent<Rigidbody>();
            var force = transform.forward * kickForce;
            ball.AddForce(force);
            ball.AddTorque(force);
        }
    }
}
