using System.Collections;
using Assets.Scripts.Common;
using UnityEngine;
using Valve.VR;

namespace Assets.Scripts.Weapons
{
    public class BasicGun : MonoBehaviour
    {
        public float ForceAmount = .25f;
        public float ForceDuration = .05f;

        private PickupOptions _pickupOptions;

        void Start()
        {
            _pickupOptions = GetComponent<PickupOptions>();
        }

        void Update()
        {
            if (_pickupOptions.Controller == null) return;

            if (_pickupOptions.Controller.GetHairTriggerDown())
            {
                StartCoroutine(RumbleController(ForceDuration, ForceAmount));
            }
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator RumbleController(float duration, float strength)
        {
            strength = Mathf.Clamp01(strength);
            var startTime = Time.realtimeSinceStartup;

            for (float i = 0; i < duration; i += Time.deltaTime)
            {
                var valveStrength = Mathf.RoundToInt(Mathf.Lerp(0, 3999, strength));
                _pickupOptions.Controller.TriggerHapticPulse((ushort)valveStrength);
                yield return null;
            }
        }
    }
}