using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Game.UI;
using Game.Utility;
using UnityEngine.UI;
using System;

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
        [SerializeField] Text nameText;
        [SerializeField] Material carColorMaterial;
        [SerializeField] GameObject ballArrow;

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
        private Renderer _renderer;
        private MaterialPropertyBlock _propBlock;
        private float lerpTime;
        public GameObject ball;

        void Start () {
            rigibody = GetComponent<Rigidbody>();
            UI = FindObjectOfType<CarUI>();
            StartCoroutine(FindBall());

            currentBoost = maxBoostEnergy;
            currentCarAccel = carNormalAccel;
            dist = GetComponentInChildren<Collider>().bounds.extents.y;

            if (isLocalPlayer)
            {
                var camera = FindObjectOfType<CameraFollow>();
                camera.Player = this.transform;
                ballArrow.SetActive(true);
                Game.Core.GameMenager.Instance.LocalCar = this;
            }
            else
                ballArrow.SetActive(false);

            SetCarColor();
            SetCarName();
        }

        private IEnumerator FindBall()
        {
            while(ball == null){
                yield return new WaitForEndOfFrame();
                ball = FindObjectOfType<Ball>().gameObject;
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
            SetBallArrow();
            Debug.Log(IsGrounded());

            if (!IsGrounded()) {

                if (Input.GetButton("GetUp"))
                {
                    GetUp();
                }
                lerpTime = 0;
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

        private void GetUp()
        {
            lerpTime += Time.deltaTime * 10;
            var from = transform.rotation;
            var to = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(from, to, lerpTime);
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

        public void SetBallArrow(){

            if (ball) {
                var dist = Vector3.Distance(transform.position, ball.transform.position);
                if (dist > 25){
                    ballArrow.transform.LookAt(ball.transform);
                    ballArrow.SetActive(true);
                }
                else
                    ballArrow.SetActive(false);
            }
        }

        [Command]
        void CmdHitBall(Vector3 force, GameObject ball)
        {
            rigibody.AddForce(force);
        }


        [SyncVar(hook = "OnColorChange")]
        public Color playerColor;
        public void OnColorChange(Color c) {
            playerColor = c;
        }
        [SyncVar(hook = "OnNameChange")]
        public string playerName;
        public void OnNameChange(string s)
        {
            if (string.IsNullOrEmpty(s))
                playerName = "Player" + netId;
            else
                playerName = s;
        }
        public void SetCarName()
        {
            nameText.text = playerName;
        }
        public void SetCarColor()
        {
            _propBlock = new MaterialPropertyBlock();
            _renderer = GetComponentInChildren<Renderer>();

            // Get the current value of the material properties in the renderer.
            _renderer.GetPropertyBlock(_propBlock);

            // Assign our new value.
            _propBlock.SetColor("_CarColor", playerColor);

            // Apply the edited values to the renderer.
            _renderer.SetPropertyBlock(_propBlock);
        }
    }
}