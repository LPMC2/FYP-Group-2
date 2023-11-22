using UnityEngine;

public class Backpack : CollapsiblePanel
{
    protected override RectTransform.Edge ExpandEdge => RectTransform.Edge.Bottom;

    protected override float ContentHeight => m_ContentRectTransform.GetChild(0).GetComponent<RectTransform>().rect.height; //I know this method is not efficient but the scene keeps updating and reverting the changes

    public RectTransform ContentTransform => m_ContentRectTransform;
}
