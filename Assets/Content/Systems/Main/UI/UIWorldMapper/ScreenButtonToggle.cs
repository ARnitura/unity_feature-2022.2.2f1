using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenButtonToggle : ScreenButton
{
    public bool State { get; private set; } = false;

    [SerializeField]
    private Image imageGraphic;

    [SerializeField]
    private Color inactiveColor, activeColor;

    public override void Init(Canvas targetCanvas, Transform reference)
    {
        base.Init(targetCanvas, reference);
        imageGraphic.color = State ? activeColor : inactiveColor;
    }



    public virtual void AddClickAction(Action onAction, Action offAction)
    {
        button.onClick.AddListener(() =>
        {

            State = !State;
            if (State)
            {
                imageGraphic.color = activeColor;
                onAction.Invoke();
            }
            else
            {
                imageGraphic.color = inactiveColor;
                offAction.Invoke();
            }
        });
    }
}
