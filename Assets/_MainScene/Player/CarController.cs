using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Game.UI;
using Game.Utility;

namespace Game.Car { 

    [NetworkSettings(channel = 0, sendInterval = 01f)]
    public class CarController : NetworkBehaviour {
   
        [Header("Movement")]
        [SerializeField] float carMaxSpeed;
        [SerializeField] float carNormalAccel;
        [SerializeField] float carBoostedAccel;
        [SerializeField] float rotSpeed;

        [Header("Utility")]
        [SerializeField] float kickForce;
        [SerializeField] float maxBoostEnergy;
        [SerializeField] ParticleSystem boostParticle;

        Rigidbody rigibody;
        float currentBoost;
        float currentCarAccel;
        public float CurrentBoost
        {
            get {
                return currentBoost;
            }
            set {
                currentBoost = Mathf.Clamp(value, 0, maxBoostEnergy);
                if (isLocalPlayer) UI.SetEnergyBarValue(currentBoost / maxBoostEnergy);
            }
        }
        public float MaxBoost
        { get { return maxBoostEnergy; } }

        CarUI UI;
        // Use this for initialization

        void Start () {
            rigibody = GetComponent<Rigidbody>();
            UI = FindObjectOfType<CarUI>();

            currentBoost = maxBoostEnergy;
            currentCarAccel = carNormalAccel;

            if (isLocalPlayer)
            {
                var camera = FindObjectOfType<CameraFollow>();
                camera.Player = this.transform;
            }

        }
	
	    // Update is called once per frame
	    void FixedUpdate ()
        {
            if (!isLocalPlayer)
                return;

            Move();
        }
   
        private void Move()
        {
            //Get input
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            SpeedBoost(ref v);


            if (v < 0) h = -h; //turn fix

           //Move car
            var force = transform.forward * currentCarAccel * v;
            rigibody.AddForce(force);

            //Lock max ca speed
            if (rigibody.velocity.magnitude > carMaxSpeed)
                rigibody.velocity = carMaxSpeed * rigibody.velocity.normalized; 


            //Rotate 
            var angle = transform.rotation.eulerAngles.y;
            var rot = Quaternion.Euler(0, h * rotSpeed * Time.deltaTime + angle, 0);
            rigibody.MoveRotation(rot);
        }

        private void SpeedBoost(ref float v)
        {
            //If boost button click
            if (Input.GetKey(KeyCode.Space))
            { 
                if (CurrentBoost == 0)
                {  // if no more boost
                    ResetCarSpeed();
                    return;
                }
                boostParticle.startLifetime = 1f;
                currentCarAccel = carBoostedAccel; //incrementSpeedBy boost speed
                CurrentBoost -= 0.25f; // boost lost per frame
                v = 1; // drive only forward 
            }
            else if (Input.GetKeyUp(KeyCode.Space))
                ResetCarSpeed();

        }

        private void ResetCarSpeed()
        {
            boostParticle.startLifetime = 0.15f;
            currentCarAccel = carNormalAccel;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!isLocalPlayer)
                return;

            if (collision.gameObject.layer == Layers.BALL)
            {
                var ball = collision.gameObject.GetComponent<Rigidbody>();
                var force = rigibody.velocity * kickForce;
                if (isServer)  ball.AddForce(force);
            
                CmdHitBall(force, ball.gameObject);
            }
        
        }

        [Command]
        void CmdHitBall(Vector3 force, GameObject ball)
        {
            rigibody.AddForce(force);
        }
    }
}