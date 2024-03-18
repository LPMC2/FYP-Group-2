using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryLoadoutImageSaver :MonoBehaviour
{
    public static InventoryLoadoutImageSaver Singleton;
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private string savePath = "/Inventory/Images/";
    public string PersistentPath { get { return Application.persistentDataPath + savePath; } }
    public string SavePath { get { return savePath; } }
    [SerializeField] private List<Sprite> m_itemSpriteList = new List<Sprite>();
    public List<Sprite> ItemSprites { get { return m_itemSpriteList; } }
    [SerializeField] private Camera targetCamera;
    public void Awake()
    {
        Singleton = this;
    }
    public void StartCapture()
    {
        int id = 0;
        foreach(Item item in itemSO.item)
        {
            GameObject captureItem = Instantiate(item.model);
            captureItem.SetActive(true);
            float cameraSize = targetCamera.orthographicSize;
            captureItem.transform.SetParent(targetCamera.transform);
            captureItem.transform.eulerAngles = item.captureAngle;
            captureItem.transform.localPosition = Vector3.forward * 10 + item.captureOffset;
            ModelPictureSaver.CaptureAndSaveImage(targetCamera,captureItem, savePath, id.ToString(), true, true, item.CustomBuffer, true);
            m_itemSpriteList.Add(ModelPictureSaver.LoadSpriteFromFile(savePath + id + ".png", true));
            targetCamera.orthographicSize = cameraSize;
            id++;
        }
    }

}
