using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class BasicClip : MonoBehaviour
    {
        void Start() {}

        void OnCollisionEnter(Collision collision)
        {
            var basicGun = collision.gameObject.GetComponent<BasicGun>();
            if (basicGun)
            {
                basicGun.Reload();
                Destroy(gameObject);
            }
        }
    }
}