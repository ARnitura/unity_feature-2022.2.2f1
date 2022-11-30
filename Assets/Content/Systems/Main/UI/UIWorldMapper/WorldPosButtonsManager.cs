using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldPosButtonsManager : MonoBehaviour
{
    [SerializeField]
    private ScreenButton buttonPrefab;

    [SerializeField]
    private ScreenButtonToggle toggleButtonPrefab;

    [SerializeField]
    private Canvas targetCanvas;

    public static WorldPosButtonsManager Instance { get; private set; }


    private readonly List<ScreenButton> screenButtons = new List<ScreenButton>();

    private void Start()
    {
        GlobalState.StateChanged += GlobalState_StateChanged;
        Instance = this;
    }

    private void GlobalState_StateChanged(GlobalState.State obj)
    {
        if (obj == GlobalState.State.ARObject)
            foreach (var item in screenButtons)
                item.gameObject.SetActive(true);
        else
            foreach (var item in screenButtons)
                item.gameObject.SetActive(false);
    }




    public void AddButton(Transform reference, Action onClickAction = null)
    {
        ScreenButton newButton = Instantiate(buttonPrefab, targetCanvas.transform);
        newButton.Init(targetCanvas, reference);
        newButton.AddClickAction(onClickAction);

        screenButtons.Add(newButton);
    }

    public void AddToggleButton(Transform reference, Action onAction = null, Action offAction = null)
    {
        if (reference == null)
            throw new ArgumentNullException("Button's Transform reference can't be null");

        ScreenButtonToggle newButton = Instantiate(toggleButtonPrefab, targetCanvas.transform);
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
