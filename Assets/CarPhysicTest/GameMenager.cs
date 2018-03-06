using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenager : MonoBehaviour {

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

        StartCoroutine(MatchStart());
    }

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

    private void MatchEnd()
    {
        Time.timeScale = 0;

        int winner = 0;
        if (firstTeamScore > secondTeamScore) winner = 1;
        else if (secondTeamScore > firstTeamScore) winner = 2;

        UI.SetWinnerPanel(winner);
    }

    private void SecondTeamScored()
    {
        secondTeamScore++;
        UI.SetTeamScore(secondTeamScore, 2);
        GoalScored();
    }

    private void FirstTeamScored()
    {
        firstTeamScore++;
        UI.SetTeamScore(firstTeamScore, 1);
        GoalScored();
    }

    private void GoalScored()
    {
        ball.transform.position = new Vector3(0, 3, 0);
        ball.velocity = Vector3.zero;
        ball.isKinematic = true;
        ball.isKinematic = false;
    }
}
