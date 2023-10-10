using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMap : MonoBehaviour
{


    public RectTransform panelRectTransform;
    public Vector2 newPosition;
    public Camera maincamera;
    private void Start()
    {
        if (is_needChangeRotate)
        {
            //SaveMapPos = obj.transform.position;
        }
    }

    public void ChangePanelPosition()
    {
        panelRectTransform.anchoredPosition = newPosition;
    }

    private float rotateSpeed = 1.5f;
    private float zoomSpeed = 15f;
    private float zoomAmount = 60f;

    private GameObject gameObjectValue;

    [SerializeField] private Vector3 mapPos;
    [SerializeField] private Vector3 SaveMapPos;
    public bool is_needChangeRotate = false;
    GameObject obj;
    // Update is called once per frame
    private void Update()
    {
        ViewAreaController();
        changeposition();
    }
    private Quaternion cameraRotation;
    private void ViewAreaController()
    {
        if (is_needChangeRotate)
        {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            cameraRotation = maincamera.transform.rotation;
            transform.rotation = Quaternion.Euler(0f, 0f, -cameraRotation.eulerAngles.y - 137);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }

    public RectTransform rectTransform;
    public void changeposition()
    {
        mapPos = maincamera.transform.position;
        Vector2 ve = new Vector2(mapPos.x-SaveMapPos.x, mapPos.y-SaveMapPos.y);
        changeMap(ve);
        SaveMapPos = maincamera.transform.position;
    }
    public void changeMap(Vector2 pos)
    {
        float step = 20 * Time.deltaTime;
        pos +=rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, pos, step);
    }
}
