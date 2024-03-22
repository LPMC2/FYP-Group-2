using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RespawnBehaviour : TimeManager
{
    public static RespawnBehaviour Singleton;
    [SerializeField] private float m_RespawnTime = 5f;
    [SerializeField] private static float invincibleTimerOnRespawn = 3f;
    public float RespawnTime { get { return m_RespawnTime; } set { m_RespawnTime = value; } }
    [SerializeField] private string m_DefaultRespawnText = "Respawn in ";
    public void AddRespawnObject(GameObject obj, float time = default, DisplayBehaviour display = null, string text = default, GameObject camera = null, UnityEvent respawnEvent = null)
    {
        if(time == default) { time = m_RespawnTime; }
        if(display != null && text == default) { text = m_DefaultRespawnText; }
        StartCoroutine(RespawnCoroutine(obj, time, display, text, camera, respawnEvent));
    }
    private void Start()
    {
        Singleton = this;
    }
    private void ResetObject(GameObject target)
    {
        target.SetActive(true);
    }
    private void SetCamera(GameObject camera, bool state)
    {
        if (camera == null) return;
        camera.gameObject.SetActive(state);
    }
    private IEnumerator RespawnCoroutine(GameObject obj, float duration, DisplayBehaviour display = null, string text = null, GameObject camera = null, UnityEvent respawnEvent = null)
    {
        SetCamera(camera, true);
        yield return StartCoroutine(RunTimer(duration, display, text, Color.red));
        if (respawnEvent != null)
        {
            respawnEvent.Invoke();
        }
        if(obj.layer == LayerMask.NameToLayer("Player"))
        {
            yield return new WaitForSeconds(2.5f);
        }
        ResetObject(obj);
        SetCamera(camera, false);
    }
    private IEnumerator StartInvincibleTimer(GameObject target)
    {
        HealthBehaviour healthBehaviour = target.GetComponent<HealthBehaviour>();
        if(healthBehaviour != null)
        {
            healthBehaviour.DeezNuts(true);
        }
        yield return new WaitForSeconds(invincibleTimerOnRespawn);
        if(healthBehaviour != null)
        {
            healthBehaviour.DeezNuts(false);
        }
    }

}
