using UnityEngine;

namespace Assets.Scripts.Target
{
    public class TargetBreak : MonoBehaviour
    {
        public AudioClip Break;

        void OnTriggerEnter(Collider collider)
        {
            AudioSource.PlayClipAtPoint(Break, transform.position);
            Destroy(gameObject);
        }
    }
}