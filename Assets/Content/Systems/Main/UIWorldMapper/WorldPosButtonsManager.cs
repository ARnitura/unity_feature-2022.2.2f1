using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldPosButtonsManager : MonoBehaviour
{
    [SerializeField]
    private Transform buttonPrefab;

    [SerializeField]
    private Transform toggleButtonPrefab;

    [SerializeField]
    private Canvas targetCanvas;


    private readonly List<ScreenButton> screenButtons = new List<ScreenButton>();


    public void AddButton(Transform reference, Action onClickAction = null)
    {
        ScreenButton newButton = Instantiate(buttonPrefab, targetCanvas.transform).GetComponent<ScreenButton>();
        newButton.Init(targetCanvas, reference);
        newButton.AddClickAction(onClickAction);

        screenButtons.Add(newButton);
    }

    public void AddToggleButton(Transform reference, Action onAction = null, Action offAction = null)
    {
        ScreenButtonToggle newButton = Instantiate(toggleButtonPrefab, targetCanvas.transform).GetComponent<ScreenButtonToggle>();
        newButton.Init(targetCanvas, reference);
        newButton.AddClickAction(onAction, offAction);

        screenButtons.Add(newButton);
    }

    public void RemoveButton(Transform reference)
    {
        for (int i = 0; i < screenButtons.Count; i++)
        {
            if (screenButtons[i].ReferenceObject == reference)
            {
                Destroy(screenButtons[i].gameObject);
                screenButtons.RemoveAt(i);
                return;
            }
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        foreach (ScreenButton item in screenButtons)
        {
            item.Refresh();
        }
    }
}
