using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMap : MonoBehaviour
{


    public Camera maincamera;

    private void Start()
    {
        SaveMapPos = maincamera.transform.position;
    }

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

    public GameObject[] Map;

    [SerializeField] private Vector3 mapPos;
    [SerializeField] private Vector3 SaveMapPos;

    public void changeposition()
    {
        mapPos = maincamera.transform.position;
        Vector2 ve = new Vector2(-(mapPos.x - SaveMapPos.x) * 100, -(mapPos.z - SaveMapPos.z) * 100);

        //for (int i=0; i>-Map.Length;i--)
        //{
        //    if(i == mapPos.y)
        //    {
        //        Map[-i].SetActive(true);
        //    }
        //    else
        //    {
        //        Map[-i].SetActive(false);
        //    }

        //}
        changeMap(ve);
        SaveMapPos = maincamera.transform.position;
    }
    public void changeMap(Vector2 pos)
    {
        RectTransform Rect1;
        for (int i = 0; i < Map.Length; i++)
        {
            Rect1 = Map[i].GetComponent<RectTransform>();
            Rect1.anchoredPosition = pos + Rect1.anchoredPosition;

        }
        
        
    }
}
