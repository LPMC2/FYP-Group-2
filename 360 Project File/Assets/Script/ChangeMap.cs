using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMap : MonoBehaviour
{


    public Vector2 newPosition;
    public Camera maincamera;
    private void Start()
    {
        SaveMapPos = obj.transform.position;
        F1MapRectTransform = Map1.GetComponent<RectTransform>();
        F2MapRectTransform = Map2.GetComponent<RectTransform>();    
    }

    private float rotateSpeed = 1.5f;
    private float zoomSpeed = 15f;
    private float zoomAmount = 60f;

    private GameObject gameObjectValue;

    [SerializeField] GameObject obj;
    // Update is called once per frame
    private void Update()
    {
            ViewAreaController();
            changeposition();
    }
    private Quaternion cameraRotation;
    private void ViewAreaController()
    {
            cameraRotation = maincamera.transform.rotation;
            transform.rotation = Quaternion.Euler(0f, 0f, -cameraRotation.eulerAngles.y - 137);

    }

    public GameObject Map1;
    public GameObject Map2;

    private RectTransform F1MapRectTransform;
    private RectTransform F2MapRectTransform;

    [SerializeField] private Vector3 mapPos;
    [SerializeField] private Vector3 SaveMapPos;
    [SerializeField] private Vector3 mapPos1;
    [SerializeField] private Vector3 SaveMapPos1;
    public void changeposition()
    {
        mapPos = maincamera.transform.position;
        Vector2 ve = new Vector2(-(mapPos.x - SaveMapPos.x) * 100, -(mapPos.z - SaveMapPos.z) * 100);

        if (mapPos.y == 0)
        {
            //Map1.SetActive(true);
            //Map2.SetActive(false);
        }
        else
        {
            //Map1.SetActive(false);
            //Map2.SetActive(true);
        }
        
        changeMap(ve);
        SaveMapPos = maincamera.transform.position;
    }
    public void changeMap(Vector2 pos)
    {
        Vector2 M1;
        Vector2 M2;

        M1 = pos + F1MapRectTransform.anchoredPosition;
        M2 = pos + F2MapRectTransform.anchoredPosition;

        F1MapRectTransform.anchoredPosition = M1;
        F2MapRectTransform.anchoredPosition = M2;
    }
}
