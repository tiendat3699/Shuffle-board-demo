using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform rotateController;
    [SerializeField] Slider sliderCurveForce;
    private Transform discTransform;
    private Disc disc;
    private bool dragging;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 dirThrow, dirMove;
    private float stepCurveForce = 0.1f;
    public float transX;

    private void Awake() {
        lineRenderer.enabled = false;
        rotateController.gameObject.SetActive(false);
        sliderCurveForce.gameObject.SetActive(false);
    }
    
    private void OnEnable() {
        RoundManager.Instance.OnDiscSpawm += (disc) => {
            discTransform = disc;
            dirThrow = discTransform.forward; 
            this.disc = disc.GetComponent<Disc>();
            rotateController.gameObject.SetActive(true);
        };

        rotateController.GetComponent<UIDrag>().OnDragging += RotateDisc;
    }

    private void LateUpdate() {
        rotateController.position = mainCamera.WorldToScreenPoint(discTransform.position + Vector3.down * 2f); 
    }

    private void OnClick(InputValue value) {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            if(hit.collider.TryGetComponent(out Disc disc)) {
                rotateController.gameObject.SetActive(false);
                sliderCurveForce.gameObject.SetActive(true);
                if(disc.notThrowable) return;
                dragging = true;
                startPos = hit.collider.transform.position;
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);
                lineRenderer.enabled = true;
            }
        }
    }

    private void OnMove(InputValue value) {
        dirMove = value.Get<Vector2>();
        dirMove.y = 0;
    }

    private void OnRelease(InputValue value) {
        if(dragging) {
            dragging = false;
            lineRenderer.enabled = false;
            float distance = Vector3.Distance(startPos, endPos);
            disc.owner = RoundManager.Instance.currentPlayer;
            disc.Throw(dirThrow.normalized, distance, sliderCurveForce.value);
            sliderCurveForce.gameObject.SetActive(false);
            sliderCurveForce.value = 0;
            transX = 0;
        }
    }

    private void RotateDisc(PointerEventData eventData) {
        float rotX = eventData.delta.normalized.x;
        Vector3 newDir = Quaternion.AngleAxis(-rotX * 1.5f, Vector3.up) * dirThrow;
        float diffAgnle = Vector3.Angle(newDir, discTransform.forward);
        if(diffAgnle >= -45 && diffAgnle <= 45) {
            disc.Rotate(newDir);
            dirThrow = newDir;
        }
    }

    private void FixedUpdate() {

        transX += dirMove.x  * Time.fixedDeltaTime;
        if(transX > 2) transX = 2;
        if(transX < -2) transX = -2;
        if(transX > -2f && transX < 2) {
            disc.Move(dirMove * Time.fixedDeltaTime);
        }

        if(dragging) {

            sliderCurveForce.value += stepCurveForce;
            if(sliderCurveForce.value >= 3) stepCurveForce = -0.1f;
            else if( sliderCurveForce.value <= -3) stepCurveForce = 0.1f;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                endPos = hit.point;
                endPos.y = startPos.y;
                lineRenderer.SetPosition(1, endPos);
            }
        }
    }

    private void OnDrawGizmos() {
        if(discTransform != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(discTransform.position,  discTransform.position + dirThrow * 3f);
        }
    }
}
