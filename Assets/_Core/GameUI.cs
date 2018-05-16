using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Game.UI { 
    public class GameUI : NetworkBehaviour {

        [Header("Time")]
        [SerializeField] Text matchTime;

        [Header("Scores")]
        [SerializeField] Text firstTeamScoreText;
        [SerializeField] Text secondTeamScoreText;
        [SerializeField] Text matchEndText;

        [Header("Nav")]
        [SerializeField] Image ballArrow;

        void Start()
        {
            matchEndText.gameObject.SetActive(false);
        }

        public void DisableText()
        {
            matchEndText.gameObject.SetActive(false);
        }

        public void SetMatchTime(int minutes, int seconds)
        {
            matchTime.text = minutes.ToString() + " : " + seconds.ToString();
            RpcSetTime(minutes, seconds);
        }
        [ClientRpc]
        void RpcSetTime(int minutes, int seconds)
        {
            matchTime.text = minutes.ToString() + " : " + seconds.ToString();
        }

        public void SetTeamScore(int score, int team)
        {
            SetScoreForTeam(score, team);

            RpcSetTeamScore(score, team);
        }

        [ClientRpc]
        void RpcSetTeamScore(int score, int team){
            SetScoreForTeam(score, team);
        }

        private void SetScoreForTeam(int score, int team)
        {
            matchEndText.gameObject.SetActive(true);
            if (team == 1)
            {
                firstTeamScoreText.text = score.ToString();
                matchEndText.color = Color.red;
                matchEndText.text = "RED TEAM SCORED";
            }
            else if (team == 2)
            {
                secondTeamScoreText.text = score.ToString();
                matchEndText.color = Color.blue;
                matchEndText.text = "BLUE TEAM SCORED";
            }
        }

        public void SetWinnerPanel(int winnerTeam)
        {
            matchEndText.gameObject.SetActive(true);

            if (winnerTeam == 1)
            {
                matchEndText.color = Color.blue;
                matchEndText.text = "BLUE TEAM WIN";
            }
            else if(winnerTeam == 2)
            {
                matchEndText.color = Color.red;
                matchEndText.text = "RED TEAM WIN";
            }
            else
            {
                matchEndText.color = Color.white;
                matchEndText.text = "DRAW";
            }
        }
    }
}
