using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace DS.Events
{


    [System.Serializable]
    public class OnCollision2DEventHandler : UnityEvent<Collision2D> { }
    [System.Serializable]
    public class OnTrigger2DEventHandler : UnityEvent<Collider2D> { }

    [RequireComponent(typeof(Collider2D))]
    public class OnCollision2DEvent : MonoBehaviour
    {

        [HideInInspector]
        public Collider2D collider;

        public OnCollision2DEventHandler onCollisionEnter2D;
        public OnCollision2DEventHandler onCollisionExit2D;

        public OnTrigger2DEventHandler onTriggerEnter2D;
        public OnTrigger2DEventHandler onTriggerExit2D;



        private void Start()
        {
            collider = GetComponent<Collider2D>();
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collider.isTrigger)
                return;

            onCollisionEnter2D.Invoke(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collider.isTrigger)
                return;

            onCollisionExit2D.Invoke(collision);
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (collider.isTrigger)
                onTriggerEnter2D.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (collider.isTrigger)
                onTriggerEnter2D.Invoke(other);
        }
    }
}