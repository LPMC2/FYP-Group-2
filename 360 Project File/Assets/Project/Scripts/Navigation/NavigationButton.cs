using UnityEngine;
using UnityEngine.UI;

public class NavigationButton : MonoBehaviour
{
    [SerializeField]
    private Image m_Sprite;

    public NavigationPoint NavPoint { get; set; }

    public void Interact()
        => NavPoint?.Navigate();
}
