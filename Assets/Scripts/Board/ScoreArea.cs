using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ScoreArea : MonoBehaviour
{
    [SerializeField] private int _score;

    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out Disc throwableObj)) {
            throwableObj.Score = _score;
        }
    }
}
