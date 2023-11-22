using UnityEngine;

public class ExternalLinkHelper : MonoBehaviour
{
    public void OpenURL(string url)
        => Application.OpenURL(url);
}
