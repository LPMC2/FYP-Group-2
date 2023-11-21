using UnityEngine;

public class Backpack : CollapsiblePanel
{
    protected override RectTransform.Edge ExpandEdge => RectTransform.Edge.Bottom;
    protected override float ContentHeight => 264f;
}
