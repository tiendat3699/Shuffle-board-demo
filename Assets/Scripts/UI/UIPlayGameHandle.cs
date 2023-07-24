using UnityEngine;
using TMPro;

public class UIPlayGameHandle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI P1Score;
    [SerializeField] private TextMeshProUGUI P2Score;

    private void OnEnable() {
        RoundManager.Instance.OnScoreUpdate += (playerIndex, totalScore, diffScore) => {
            if(playerIndex == 0) {
                P1Score.text = totalScore.ToString();
            } else {
                P2Score.text = totalScore.ToString();
            }
        };
    }
}
