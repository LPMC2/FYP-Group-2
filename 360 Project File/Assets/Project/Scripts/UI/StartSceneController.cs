using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class StartSceneController : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup[] m_ManagedGroups;
    [SerializeField]
    private int m_InitialIndex;

    [SerializeField]
    private AnimationCurve m_SwitchAnimation = AnimationCurve.EaseInOut(0f, 0f, 0.5f, 1f);

    private int m_CurrentIndex;

    private void Awake()
    {
        foreach (var canvasGroup in m_ManagedGroups)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        m_ManagedGroups[m_InitialIndex].alpha = 1f;
        m_ManagedGroups[m_InitialIndex].blocksRaycasts = true;
        m_CurrentIndex = m_InitialIndex;
    }

    public void OpenURL(string url)
        => Application.OpenURL(url);

    public void Switch(int index)
    {
        Assert.IsTrue(0 <= index && index < m_ManagedGroups.Length);
        StartCoroutine(PerformSwitch(index));
    }

    private IEnumerator PerformSwitch(int index)
    {
        var oldGroup = m_ManagedGroups[m_CurrentIndex];
        var newGroup = m_ManagedGroups[index];

        var time = 0f;
        while (time < m_SwitchAnimation.GetLastKeyTime())
        {
            oldGroup.alpha = Mathf.Lerp(1f, 0f, m_SwitchAnimation.Evaluate(time));
            newGroup.alpha = Mathf.Lerp(0f, 1f, m_SwitchAnimation.Evaluate(time));
            time += Time.deltaTime;
            yield return null;
        }
        oldGroup.alpha = 0f;
        oldGroup.blocksRaycasts = false;
        newGroup.alpha = 1f;
        newGroup.blocksRaycasts = true;

        m_CurrentIndex = index;
    }
}
