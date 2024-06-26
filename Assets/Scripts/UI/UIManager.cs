using System;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.EventSystem;
using Util.SingletonSystem;
using EventType = Util.EventSystem.EventType;

namespace UI
{
	public enum EuiState
	{
		Start,
		Login,
		Lobby,
		InGame,
		Result
	}
	public class UIManager : MonoBehaviourSingleton<UIManager>, IEventListener
	{
		[Header("UI Set")]
		[SerializeField] private GameObject menuUISet;
	
		[Header("Animation Button")]
		[SerializeField] private AnimationUI loginToStartB;
		[SerializeField] private AnimationUI startB;
		[SerializeField] private AnimationUI logoUI;
		[SerializeField] private AnimationUI creditUI;
		[SerializeField] private AnimationUI loginUI;
		[SerializeField] private AnimationUI matchMakingUI;
	
		[Header("Buttons")]
		[SerializeField] private Button toStartButton;
		[SerializeField] private Button startButton;
		[SerializeField] private Button loginButton;
		[SerializeField] private Button matchButton;
	
		[Header("Texts")]
		[SerializeField] private TMP_Text loginResultText;
		[SerializeField] private TMP_Text matchTimeText;
		[SerializeField] private TMP_Text activeUserText;
	
		[Header("InputFields")]
		[SerializeField] private TMP_InputField loginInput;

		public const float AnimationTime = 0.5f;

		private void Start()
		{
			EventManager.Instance.AddListener(EventType.ProgramStart, this);
			EventManager.Instance.AddListener(EventType.ServerConnection, this);
			toStartButton.onClick.AddListener(()=>SetUI(EuiState.Start));
			startButton.onClick.AddListener(()=> SetUI(EuiState.Login));
			loginButton.onClick.AddListener(LoginButton);
			matchButton.onClick.AddListener(()=>GameManager.Instance.StartMachMaking());
		}
		private void LoginButton()
		{
			if (loginInput.text.Length == 0)
			{
				loginResultText.text = "Enter Name And Try Again";
				return;
			}
			loginResultText.text = "Connecting...";
			NetworkManager.Instance.TryConnectServer(loginInput.text);
		}
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
				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}
		}
		public void SetActiveUserText(string number)
		{
			activeUserText.text = "ActiveUser : " + number;
		}
		public void SetMatchMakingText(int second)
		{
			matchTimeText.text = "MatchQ  " + (second / 60).ToString("00") + ":" + (second % 60).ToString("00");
		}
		private void ServerConnectionAction(EConnectResult result)
		{
			switch (result)
			{
				case EConnectResult.Success:
					SetUI(EuiState.Lobby);
					loginResultText.text = "Server Connect Success";
					break;
				case EConnectResult.TimeOut:
					SetUI(EuiState.Login);
					loginResultText.text = "Server TimeOut, Try Again";
					break;
				case EConnectResult.Disconnect:
					SetUI(EuiState.Login);
					loginResultText.text = "Server Disconnect, Try Again";
					break;
				case EConnectResult.Error:
					SetUI(EuiState.Login);
					loginResultText.text = "Server Error, Try Again";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(result), result, null);
			}
		}
		public void SetUI(EuiState state, object param = null)
		{
			switch (state)
			{
				case EuiState.Start:
					menuUISet.SetActive(true);
					loginToStartB.Hide();
					startB.Show();
					logoUI.Show();
					creditUI.Show();
					loginUI.Hide();
					matchMakingUI.Hide();
					break;
				case EuiState.Login:
					menuUISet.SetActive(true);
					loginToStartB.Show();
					startB.Hide();
					logoUI.Hide();
					creditUI.Hide();
					loginUI.Show();
					matchMakingUI.Hide();
					break;
				case EuiState.Lobby:
					SetActiveUserText("--");
					menuUISet.SetActive(true);
					loginToStartB.Hide();
					startB.Hide();
					logoUI.Hide();
					creditUI.Hide();
					loginUI.Hide();
					matchMakingUI.Show();
					break;
				case EuiState.InGame:
					menuUISet.SetActive(false);
					break;
				case EuiState.Result:
					menuUISet.SetActive(false);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}
	}
}