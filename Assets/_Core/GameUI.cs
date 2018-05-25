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
        [SerializeField] Image matchEndImage;

        [Header("UI Scores Images")]
        [SerializeField] Sprite redScored;
        [SerializeField] Sprite blueScored;

        [Header("UI Game Images")]
        [SerializeField] Sprite redWin;
        [SerializeField] Sprite blueWin;
        [SerializeField] Sprite draw;

        void Start()
        {
            matchEndImage.gameObject.SetActive(false);
        }

        public void DisableText()
        {
            matchEndImage.gameObject.SetActive(false);
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
            matchEndImage.gameObject.SetActive(true);
            if (team == 1)
            {
                firstTeamScoreText.text = score.ToString();
                matchEndImage.sprite = blueScored;
            }
            else if (team == 2)
            {
                secondTeamScoreText.text = score.ToString();
                matchEndImage.sprite = redScored;
            }
        }

        public void SetWinnerPanel(int winnerTeam)
        {
            matchEndImage.gameObject.SetActive(true);

            if (winnerTeam == 1)
                matchEndImage.sprite = redWin;
            else if(winnerTeam == 2)
                matchEndImage.sprite = blueWin;
            else
                matchEndImage.sprite = draw;
        }
    }
}
