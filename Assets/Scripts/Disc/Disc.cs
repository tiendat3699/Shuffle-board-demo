using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
public class Disc : MonoBehaviour
{
    [SerializeField] private GameObject _arrow;
    [SerializeField] private Rigidbody _rigibody;
    [SerializeField] private MeshCollider _meshCollider;
    public bool Stop { get => _rigibody.velocity.magnitude < 0.01f || DropOut; }
    public bool DropOut;
    public bool NotThrowable { get; private set;}
    public PlayerType Owner;
    public int Score;
    private float _curveValue;
    private float _curveForce;
    private Vector3 _bound;


    private void FixedUpdate() {
        AddCurveForce();
    }

    private void AddCurveForce() {
        if(_curveValue == 0) return;
        //check if curve force is zero we wont call addForce for optimize physic logic
        if(_curveForce > 0) {
            //make simple simulate drag for curve force
            _curveForce = _curveForce > 0 ? _curveForce - _rigibody.drag : 0;
            Vector3 dirForce = _curveValue > 0 ? Vector3.right : Vector3.left;
            _bound = _meshCollider.ClosestPointOnBounds(transform.position + dirForce);
            _rigibody.AddForceAtPosition(dirForce * _curveForce, _bound);
        }
    }

    public void Throw(Vector3 direction, float force, float curveValue)
    {
        NotThrowable = true;
        _arrow.SetActive(false);
        force = Mathf.Clamp(force, 3, 10);
        RoundManager.Instance.StartThrow();
        _rigibody.AddForce(direction * force * 2.5f , ForceMode.VelocityChange);
        _curveValue = curveValue;
        _curveForce = Mathf.Abs(curveValue);
    }

    public void Move(Vector2 moveMoment) {
        if(!NotThrowable) {
            transform.Translate(moveMoment * 2f);
        }
    }

    public void Rotate(Vector3 dirLook) {
        transform.rotation = Quaternion.LookRotation(dirLook);
    }
}
