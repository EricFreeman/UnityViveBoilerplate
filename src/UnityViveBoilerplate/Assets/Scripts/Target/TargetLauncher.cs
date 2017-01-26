using UnityEngine;

namespace Assets.Scripts.Target
{
    public class TargetLauncher : MonoBehaviour
    {
        public GameObject Target;
        public AudioClip LaunchSound;

        public float RandomLaunchX = 1.5f;
        public float MinLaunchDelay = .5f;
        public float MaxLaunchDelay = 5;

        public float MinLaunchForceX = 0;
        public float MaxLaunchForceX = 5;

        public float MinLaunchForceY = 5;
        public float MaxLaunchForceY = 7.5f;

        private float _launchDelay;

        void Start()
        {
            _launchDelay = Random.Range(MinLaunchDelay, MaxLaunchDelay);
        }

        void Update()
        {
            _launchDelay -= Time.deltaTime;

            if (_launchDelay <= 0)
            {
                _launchDelay = Random.Range(MinLaunchDelay, MaxLaunchDelay);
                AudioSource.PlayClipAtPoint(LaunchSound, transform.position);

                var target = Instantiate(Target);
                target.transform.position = transform.position + new Vector3(Random.Range(-RandomLaunchX, RandomLaunchX), 0, 0);
                target.GetComponent<Rigidbody>().AddForce(Random.Range(MinLaunchForceX, MaxLaunchForceX), Random.Range(MinLaunchForceY, MaxLaunchForceY), 0, ForceMode.Impulse);
            }
        }
    }
}