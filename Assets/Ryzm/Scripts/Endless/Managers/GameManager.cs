﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class GameManager : MonoBehaviour
    {
        public GameStatus status = GameStatus.MainMenu;
        public float speed = 0.15f;
        IEnumerator lerpGameSpeed;
        IEnumerator deactivateHomeIsland;
        IEnumerator restart2Starting;

        void Awake()
        {
            Message.AddListener<GameStatusRequest>(OnGameStatusRequest);
            Message.AddListener<RunnerDie>(OnRunnerDie);
            Message.AddListener<RequestGameSpeedChange>(OnRequestGameSpeedChange);
            Message.AddListener<GameSpeedRequest>(OnGameSpeedRequest);
            Message.AddListener<MadeWorld>(OnMadeWorld);
            Message.AddListener<StartGame>(OnStartGame);
            Message.AddListener<PauseGame>(OnPauseGame);
            Message.AddListener<ResumeGame>(OnResumeGame);
            Message.AddListener<RestartGame>(OnRestartGame);
            UpdateGameStatus(GameStatus.MainMenu);
        }

        void OnDestroy()
        {
            Message.RemoveListener<GameStatusRequest>(OnGameStatusRequest);
            Message.RemoveListener<RunnerDie>(OnRunnerDie);
            Message.RemoveListener<RequestGameSpeedChange>(OnRequestGameSpeedChange);
            Message.RemoveListener<GameSpeedRequest>(OnGameSpeedRequest);
            Message.RemoveListener<MadeWorld>(OnMadeWorld);
            Message.RemoveListener<StartGame>(OnStartGame);
            Message.RemoveListener<PauseGame>(OnPauseGame);
            Message.RemoveListener<ResumeGame>(OnResumeGame);
            Message.RemoveListener<RestartGame>(OnRestartGame);
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                Time.timeScale = Time.timeScale > 0 ? 0 : 1;
            }
        }

        void OnMadeWorld(MadeWorld madeWorld)
        {
            OnStartingGame();
        }

        void OnStartingGame()
        {
            UpdateGameStatus(GameStatus.Transition);
            Message.Send(new CreateSectionRow());
            Debug.Log("CreateSectionRow");
            UpdateGameStatus(GameStatus.Starting);
        }

        void OnStartGame(StartGame start)
        {
            UpdateGameStatus(GameStatus.Active);
            deactivateHomeIsland = null;
            deactivateHomeIsland = DeactivateHomeIsland();
            StartCoroutine(deactivateHomeIsland);
        }

        void OnPauseGame(PauseGame pause)
        {
            UpdateGameStatus(GameStatus.Paused);
        }

        void OnResumeGame(ResumeGame resume)
        {
            UpdateGameStatus(GameStatus.Active);
        }

        void OnRunnerDie(RunnerDie runnerDie)
        {
            StopAllCoroutines();
            UpdateGameStatus(GameStatus.Ended);
        }

        void OnRestartGame(RestartGame restartGame)
        {
            UpdateGameStatus(GameStatus.Restart);
            restart2Starting = null;
            restart2Starting = Restart2Starting();
            StartCoroutine(restart2Starting);
        }

        void OnGameStatusRequest(GameStatusRequest statusRequest)
        {
            UpdateGameStatus(status);
        }

        void UpdateGameStatus(GameStatus status)
        {
            this.status = status;
            Message.Send(new GameStatusResponse(status));
        }

        IEnumerator DeactivateHomeIsland()
        {
            Debug.Log("deactivating home");
            yield return new WaitForSeconds(4);
            Message.Send(new DeactivateHome());
        }

        IEnumerator Restart2Starting()
        {
            yield return new WaitForSeconds(2);
            OnStartingGame();
        }

        void OnRequestGameSpeedChange(RequestGameSpeedChange requestChangeSpeed)
        {
            if(requestChangeSpeed.lerpTime <= 0)
            {
                UpdateSpeed(requestChangeSpeed.speed);
            }
            else
            {
                lerpGameSpeed = LerpGameSpeed(requestChangeSpeed.speed, requestChangeSpeed.lerpTime);
                StartCoroutine(lerpGameSpeed);
            }
        }

        IEnumerator LerpGameSpeed(float targetSpeed, float lerpTime)
        {
            float _time = 0;
            while(_time <= lerpTime)
            {
                UpdateSpeed(Mathf.Lerp(speed, targetSpeed, _time / lerpTime));
                _time += Time.deltaTime;
            }
            UpdateSpeed(targetSpeed);
            yield break;
        }

        void UpdateSpeed(float newSpeed)
        {
            this.speed = newSpeed;
            Message.Send(new GameSpeedResponse(newSpeed));
        }

        void OnGameSpeedRequest(GameSpeedRequest request)
        {
            UpdateSpeed(this.speed);
        }
    }

    public enum GameStatus
    {
        MainMenu,
        Starting,
        Active,
        Paused,
        Ended,
        Restart,
        Transition
    }
}
