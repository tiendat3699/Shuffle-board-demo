using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
public class Disc : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshCollider meshCollider;
    public bool stop { get => rb.velocity.magnitude < 0.01f || dropOut; }
    public bool dropOut;
    public bool notThrowable { get; private set;}
    public int owner;
    public int score;
    private float curveValue;
    private float curveForce;
    private Vector3 bound;


    private void FixedUpdate() {
        if(curveValue == 0) return;
        if(curveForce > 0) {
            curveForce = curveForce > 0 ? curveForce - rb.drag : 0;
            Vector3 dirForce = curveValue > 0 ? Vector3.right : Vector3.left;
            bound = meshCollider.ClosestPointOnBounds(transform.position + dirForce);
            rb.AddForceAtPosition(dirForce * curveForce, bound);
        }
    }

    public void Throw(Vector3 direction, float force, float curveValue)
    {
        notThrowable = true;
        arrow.SetActive(false);
        force = Mathf.Clamp(force, 3, 10);
        RoundManager.Instance.StartThrow();
        rb.AddForce(direction * force * 2.5f , ForceMode.VelocityChange);
        this.curveValue = curveValue;
        curveForce = Mathf.Abs(curveValue);
    }

    public void Move(Vector2 moveMoment) {
        if(!notThrowable) {
            transform.Translate(moveMoment * 2f);
        }
    }

    public void Rotate(Vector3 dirLook) {
        transform.rotation = Quaternion.LookRotation(dirLook);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(bound, 0.2f);
    }
}
