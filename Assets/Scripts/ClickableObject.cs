using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonClick : MonoBehaviour, IPointerClickHandler
{
    public Button button; // Assign this in the Inspector

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the left mouse button was clicked
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Button clicked: " + button.name);
        }
    }

    private void OnButtonClick()
    {
        Debug.Log("Button clicked using Unity's Button component");
    }
}