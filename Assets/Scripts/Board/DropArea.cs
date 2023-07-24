using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DropArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out Disc disc)) {
            disc.dropOut = true;
            disc.score = 0;
        }
    }
}
