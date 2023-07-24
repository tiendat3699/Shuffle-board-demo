using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IDragHandler
{
    public event Action<PointerEventData> OnDragging;

    public void OnDrag(PointerEventData eventData)
    {
        OnDragging?.Invoke(eventData);
    }
}
