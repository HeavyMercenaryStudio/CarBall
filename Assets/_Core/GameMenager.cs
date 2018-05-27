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

        public Car.CarController LocalCar;

        List<Car.CarController> cars = new List<Car.CarController>();

        GameUI UI;
        int firstTeamScore;
        int secondTeamScore;

        int currentMinutes;
        int currentSeconds;

	    void Start ()
        {
            UI = GetComponent<GameUI>();
            firstTeamGoalGate.notifyGoalScored += FirstTeamScored;
            secondTeamGoalGate.notifyGoalScored += SecondTeamScored;

            DisableMainMenu();

            FindLocalCar();

            StartTime();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                StartCoroutine(BackToLobby(0));
        }

        //[Server]
        //private void SetCarsStartPositions()
        //{
        //    var positions = FindObjectsOfType<NetworkStartPosition>();
        //    LocalCar.startPosition = positions[0].transform.position;

        //    for (int i = 1; i < cars.Count; i++)
        //    {
        //        var pos = positions[i];
        //        RpcSetStartPosition(pos.transform.position);
        //    }
        //}
        
        //[ClientRpc]
        //private void RpcSetStartPosition(Vector3 pos){
        //    LocalCar.startPosition = pos;
        //}

        private static void DisableMainMenu()
        {
            var LM = FindObjectOfType<Prototype.NetworkLobby.LobbyManager>();
            LM.mainMenuPanel.gameObject.SetActive(false);
            LM.logo.gameObject.SetActive(false);
        }

        void FindLocalCar()
        {
            var list = FindObjectsOfType<Game.Car.CarController>();
            for (int i = 0; i < list.Length; i++)
            {
                cars.Add(list[i]);
                if (list[i].isLocalPlayer)
                {
                    LocalCar = list[i];
                    break;
                }
            }
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
            StartCoroutine(BackToLobby(2));
        }

        private IEnumerator BackToLobby(int time)
        {
            yield return new WaitForSeconds(time);
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
            ball.transform.position = new Vector3(0, 9.43f, -2.95f);
            ball.velocity = Vector3.zero;
            ball.isKinematic = true;
            ball.isKinematic = false;
            StartCoroutine(PauseGame());
            RpcPauseGame();
        }

        [ClientRpc]
        void RpcPauseGame(){
            StartCoroutine(PauseGame());
        }

        IEnumerator PauseGame()
        {
            if (LocalCar == null)
                FindLocalCar();
            LocalCar.enabled = false;
            SoundManager.Instance.PlayWhistleSound();
            yield return new WaitForSeconds(2f);

            LocalCar.ResetPosition();
            LocalCar.enabled = true;
            UI.DisableText();

        }
    }
}