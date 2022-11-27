using UnityEngine;


public abstract class UIWorldMapper<T> : MonoBehaviour where T : Component
{
    //[field: SerializeField]
    public T ReferenceObject { get; private set; }

    private Camera _targetCamera;
    private RectTransform _uiPointWorld;
    private float _planeDistance;
    [SerializeField]
    protected Vector3 _offset;
    private bool _camOrth;
    private Canvas _canvas;
    private Vector3 lastTarget = Vector3.zero;
    private Transform refTransform;
    public virtual void Init(Canvas targetCanvas, T reference)
    {
        ReferenceObject = reference;
        _targetCamera = Camera.main;
        _uiPointWorld = GetComponent<RectTransform>();
        _planeDistance = targetCanvas.planeDistance;
        _canvas = targetCanvas;
        refTransform = ReferenceObject.transform;

        if (_targetCamera.orthographic)
            _camOrth = true;
    }

    //public abstract void Init();

    public virtual void Refresh()
    {
        Vector3 curTarget = GetMapTarget();
        if ((curTarget - lastTarget).sqrMagnitude < 0.01f)
            return;

        Vector3 targetPosition = curTarget + refTransform.right * _offset.x + refTransform.up * _offset.y + refTransform.forward * _offset.z;

        if (_targetCamera.WorldToScreenPoint(targetPosition).z < 0)
            return;

        _uiPointWorld.localPosition = _canvas.WorldToCanvasPosition(targetPosition, _targetCamera);
        lastTarget = targetPosition;
        /*

                Vector2 screenPos = _targetCamera.WorldToScreenPoint(targetPosition).normalized * 100;


                Vector3 canvasPos = _targetCamera.ScreenToWorldPoint((Vector3)screenPos + new Vector3(0, 0, _planeDistance));

                if (!_camOrth)
                    _uiPointWorld.localPosition = canvasTransform.InverseTransformPoint(canvasPos);
                else
                    _uiPointWorld.localPosition = canvasTransform.InverseTransformPoint(canvasPos) / _targetCamera.orthographicSize;

                Vector3 localPosition = _uiPointWorld.localPosition;
                localPosition.z = 0;

                _uiPointWorld.localPosition = localPosition;*/
    }

    protected abstract Vector3 GetMapTarget();

}
