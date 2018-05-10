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
        [SerializeField] float rotSpeed = 5000;

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

        float dist;

        void Start () {
            rigibody = GetComponent<Rigidbody>();
            UI = FindObjectOfType<CarUI>();

            currentBoost = maxBoostEnergy;
            currentCarAccel = carNormalAccel;
            dist = GetComponentInChildren<Collider>().bounds.extents.y;

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
            Debug.Log(IsGrounded());


            if (!IsGrounded()) {

                if(Input.GetKeyDown(KeyCode.R))
                     transform.rotation = Quaternion.identity;

                return;
            } 


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
            var rot = Quaternion.Euler(transform.rotation.x, h * rotSpeed * Time.deltaTime + angle, transform.rotation.z);
            rigibody.MoveRotation(rot);
            //if (h < 0) h = -1;
            //else if (h > 0) h = 1;
            //var rot = new Vector3(0, h, 0) * rotSpeed;
            //rigibody.AddTorque(rot);

            var pos = new Vector3(transform.position.x, transform.position.y + dist, transform.position.z);
            Debug.DrawRay(pos,-transform.up * distToGround);
        }

        public float distToGround = 1;
        bool IsGrounded() {
            int mask = 1 << 10;
            var pos = new Vector3(transform.position.x, transform.position.y + dist, transform.position.z);
            return Physics.Raycast(pos, -transform.up, distToGround);
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
                boostParticle.startLifetime = 2f;
                boostParticle.emissionRate = 120f;
                currentCarAccel = carBoostedAccel; //incrementSpeedBy boost speed
                CurrentBoost -= 0.25f; // boost lost per frame
                v = 1; // drive only forward 
            }
            else if (v > 0)
                ResetCarSpeed();
            else
                boostParticle.emissionRate = 20f;

        }

        private void ResetCarSpeed()
        {
            boostParticle.startLifetime = 1f;
            boostParticle.emissionRate = 80f;
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