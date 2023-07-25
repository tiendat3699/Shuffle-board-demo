using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DropArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out Disc disc)) {
            disc.DropOut = true;
            disc.Score = 0;
        }
    }
}
