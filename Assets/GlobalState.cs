using FlutterUnityIntegration;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalState
{
    public enum State { None, Scan, ARObject, ARObjectPlacement, ARWallCreation, ARWallEdit }
    public static State CurrentState { get; private set; } = State.None;
    public static State PreviousState { get; private set; } = State.None;

    public static event Action<State> StateChanged;

    public static void SetState(State newState)
    {
        UnityMessageManager.Instance.SendMessageToFlutter($"@s {newState}");

        PreviousState = CurrentState;
        CurrentState = newState;
        StateChanged?.Invoke(newState);
    }
}
