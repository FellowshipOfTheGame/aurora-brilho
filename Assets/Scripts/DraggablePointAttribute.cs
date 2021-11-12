using UnityEngine;

public class DraggablePointAttribute : PropertyAttribute
{
    public bool local;

    public DraggablePointAttribute(bool local = false)
    {
        this.local = local;
    }
}