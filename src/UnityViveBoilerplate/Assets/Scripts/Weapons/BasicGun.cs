using System.Collections;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class BasicGun : MonoBehaviour
    {
        public GameObject BulletPrefab;

        public int ClipSize = 12;
        private int _currentClip;

        public float CooldownSpeed = .25f;
        private float _currentCooldownSpeed;

        public float RumbleForceAmount = .25f;
        public float RumbleForceDuration = .05f;

        public float EmptyRumbleForceAmount = .15f;
        public float EmptyRumbleForceDuration = .025f;

        public AudioClip FireSound;
        public AudioClip ReloadSound;
        public AudioClip EmptySound;

        private PickupOptions _pickupOptions;
        private GunState _gunState = GunState.ReadyToFire;
        private AudioSource _audioSource;

        void Start()
        {
            _pickupOptions = GetComponent<PickupOptions>();
            _audioSource = GetComponent<AudioSource>();
            _currentClip = ClipSize;
        }

        void Update()
        {
            if (_pickupOptions.Controller == null) return;

            UpdateRumble();

            if (_gunState == GunState.ReadyToFire)
            {
                LoadedUpdate();
            }
            else if (_gunState == GunState.CoolingDown)
            {
                CoolingDownUpdate();
            }
            else if (_gunState == GunState.Empty)
            {
                EmptyState();
            }
        }

        public void Reload()
        {
            _currentClip = ClipSize;
            _audioSource.PlayOneShot(ReloadSound);
        }

        private void UpdateRumble()
        {
            if (_pickupOptions.Controller.GetHairTriggerDown())
            {
                StartCoroutine(RumbleController(
                    _gunState == GunState.ReadyToFire ? RumbleForceDuration : EmptyRumbleForceDuration,
                    _gunState == GunState.ReadyToFire ? RumbleForceAmount : EmptyRumbleForceAmount));
            }
        }

        private void LoadedUpdate()
        {
            if (_pickupOptions.Controller.GetHairTriggerDown())
            {
                var bullet = Instantiate(BulletPrefab);
                var tip = transform.Find("Tip");
                bullet.transform.position = tip.transform.position;
                bullet.transform.rotation = tip.transform.rotation;

                _currentClip--;
                _audioSource.PlayOneShot(FireSound);
                if (_currentClip <= 0)
                {
                    _gunState = GunState.Empty;
                    return;
                }

                _gunState = GunState.CoolingDown;
                _currentCooldownSpeed = CooldownSpeed;
            }
        }

        private void CoolingDownUpdate()
        {
            _currentCooldownSpeed -= Time.deltaTime;
            if (_currentCooldownSpeed <= 0)
            {
                _gunState = GunState.ReadyToFire;
            }
        }

        private void EmptyState()
        {
            if (_pickupOptions.Controller.GetHairTriggerDown())
            {
                _audioSource.PlayOneShot(EmptySound);
            }

            if (_currentClip > 0)
            {
                _gunState = GunState.ReadyToFire;
            }
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator RumbleController(float duration, float strength)
        {
            strength = Mathf.Clamp01(strength);

            for (float i = 0; i < duration; i += Time.deltaTime)
            {
                var valveStrength = Mathf.RoundToInt(Mathf.Lerp(0, 3999, strength));
                _pickupOptions.Controller.TriggerHapticPulse((ushort)valveStrength);
                yield return null;
            }
        }
    }

    public enum GunState
    {
        ReadyToFire,
        CoolingDown,
        Empty
    }
}