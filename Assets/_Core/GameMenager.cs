using Game.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Core { 
    [NetworkSettings(channel =0,sendInterval =0.1f)]
    public class GameMenager : NetworkBehaviour {

        [SerializeField] int matchTimeInMinutes;
        [SerializeField] GoalGate firstTeamGoalGate;
        [SerializeField] GoalGate secondTeamGoalGate;
        [SerializeField] Rigidbody ball;

        GameUI UI;
        int firstTeamScore;
        int secondTeamScore;

        int currentMinutes;
        int currentSeconds;

	    // Use this for initialization
	    void Start () {

            UI = GetComponent<GameUI>();
            firstTeamGoalGate.notifyGoalScored += FirstTeamScored;
            secondTeamGoalGate.notifyGoalScored += SecondTeamScored;
            StartTime();
        }

        [Server]
        void StartTime(){
            StartCoroutine(MatchStart());
        }

        [Server]
        private IEnumerator MatchStart()
        {
            currentMinutes = matchTimeInMinutes - 1;
            currentSeconds = 59;
            UI.SetMatchTime(currentMinutes, currentSeconds);

            while (true)
            {
                UI.SetMatchTime(currentMinutes, currentSeconds);
                currentSeconds--;
                if(currentSeconds == 0){
                    currentSeconds = 59;
                    currentMinutes--;
                }

                if (currentMinutes < 0){
                    MatchEnd();
                    yield return null;
                }

                yield return new WaitForSeconds(1);
            }
        }

        [Server]
        private void MatchEnd(){

            int winner = 0;
            if (firstTeamScore > secondTeamScore) winner = 1;
            else if (secondTeamScore > firstTeamScore) winner = 2;
            RpcSetWinnerPanel(winner);
        }

        [ClientRpc]
        void RpcSetWinnerPanel(int winner)
        {
            UI.SetWinnerPanel(winner);
            StartCoroutine(BackToLobby());
        }

        private IEnumerator BackToLobby()
        {
            yield return new WaitForSeconds(2);
            var lobby = FindObjectOfType<NetworkLobbyManager>();
            lobby.ServerReturnToLobby();
        }

        [Server]
        private void SecondTeamScored()
        {
            secondTeamScore++;
            UI.SetTeamScore(secondTeamScore, 2);
            GoalScored();
        }
        [Server]
        private void FirstTeamScored()
        {
            firstTeamScore++;
            UI.SetTeamScore(firstTeamScore, 1);
            GoalScored();
        }
 
        [Server]
        private void GoalScored()
        {
            ball.transform.position = new Vector3(0, 7, 0);
            ball.velocity = Vector3.zero;
            ball.isKinematic = true;
            ball.isKinematic = false;
        }
    }
}