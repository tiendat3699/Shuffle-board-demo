using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _rotateController;
    [SerializeField] private Slider _sliderCurveForce;
    private Transform _discTransform;
    private Disc _disc;
    private bool _dragging;
    private Vector3 _startPos;
    private Vector3 _endPos;
    private Vector3 _dirThrow, _dirMove;
    private float _stepCurveForce = 0.1f;
    private float _transX;

    private void Awake() {
        _lineRenderer.enabled = false;
        _rotateController.gameObject.SetActive(false);
        _sliderCurveForce.gameObject.SetActive(false);
    }
    
    private void OnEnable() {
        RoundManager.Instance.OnDiscSpawm += (disc) => {
            _discTransform = disc;
            _dirThrow = _discTransform.forward; 
            _disc = disc.GetComponent<Disc>();
            _rotateController.gameObject.SetActive(true);
        };

        _rotateController.GetComponent<UIDrag>().OnDragging += RotateDisc;
    }

    private void FixedUpdate() {
        HandleMoveDisc();
        HandleControlForce();
    }

    private void LateUpdate()
    {
        //make _rotateController follow disc
        _rotateController.position = _mainCamera.WorldToScreenPoint(_discTransform.position + Vector3.down * 2f); 
    }

    private void OnClick(InputValue value) {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            if(hit.collider.TryGetComponent(out Disc disc)) {
                _rotateController.gameObject.SetActive(false);
                _sliderCurveForce.gameObject.SetActive(true);
                if(disc.NotThrowable) return;
                _dragging = true;
                _startPos = hit.collider.transform.position;
                _lineRenderer.SetPosition(0, _startPos);
                _lineRenderer.SetPosition(1, _endPos);
                _lineRenderer.enabled = true;
            }
        }
    }

    private void OnMove(InputValue value) {
        _dirMove = value.Get<Vector2>();
        _dirMove.y = 0;
    }

    private void OnRelease(InputValue value) {
        if(_dragging) {
            _dragging = false;
            _lineRenderer.enabled = false;
            float distance = Vector3.Distance(_startPos, _endPos);

            if(distance > 2f) {
                _disc.Throw(_dirThrow.normalized, distance, _sliderCurveForce.value);
            } else {
                _rotateController.gameObject.SetActive(false);
            }

            _sliderCurveForce.gameObject.SetActive(false);
            _sliderCurveForce.value = 0;
            _transX = 0;
        }
    }

    private void RotateDisc(PointerEventData eventData) {
        float rotX = eventData.delta.normalized.x;
        Vector3 newDir = Quaternion.AngleAxis(-rotX * 1.8f, Vector3.up) * _dirThrow;
        float diffAgnle = Vector3.Angle(newDir, Vector3.forward);
        if(diffAgnle >= -45 && diffAgnle <= 45) {
            _disc.Rotate(newDir);
            _dirThrow = newDir;
        }
    }

    private void HandleMoveDisc() {
        //limit translateX for disc
        _transX += _dirMove.x  * Time.fixedDeltaTime;
        if(_transX > 2) _transX = 2;
        if(_transX < -2) _transX = -2;
        if(_transX > -2f && _transX < 2) {
            _disc.Move(_dirMove);
        }
    }

    private void HandleControlForce() {
        
        if(_dragging) {

            _sliderCurveForce.value += _stepCurveForce;
            if(_sliderCurveForce.value >= 3) _stepCurveForce = -0.1f;
            else if( _sliderCurveForce.value <= -3) _stepCurveForce = 0.1f;

            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                _endPos = hit.point;
                _endPos.y = _startPos.y;
                _lineRenderer.SetPosition(1, _endPos);
            }
        }
    }
}
