using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    [Header("Time")]
    [SerializeField] Text matchTime;

    [Header("Scores")]
    [SerializeField] Text firstTeamScoreText;
    [SerializeField] Text secondTeamScoreText;
    [SerializeField] Text matchEndText;


    void Start()
    {
        matchEndText.gameObject.SetActive(false);
    }

    public void SetMatchTime(int minutes, int seconds)
    {
        matchTime.text = minutes.ToString() + " : " + seconds.ToString();
    }

    public void SetTeamScore(int score, int team)
    {
        if (team == 1) firstTeamScoreText.text = score.ToString();
        else if (team == 2) secondTeamScoreText.text = score.ToString();
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
