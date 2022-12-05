using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARWallAnchor : MonoBehaviour
{
    // Start is called before the first frame update

    private ARWallAnchor previousAnchor = null;
    private ARWallAnchor nextAnchor = null;

    public ARWallAnchor PreviousAnchor { get => previousAnchor; private set => previousAnchor = value; }
    public ARWallAnchor NextAnchor { get => nextAnchor; private set => nextAnchor = value; }



    public List<ARWallObject> ConnectedWalls { get; private set; } = new List<ARWallObject>();


    private void Awake()
    {
        GlobalState.StateChanged += GlobalState_StateChanged;
    }

    private void OnDestroy()
    {
        GlobalState.StateChanged -= GlobalState_StateChanged;
    }

    private void GlobalState_StateChanged(GlobalState.State obj)
    {
        gameObject.SetActive(obj == GlobalState.State.ARWallCreation || obj == GlobalState.State.ARWallEdit);
    }

    public void Init(Vector3 pos, ARWallAnchor previous, ARWallObject wallObject)
    {
        previousAnchor = previous;

        if (previous != null)
            previous.nextAnchor = this;

        transform.position = pos;

        if (wallObject != null)
        {
            ConnectedWalls.Add(wallObject);
            previous?.ConnectedWalls.Add(wallObject);
        }

        /*
        if(previous.ConnectedWalls.Count != 0)
        if (previous.ConnectedWalls[previous.ConnectedWalls.Count - 1] != null)
            ConnectedWalls.Add(previous.ConnectedWalls[previous.ConnectedWalls.Count - 1]);
        */

        wallObject?.CreateMesh(this, previous);
        //test wall logic
    }
}
