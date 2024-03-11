using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        None = 0,
        Setup = 1,
        PlayerOneTurn = 2,
        PlayerTwoTurn = 3,
        EndOfMatch = 4,
    }

    [SerializeField] private GameState currentState = GameState.None;

    public event Action OnSetupEnterEvent;
    public event Action<bool> OnPlayerOneEnterEvent;
    public event Action<bool> OnPlayerTwoEnterEvent;
    public event Action OnEndOfMatchEnterEvent;

    [Header("Visual Refs")]
    [SerializeField] private Animator turnIndicatorAnim;

    private void Start()
    {
        EntitiesController.Instance.OnAllEntitiesDieEvent += AllEntitiesOfPlayerDie;
        CommandManager.Instance.OnQueueEndEvent += EndOfTurn;

        //TODO: Implementar sistema de turnos direito - isso é só pra testar
        ChangeState(GameState.PlayerOneTurn);
    }

    //private void OnDisable()
    //{
    //    EntitiesController.Instance.OnAllEntitiesDieEvent -= AllEntitiesOfPlayerDie;
    //}

    public void ChangeState(GameState nextState)
    {
        currentState = nextState;
        OnStateChanged(currentState);
    }

    private void OnStateChanged(GameState nexState)
    {
        switch (nexState)
        {
            case GameState.Setup:
                OnSetupEnter();
                break;
            case GameState.PlayerOneTurn:
                OnPlayerOneTurnEnter();
                break;
            case GameState.PlayerTwoTurn:
                OnPlayerTwoTurnEnter();
                break;
            case GameState.EndOfMatch:
                OnEndOfMatchEnter();
                break;

            default:
                break;

        }
    }

    public void EndOfTurn()
    {
        if(currentState == GameState.PlayerOneTurn)
        {
            ChangeState(GameState.PlayerTwoTurn);
            turnIndicatorAnim.SetTrigger("TurnPlayerTwo");
            return;
        }

        if (currentState == GameState.PlayerTwoTurn)
        {
            ChangeState(GameState.PlayerOneTurn);
            turnIndicatorAnim.SetTrigger("TurnPlayerOne");
            return;
        }
    }

    private void AllEntitiesOfPlayerDie()
    {
        ChangeState(GameState.EndOfMatch);
    }


    void OnSetupEnter()
    {
        OnSetupEnterEvent?.Invoke();
    }

    void OnPlayerOneTurnEnter()
    {
        OnPlayerOneEnterEvent?.Invoke(true);
        OnPlayerTwoEnterEvent?.Invoke(false);
    }

    void OnPlayerTwoTurnEnter()
    {
        OnPlayerTwoEnterEvent?.Invoke(true);
        OnPlayerOneEnterEvent?.Invoke(false);
    }

    void OnEndOfMatchEnter()
    {
        OnEndOfMatchEnterEvent?.Invoke();
    }
}
