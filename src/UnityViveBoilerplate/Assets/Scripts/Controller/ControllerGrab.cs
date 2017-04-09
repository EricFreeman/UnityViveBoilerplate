﻿using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class ControllerGrab : MonoBehaviour
    {
        private SteamVR_TrackedObject _trackedObj;
        private SteamVR_Controller.Device Controller
        {
            get { return SteamVR_Controller.Input((int)_trackedObj.index); }
        }

        private GameObject _collidingObject;
        private GameObject _objectInHand;

        void Awake()
        {
            _trackedObj = GetComponent<SteamVR_TrackedObject>();
        }

        void Update()
        {
            if (Controller.GetHairTriggerDown())
            {
                if (_collidingObject)
                {
                    GrabObject();
                }
            }

            if (Controller.GetHairTriggerUp())
            {
                if (_objectInHand)
                {
                    ReleaseObject();
                }
            }
        }

        private void SetCollidingObject(Collider col)
        {
            if (_collidingObject || !col.GetComponent<Rigidbody>())
            {
                return;
            }

            _collidingObject = col.gameObject;
        }

        public void OnTriggerEnter(Collider other)
        {
            SetCollidingObject(other);
        }

        public void OnTriggerStay(Collider other)
        {
            SetCollidingObject(other);
        }

        public void OnTriggerExit(Collider other)
        {
            if (!_collidingObject)
            {
                return;
            }

            _collidingObject = null;
        }

        private void GrabObject()
        {
            _objectInHand = _collidingObject;
            _collidingObject = null;

            var joint = AddFixedJoint();
            joint.connectedBody = _objectInHand.GetComponent<Rigidbody>();
        }

        private FixedJoint AddFixedJoint()
        {
            var fx = gameObject.AddComponent<FixedJoint>();
            fx.breakForce = 20000;
            fx.breakTorque = 20000;
            return fx;
        }

        private void ReleaseObject()
        {
            if (GetComponent<FixedJoint>())
            {
                GetComponent<FixedJoint>().connectedBody = null;
                Destroy(GetComponent<FixedJoint>());

                _objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
                _objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
            }

            _objectInHand = null;
        }
    }
}