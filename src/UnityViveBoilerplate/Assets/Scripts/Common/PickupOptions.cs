using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Common
{
    public class PickupOptions : MonoBehaviour
    {
        public Vector3 Offset;
        public bool StayInHand;
        public UnityEvent OnGrab;
        public UnityEvent OnDrop;

        [HideInInspector]
        public SteamVR_Controller.Device Controller;
    }
}