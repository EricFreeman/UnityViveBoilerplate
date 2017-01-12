using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class LaserPointer : MonoBehaviour
    {
        public Transform CameraRigTransform;
        public Transform HeadTransform;

        public LayerMask TeleportMask;
        public GameObject TeleportReticlePrefab;
        public Vector3 TeleportReticleOffset;
        public GameObject LaserPrefab;

        private GameObject _reticle;
        private Transform _teleportReticleTransform;
        private bool _shouldTeleport;
        private GameObject _laser;
        private Transform _laserTransform;
        private Vector3 _hitPoint;

        private SteamVR_TrackedObject _trackedObj;
        private SteamVR_Controller.Device Controller
        {
            get { return SteamVR_Controller.Input((int)_trackedObj.index); }
        }

        void Awake()
        {
            _trackedObj = GetComponent<SteamVR_TrackedObject>();
        }

        void Start()
        {
            _laser = Instantiate(LaserPrefab);
            _laserTransform = _laser.transform;
            _reticle = Instantiate(TeleportReticlePrefab);
            _teleportReticleTransform = _reticle.transform;
        }

        void Update()
        {
            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
            {
                RaycastHit hit;
                if (Physics.Raycast(_trackedObj.transform.position, transform.forward, out hit, 100, TeleportMask))
                {
                    _hitPoint = hit.point;
                    ShowLaser(hit);
                }
                else
                {
                    HideLaser();
                }
            }
            else
            {
                HideLaser();
            }

            if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && _shouldTeleport)
            {
                Teleport();
            }
        }

        private void ShowLaser(RaycastHit hit)
        {
            _laser.SetActive(true);
            _laserTransform.position = Vector3.Lerp(_trackedObj.transform.position, _hitPoint, .5f);
            _laserTransform.LookAt(_hitPoint);
            _laserTransform.localScale = new Vector3(_laserTransform.localScale.x, _laserTransform.localScale.y, hit.distance);

            _reticle.SetActive(true);
            _teleportReticleTransform.position = _hitPoint + TeleportReticleOffset;
            _shouldTeleport = true;
        }

        private void HideLaser()
        {
            _laser.SetActive(false);
            _reticle.SetActive(false);
        }

        private void Teleport()
        {
            _shouldTeleport = false;
            _reticle.SetActive(false);
            var difference = CameraRigTransform.position - HeadTransform.position;
            difference.y = 0;
            CameraRigTransform.position = _hitPoint + difference;
        }
    }
}