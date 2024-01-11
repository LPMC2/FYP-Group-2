using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RespawnBehaviour : TimeManager
{
    public static RespawnBehaviour Singleton;
    [SerializeField] private float m_RespawnTime = 5f;
    public float RespawnTime { get { return m_RespawnTime; } set { m_RespawnTime = value; } }
    [SerializeField] private string m_DefaultRespawnText = "Respawn in ";
    public void AddRespawnObject(GameObject obj, float time = default, DisplayBehaviour display = null, string text = default, Camera camera = null)
    {
        if(time == default) { time = m_RespawnTime; }
        if(display != null && text == default) { text = m_DefaultRespawnText; }
        StartCoroutine(RespawnCoroutine(obj, time, display, text, camera));
    }
    private void Start()
    {
        Singleton = this;
    }
    private void ResetObject(GameObject target)
    {
        target.SetActive(true);
    }
    private void SetCamera(Camera camera, bool state)
    {
        if (camera == null) return;
        camera.gameObject.SetActive(state);
    }
    private IEnumerator RespawnCoroutine(GameObject obj, float duration, DisplayBehaviour display = null, string text = null, Camera camera = null)
    {
        SetCamera(camera, true);
        yield return StartCoroutine(RunTimer(duration, display, text));
        ResetObject(obj);
        SetCamera(camera, false);
    }


}
