using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel =0,sendInterval =0.1f)]
public class CarController : NetworkBehaviour {

    [SerializeField] float maxBoost;
    [SerializeField] float carSpeed;
    [SerializeField] float carBoostedSpeed;
    [SerializeField] float rotSpeed;
    [SerializeField] float kickForce;

    Rigidbody rigibody;
    [SyncVar(hook = "OnChangeBoost")]
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
        if (!isLocalPlayer)
            return;

        //UI = FindObjectOfType<CarUI>();
        UI = FindObjectOfType<CarUI>();
        UI.energyBar = GameObject.FindGameObjectWithTag("BoostPointer").GetComponent<Image>();
        //Image boostFill= GameObject.FindGameObjectWithTag("BoostPointer").GetComponent<Image>();
        //Image boostFill = GameObject.Find("CircleFill").GetComponent<Image>();
        //if (boostFill != null)
        //    UI.energyBar = boostFill;
        currentBoost = maxBoost;
        currentCarSpeed = carSpeed;
    }

    void OnChangeBoost(float boost)
    {
        UI.SetEnergyBarValue(boost / maxBoost);
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isLocalPlayer)
            return;
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
        if (!isLocalPlayer)
            return;
        if (collision.gameObject.layer == Layers.BALL)
        {
            var ball = collision.gameObject.GetComponent<Rigidbody>();
            var force = transform.forward * kickForce;
            if(isServer)
                rigibody.AddForce(force);
            CmdHitBall(force, ball.gameObject);
        }
    }

    [Command]
    void CmdHitBall(Vector3 force, GameObject ball)
    {
        rigibody.AddForce(force);

    }
}
