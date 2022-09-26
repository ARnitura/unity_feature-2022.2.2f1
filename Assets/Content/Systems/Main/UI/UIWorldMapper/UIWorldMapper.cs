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

    public virtual void Init(Canvas targetCanvas, T reference)
    {
        ReferenceObject = reference;
        _targetCamera = Camera.main;
        _uiPointWorld = GetComponent<RectTransform>();
        _planeDistance = targetCanvas.planeDistance;

        if (_targetCamera.orthographic)
            _camOrth = true;
    }

    //public abstract void Init();

    public virtual void Refresh()
    {

        Transform refTransform = ReferenceObject.transform;
        Vector3 targetPosition = GetMapTarget() + refTransform.right * _offset.x + refTransform.up * _offset.y + refTransform.forward * _offset.z;

        if (_targetCamera.WorldToScreenPoint(targetPosition).z < 0)
            return;

        Vector2 screenPos = _targetCamera.WorldToScreenPoint(targetPosition);


        Vector3 canvasPos = _targetCamera.ScreenToWorldPoint((Vector3)screenPos + new Vector3(0, 0, _planeDistance));

        if (!_camOrth)
            _uiPointWorld.localPosition = transform.parent.InverseTransformPoint(canvasPos);
        else
            _uiPointWorld.localPosition = transform.parent.InverseTransformPoint(canvasPos) / _targetCamera.orthographicSize;

        Vector3 localPosition = _uiPointWorld.localPosition;
        localPosition = new Vector3(localPosition.x, localPosition.y, 0);
        _uiPointWorld.localPosition = localPosition;
    }

    protected abstract Vector3 GetMapTarget();

}
