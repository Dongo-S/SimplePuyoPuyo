using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace DS.Events
{


    [System.Serializable]
    public class OnCollisionEventHandler : UnityEvent<Collision> { }
    [System.Serializable]
    public class OnTriggerEventHandler : UnityEvent<Collider> { }

    [RequireComponent(typeof(Collider))]
    public class OnCollisionEvent : MonoBehaviour
    {

        [HideInInspector]
        public Collider collider;

        public OnCollisionEventHandler onCollisionEnter;
        public OnCollisionEventHandler onCollisionExit;

        public OnTriggerEventHandler onTriggerEnter;
        public OnTriggerEventHandler onTriggerExit;
        


        private void Start()
        {
            collider = GetComponent<Collider>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collider.isTrigger)
                return;

            onCollisionEnter.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collider.isTrigger)
                return;

            onCollisionExit.Invoke(collision);
        }


        private void OnTriggerEnter(Collider other)
        {
            if (collider.isTrigger)
                onTriggerEnter.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (collider.isTrigger)
                onTriggerEnter.Invoke(other);
        }
    }
}