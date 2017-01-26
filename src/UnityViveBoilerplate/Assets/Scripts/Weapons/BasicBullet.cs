using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class BasicBullet : MonoBehaviour
    {
        public float Speed = 3f;
        public float Damage = 1;
        public float ImpactForce = 10f;

        void Update()
        {
            transform.Translate(0, 0, Speed * Time.deltaTime);
        }

        void OnTriggerEnter(Collider collider)
        {
            var rigidbody = collider.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.AddForce(transform.forward*ImpactForce, ForceMode.Impulse);
            }

            Destroy(gameObject);
        }
    }
}