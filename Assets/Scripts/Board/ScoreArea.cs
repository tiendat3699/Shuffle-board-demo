using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ScoreArea : MonoBehaviour
{
    [SerializeField] private int score;

    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out Disc throwableObj)) {
            throwableObj.score = score;
        }
    }
}
