using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DS.Movements
{
    public class MoveGameObject : MonoBehaviour
    {

        public float speed = 150f;

        [SerializeField] bool moving;
        [SerializeField] Directions direction;
        Vector3 dir;
        [SerializeField] UnityEvent onMove;
       
        public void StartMoving()
        {
            moving = true;
            this.dir = Direction(direction);
        }

        public void StopMoving()
        {
            moving = false;
            dir = Vector3.zero;
        }


        public static Vector3 Direction(Directions direction)
        {
            return direction switch
            {
                Directions.Up => Vector3.up,
                Directions.Right => Vector3.right,
                Directions.Down => Vector3.down,
                Directions.Left => Vector3.left,
                Directions.UpLeft => (Vector3.up + Vector3.left).normalized,
                Directions.UpRight => (Vector3.up + Vector3.right).normalized,
                Directions.DownLeft => (Vector3.down + Vector3.left).normalized,
                Directions.DownRight=> (Vector3.down + Vector3.right).normalized,
                _ => throw new System.NotImplementedException(),
            };
        }

        private void Update()
        {
            if (moving)
            {
                transform.Translate(dir * Time.deltaTime * speed);
                onMove.Invoke();
            }
        }
        
        public bool IsMoving()
        {
            return moving;
        }

        private void OnDisable()
        {
            moving = false;
        }
    }


    [System.Serializable]
    public enum Directions
    {
        None,
        Up = 1,
        Down,
        Right,
        Left,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }
}