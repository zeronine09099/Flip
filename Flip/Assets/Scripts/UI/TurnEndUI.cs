using UnityEngine;
using UnityEngine.EventSystems;

public class TurnEndUI : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        Debug.Log("Flip!");
        Events.CallFliped();
    }
}
