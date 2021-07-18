using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace DS.UI.Utils
{
    //Script para arrastrar imágenes de UI
    // TODO: Separar la funcionalidad de click y drag
    public class DragUIElement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public int id; //utilizado para facilitar identificar el objeto que se arrastra con el Slot que le corresponde
        public bool isDraggable = false; //Esta activada la opción de arrastrar este objeto?
        Vector2 posicionInicial;
        public Vector2 localPosicionInicial;
        public Transform dragCanvas;
        Transform startParent;
        public float speed = 10f;

        public bool limitX;
        public bool limitY;

        [SerializeField]
        Camera cam;

        Image image;

        public bool isDragging = false;
        Vector2 mouseDelta;

        public bool drop = false;


        // public OnlyDragOne managerInterface;
        //void OnValidate()
        //{
        //    if (!(movement is IOnlyDragOne))
        //        movement = null;
        //}

        public UnityEvent onBeginDrag;
        public UnityEvent onEndDrag;
        public UnityEvent onDragging;
        public UnityEvent onReturn;


        public void OnBeginDrag(PointerEventData eventData)
        {

            if (!isDraggable)
                return;

            if (!isDragging)
            {

                startParent = transform.parent;
                onBeginDrag.Invoke();
                posicionInicial = GetComponent<RectTransform>().anchoredPosition;
                localPosicionInicial = transform.localPosition;
                image.raycastTarget = false;

                transform.parent = dragCanvas;
                mouseDelta = (eventData.position) - (Vector2)(transform.position);
                //regresar = true;
                isDragging = true;
            }

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDraggable)
                return;

            if (isDragging)
            {
                onDragging.Invoke();
                Vector2 diff = ((eventData.position - mouseDelta) - (Vector2)(transform.position));
                if (limitX)
                    diff.x = 0f;
                if (limitY)
                    diff.y = 0f;

                transform.position = ((Vector2)transform.position + (diff * speed * Time.deltaTime));
            }

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDraggable)
                return;
            if (!isDragging)
                return;

            image.raycastTarget = true;
            transform.parent = startParent;
            transform.SetAsFirstSibling();

            //Debug.Log("End drag en " + gameObject.name);
            onEndDrag.Invoke();

            isDragging = false;

            image.raycastTarget = true;

            if (!drop)
            {

                transform.parent = startParent;
                transform.SetAsFirstSibling();

                //MoverInicial();
            }
        }

        //public void MoverInicial()
        //{
        //    //LeanTween.cancel(gameObject, false);
        //    if (!LeanTween.isTweening(gameObject))
        //    {
        //        id_Tween_regresar = LeanTween.move(GetComponent<RectTransform>(), posicionInicial, 0.4f).setOnComplete(() =>
        //        {
        //            drop = false;
        //            isDragging = false;
        //            onReturn.Invoke();
        //        }).id;
        //    }
        //}


        // Start is called before the first frame update
        void Start()
        {
            if (!dragCanvas)
                dragCanvas = GameObject.Find("DragCanvas").transform;

            if (!cam)
                cam = Camera.main;

            image = GetComponent<Image>();


            //LeanTween.init();
        }
    }



}