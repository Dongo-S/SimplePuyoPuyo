using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UltEvents;
using UnityEngine.Events;

namespace DS.Events
{


    public class ExecuteOn : MonoBehaviour
    {
        [SerializeField] Method executeOn;


        //[SerializeField] UltEvent execute;
        [SerializeField] UnityEvent execute;


        private void Start()
        {
            if (executeOn == Method.Start)
            {
                execute?.Invoke();
            }
        }

        private void Update()
        {
            if (executeOn == Method.Update)
            {
                execute?.Invoke();
            }
        }

        private void OnEnable()
        {
            if (executeOn == Method.OnEnable)
            {
                execute?.Invoke();
            }
        }

        private void OnDisable()
        {
            if (executeOn == Method.OnDisable)
            {
                execute?.Invoke();
            }
        }
    }


}


[System.Serializable]
public enum Method
{
    Start,
    Update,
    OnDisable,
    OnEnable

}
