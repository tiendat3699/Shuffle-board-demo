using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PlayerType{
    P1,
    P2,
}

public class RoundManager : Singleton<RoundManager>
{
    [SerializeField] private int _maxTurn;
    [SerializeField] private PlayerBase _player1;
    [SerializeField] private PlayerBase _player2;
    [SerializeField] private Vector3 _spawnPosition;
    private List<Disc> _discList = new List<Disc>();
    public PlayerType CurrentPlayer {get; private set;}
    private int _currentTurn;
    public event Action<Transform> OnDiscSpawm;

    // params (P1Score, P2Score)
    public event Action<int, int> OnScoreUpdate;

    private void StartTurn() {
        Disc discPrefab = CurrentPlayer == PlayerType.P1 ? _player1.DiscPrefabs : _player2.DiscPrefabs;
        Disc disc = Instantiate(discPrefab, _spawnPosition, Quaternion.identity);
        OnDiscSpawm?.Invoke(disc.transform);
        _discList.Add(disc);
    }

    public async void StartThrow() {
        await CheckDiscMoving();
    }

    private void Start() {
        _currentTurn = 1;
        StartTurn();
    }


    private async UniTask CheckDiscMoving() {
        //wait for first disc spawn;
        await UniTask.WaitForSeconds(0.1f);

        int distStopCount;
        int p1NewScores;
        int p2NewScores;

        //check all disc moving
        do
        {
            p1NewScores = 0; 
            p2NewScores = 0;
            distStopCount = 0;
            for(int i = 0; i < _discList.Count; i++) {
                // check and add new score;
                if(_discList[i].Owner == PlayerType.P1) {
                    p1NewScores += _discList[i].Score;
                } else 
                {
                    p2NewScores += _discList[i].Score;
                }
                //count disc not moving
                distStopCount += _discList[i].Stop ? 1 : -1;
            }

            await UniTask.WaitForSeconds(0.1f);
            
        } while (distStopCount != _discList.Count);

        //calc different score for UI
        int diffScore1 = _player1.Score = p1NewScores;
        int diffScore2 = _player2.Score = p2NewScores;

        // update player score when all disc not moving
        _player1.Score = p1NewScores;
        _player2.Score = p2NewScores;
        OnScoreUpdate?.Invoke(_player1.Score, _player2.Score);

        CurrentPlayer ++;
        if((int)CurrentPlayer >= 2) {
            CurrentPlayer = 0;
            _currentTurn++;
        }
        if(_currentTurn <= _maxTurn) {
            StartTurn();
        } else {
            if(_player1.Score > _player2.Score) {
                Debug.Log("Player 1 Win!!!");
            } else if(_player1.Score < _player2.Score) {
                Debug.Log("Player 2 Win!!!");
            } else {
                Debug.Log("Draw!!!");
            }
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Handles.color = new Color32(0, 255, 0, 150);
        Handles.DrawSolidDisc(_spawnPosition, Vector3.up, 0.3f);
    }
#endif
}