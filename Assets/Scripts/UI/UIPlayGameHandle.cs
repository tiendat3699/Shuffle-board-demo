using UnityEngine;
using TMPro;

public class UIPlayGameHandle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _p1Score;
    [SerializeField] private TextMeshProUGUI _p2Score;

    private void OnEnable() {
        RoundManager.Instance.OnScoreUpdate += (P1Score, P2Score) => {
            _p1Score.text = P1Score.ToString();
            _p2Score.text = P2Score.ToString();
        };
    }
}
