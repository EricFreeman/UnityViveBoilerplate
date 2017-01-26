using Assets.Scripts.Common;
using UnityEngine;
using Valve.VR;

namespace Assets.Scripts.Controller
{
    public class ControllerGrab : MonoBehaviour
    {
        public EVRButtonId PickupButton;

        private SteamVR_TrackedObject _trackedObj;
        private SteamVR_Controller.Device Controller
        {
            get { return SteamVR_Controller.Input((int)_trackedObj.index); }
        }

        private GameObject _collidingObject;
        private GameObject _objectInHand;
        private bool _stayInHand;

        void Awake()
        {
            _trackedObj = GetComponent<SteamVR_TrackedObject>();
        }

        void Update()
        {
            if (Controller.GetPressDown(PickupButton))
            {
                if (_objectInHand && _stayInHand)
                {
                    ReleaseObject();
                }
                else if (_collidingObject)
                {
                    GrabObject();
                }
            }

            if (Controller.GetPressUp(PickupButton) && !_stayInHand)
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

            var pickupOptions = _objectInHand.GetComponent<PickupOptions>();
            if (pickupOptions)
            {
                // equip to correct position in hand
                if (pickupOptions.Offset != Vector3.zero)
                {
                    _objectInHand.transform.position = transform.position;
                    _objectInHand.transform.rotation = transform.rotation;
                    _objectInHand.transform.Translate(pickupOptions.Offset);
                }

                // set grab options
                _stayInHand = pickupOptions.StayInHand;

                pickupOptions.Controller = Controller;

                if (pickupOptions.OnGrab != null)
                {
                    pickupOptions.OnGrab.Invoke();
                }
            }

            AddFixedJoint(_objectInHand);
        }

        private void AddFixedJoint(GameObject obj)
        {
            var fx = gameObject.AddComponent<FixedJoint>();
            fx.connectedBody = obj.GetComponent<Rigidbody>();
            fx.anchor = obj.transform.InverseTransformPoint(transform.Find("tip").position);
            fx.breakForce = 20000;
            fx.breakTorque = 20000;
        }

        void OnJointBreak(float breakForce)
        {
            ReleaseObject();
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

            var pickupOptions = _objectInHand.GetComponent<PickupOptions>();
            if (pickupOptions && pickupOptions.OnDrop != null)
            {
                pickupOptions.Controller = null;
                pickupOptions.OnDrop.Invoke();
            }

            _objectInHand = null;
            _stayInHand = false;
        }
    }
}
