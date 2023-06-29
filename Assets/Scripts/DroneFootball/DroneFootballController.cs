using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DroneFootball
{
    public class DroneFootballController : DroneController
    {
        public FootballController footballController;
        [SerializeField] private Rigidbody rb;

        private List<DroneEngine> _engines;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        private float _isMove;
        private bool _isStopAlready;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            _engines = GetComponentsInChildren<DroneEngine>().ToList();
            footballController = FindObjectOfType<FootballController>();
            footballController.droneFootballController = this;
            isSimpleMode = footballController.isSimpleMode;
            footballController.playerCamera = FindObjectOfType<CinemachineBrain>();
        }

        private void Start()
        {
            FindObjectOfType<Server>().player = this;
        }

        private void FixedUpdate()
        {
            currentSpeed = rb.velocity.magnitude / 8.2f * 40f;
            currentPercentSpeed = rb.velocity.magnitude / 8.2f * 100f;
            if (footballController.isGameStart && !isStop)
            {
                _isMove = 0;
                _isMove = Mathf.Abs(cyclic.x) + Mathf.Abs(cyclic.y) + Mathf.Abs(pedals) + Mathf.Abs(throttle);
                DroneMove();
            }
            else if (footballController.isGameStart && !_isStopAlready)
            {
                StartCoroutine(DroneStop());
            }
        }

        private void OnCyclic(InputValue value)
        {
            cyclic = value.Get<Vector2>();
        }

        private void OnPedals(InputValue value)
        {
            pedals = value.Get<float>();
        }

        private void OnThrottle(InputValue value)
        {
            throttle = value.Get<float>();
        }

        private void DroneMove()
        {
            if (_isMove != 0 || isSimpleMode)
                foreach (var engine in _engines)
                    engine.UpdateEngine(rb, throttle);
            CheckDroneHover();

            var pitch = cyclic.y * minMaxPitch;
            var roll = -cyclic.x * minMaxRoll;
            yaw += pedals * yawPower;

            _finalPitch = Mathf.Lerp(_finalPitch, pitch, Time.deltaTime * lerpSpeed);
            _finalRoll = Mathf.Lerp(_finalRoll, roll, Time.deltaTime * lerpSpeed);
            _finalYaw = Mathf.Lerp(_finalYaw, yaw, Time.deltaTime * lerpSpeed);

            var rot = Quaternion.Euler(_finalPitch, _finalYaw, _finalRoll);
            rb.MoveRotation(rot);
        }

        private void CheckDroneHover()
        {
            if (isSimpleMode && _isMove == 0)
                rb.drag = 5;
            else
                rb.drag = 0.5f;
        }

        private IEnumerator DroneStop()
        {
            _isStopAlready = true;
            isStop = true;
            rb.drag = 10;
            rb.angularDrag = 10;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            yield return new WaitForSeconds(2);
            _isStopAlready = false;
            isStop = false;
        }
    }
}