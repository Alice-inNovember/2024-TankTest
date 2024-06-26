using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Network;
using UI;
using UnityEngine;
using Util.EventSystem;
using Util.SingletonSystem;
using EventType = Util.EventSystem.EventType;

public enum EGameState
{
	PreLogin,
	Lobby,
	MatchMaking,
	PreGame,
	InGame,
	PostGame,
}

public class GameManager : MonoBehaviourSingleton<GameManager>, IEventListener
{
	public EGameState State { get; private set; }
	private CancellationTokenSource _matchTimeCountCancelToken;
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject enemy;
	
	public void OnEvent(EventType eventType, Component sender, object param = null)
	{
		switch (eventType)
		{
			case EventType.ProgramStart:
				break;
			case EventType.ServerConnection:
				if (param != null)
					ServerConnectionAction((EConnectResult)param);
				break;
			case EventType.GameStart:
				break;
			case EventType.PlayerTileClicked:
				break;
			case EventType.EnemyAction:
				break;
			case EventType.TileCommand:
				break;
			case EventType.BlockCommand:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
		}
	}

	private void Start()
	{
		EventManager.Instance.AddListener(EventType.ServerConnection, this);
		Application.runInBackground = true;
		State = EGameState.PreLogin;
	}
	public async void StartMachMaking()
	{
		if (State != EGameState.Lobby)
			return;
		State = EGameState.MatchMaking;
		MatchMakingTimeCount();
		await NetworkManager.Instance.Send(new Message(EMessageType.MT_MATCHQ_JOIN, ""));
	}
	public void MatchFound(string matchInfo)
	{
		var arg = matchInfo.Split('|');
		var playerType = arg[0].Trim(' ');
		var enemyName = arg[1].Trim(' ');
		_matchTimeCountCancelToken?.Cancel();
		UIManager.Instance.SetUI(EuiState.InGame);
		Debug.Log("My type : " + playerType + " | EnemyName : " + enemyName);
		State = EGameState.PreGame;
		EventManager.Instance.PostNotification(EventType.GameStart, this);
		State = EGameState.InGame;
	}
	private void MatchStop()
	{
		_matchTimeCountCancelToken?.Cancel();
		State = EGameState.Lobby;
	}
	public void GameOver(string arg)
	{
		State = EGameState.PostGame;
	}
	private async void MatchMakingTimeCount()
	{
		Debug.Log("MatchMakingTimeCount");
		_matchTimeCountCancelToken?.Cancel();
		_matchTimeCountCancelToken = new CancellationTokenSource();
		var startTime = Time.time;
		try
		{
			while (true)
			{
				_matchTimeCountCancelToken.Token.ThrowIfCancellationRequested();
				await NetworkManager.Instance.Send(new Message(EMessageType.MT_ACTIVE_USER, ""));
				UIManager.Instance.SetMatchMakingText((int)(Time.time - startTime));
				await Task.Delay(1000);
			}
		}
		catch (OperationCanceledException)
		{
			UIManager.Instance.SetMatchMakingText(00);
		}
		finally
		{
			_matchTimeCountCancelToken.Dispose();
			_matchTimeCountCancelToken = null;
		}
	}
	
	private void ServerConnectionAction(EConnectResult connectResult)
	{
		switch (connectResult)
		{
			case EConnectResult.Success:
				State = EGameState.Lobby;
				break;
			case EConnectResult.TimeOut:
				State = EGameState.PreLogin;
				break;
			case EConnectResult.Disconnect:
				State = EGameState.PreLogin;
				MatchStop();
				break;
			case EConnectResult.Error:
				State = EGameState.PreLogin;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(connectResult), connectResult, null);
		}
	}
}