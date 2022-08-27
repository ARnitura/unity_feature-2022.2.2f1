using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RulerButton : MonoBehaviour
{
    [SerializeField]
    private ARObjectDecorator decorator;

    [SerializeField]
    private Image rulerImage;

    [SerializeField]
    private Color inactiveColor, activeColor;

    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { OnClick(); });
    }

    private void OnClick()
    {
        decorator.SwitchRuler();
        if (decorator.RulerEnabled)
            rulerImage.color = activeColor;
        else
            rulerImage.color = inactiveColor;
    }
}
