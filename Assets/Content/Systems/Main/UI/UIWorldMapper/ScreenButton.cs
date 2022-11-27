using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ScreenButton : UIWorldMapper<Transform>
{
    protected Button button;

    public override void Init(Canvas targetCanvas, Transform reference)
    {
        base.Init(targetCanvas, reference);
        button = GetComponent<Button>();
    }

    public virtual void AddClickAction(Action action)
    {
        button.onClick.AddListener(() => { action.Invoke(); });
    }

    public override void Refresh()
    {
        base.Refresh();

        //TODO: make it an event and dont load main thread with this bullshit
        //gameObject.SetActive(ReferenceObject.gameObject.activeInHierarchy);
    }

    protected override Vector3 GetMapTarget()
    {
        return ReferenceObject.position;
    }
}
