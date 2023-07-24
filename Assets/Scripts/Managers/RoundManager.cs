using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoundManager : Singleton<RoundManager>
{
    [SerializeField] private int maxTurn;
    [SerializeField] private List<PlayerBase> players = new List<PlayerBase>();
    [SerializeField] private Vector3 spawnPosition;
    private List<Disc> discList = new List<Disc>();
    public int currentPlayer {get; private set;}
    private int currentTurn;
    public event Action<Transform> OnDiscSpawm;
    // params (playerIndex, totalScore, newScorePlus)
    public event Action<int, int, int> OnScoreUpdate;

    private void StartTurn() {
        Disc disc = Instantiate(players[currentPlayer].discPrefabs, spawnPosition, Quaternion.identity);
        OnDiscSpawm?.Invoke(disc.transform);
        discList.Add(disc);
    }

    public void StartThrow() {
        StartCoroutine(CheckDiscMoving());
    }

    private void Start() {
        currentTurn = 1;
        StartTurn();
    }

    private IEnumerator CheckDiscMoving() {
        yield return new WaitForSeconds(0.1f);

        int distStopCount;
        int[] scores;

        //check all disc moving
        do
        {
            scores = new int[players.Count];
            distStopCount = 0;
            for(int i = 0; i < discList.Count; i++) {
                // check and add new score;
                scores[discList[i].owner] += discList[i].score;
                //count dist not moving
                distStopCount += discList[i].stop ? 1 : -1;
            }

            yield return new WaitForEndOfFrame();
            
        } while (distStopCount != discList.Count);

        //update player score when all disc not moving
        for(int i = 0; i < players.Count; i++) {
            int diffScore = scores[i] - players[i].score;
            players[i].score = scores[i];
            OnScoreUpdate?.Invoke(i, players[i].score, diffScore);
        }

        currentPlayer++;
        if(currentPlayer >= players.Count) {
            currentPlayer = 0;
            currentTurn++;
        }
        if(currentTurn <= maxTurn) {
            StartTurn();
        } else {
            if(players[0].score > players[1].score) {
                Debug.Log("Player 1 Win!!!");
            } else if(players[0].score < players[1].score) {
                Debug.Log("Player 2 Win!!!");
            } else {
                Debug.Log("Draw!!!");
            }
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Handles.color = new Color32(0, 255, 0, 150);
        Handles.DrawSolidDisc(spawnPosition, Vector3.up, 0.3f);
    }
#endif
}