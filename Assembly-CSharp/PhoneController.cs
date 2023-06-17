using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020003A2 RID: 930
public class PhoneController : EntityComponent<global::BaseEntity>
{
	// Token: 0x040019A0 RID: 6560
	private PhoneController activeCallTo;

	// Token: 0x040019A1 RID: 6561
	public int PhoneNumber;

	// Token: 0x040019A2 RID: 6562
	public string PhoneName;

	// Token: 0x040019A3 RID: 6563
	public bool CanModifyPhoneName = true;

	// Token: 0x040019A4 RID: 6564
	public bool CanSaveNumbers = true;

	// Token: 0x040019A5 RID: 6565
	public bool RequirePower = true;

	// Token: 0x040019A6 RID: 6566
	public bool RequireParent;

	// Token: 0x040019A7 RID: 6567
	public float CallWaitingTime = 12f;

	// Token: 0x040019A8 RID: 6568
	public bool AppendGridToName;

	// Token: 0x040019A9 RID: 6569
	public bool IsMobile;

	// Token: 0x040019AA RID: 6570
	public bool CanSaveVoicemail;

	// Token: 0x040019AB RID: 6571
	public GameObjectRef PhoneDialog;

	// Token: 0x040019AC RID: 6572
	public VoiceProcessor VProcessor;

	// Token: 0x040019AD RID: 6573
	public PreloadedCassetteContent PreloadedContent;

	// Token: 0x040019AE RID: 6574
	public SoundDefinition DialToneSfx;

	// Token: 0x040019AF RID: 6575
	public SoundDefinition RingingSfx;

	// Token: 0x040019B0 RID: 6576
	public SoundDefinition ErrorSfx;

	// Token: 0x040019B1 RID: 6577
	public SoundDefinition CallIncomingWhileBusySfx;

	// Token: 0x040019B2 RID: 6578
	public SoundDefinition PickupHandsetSfx;

	// Token: 0x040019B3 RID: 6579
	public SoundDefinition PutDownHandsetSfx;

	// Token: 0x040019B4 RID: 6580
	public SoundDefinition FailedWrongNumber;

	// Token: 0x040019B5 RID: 6581
	public SoundDefinition FailedNoAnswer;

	// Token: 0x040019B6 RID: 6582
	public SoundDefinition FailedNetworkBusy;

	// Token: 0x040019B7 RID: 6583
	public SoundDefinition FailedEngaged;

	// Token: 0x040019B8 RID: 6584
	public SoundDefinition FailedRemoteHangUp;

	// Token: 0x040019B9 RID: 6585
	public SoundDefinition FailedSelfHangUp;

	// Token: 0x040019BA RID: 6586
	public Light RingingLight;

	// Token: 0x040019BB RID: 6587
	public float RingingLightFrequency = 0.4f;

	// Token: 0x040019BC RID: 6588
	public AudioSource answeringMachineSound;

	// Token: 0x040019BD RID: 6589
	public EntityRef currentPlayerRef;

	// Token: 0x040019C0 RID: 6592
	public List<ProtoBuf.VoicemailEntry> savedVoicemail;

	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x0600208F RID: 8335 RVA: 0x000D70AD File Offset: 0x000D52AD
	// (set) Token: 0x06002090 RID: 8336 RVA: 0x000D70B5 File Offset: 0x000D52B5
	public global::Telephone.CallState serverState { get; set; }

	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x06002091 RID: 8337 RVA: 0x000D70C0 File Offset: 0x000D52C0
	public uint AnsweringMessageId
	{
		get
		{
			global::Telephone telephone;
			if ((telephone = (base.baseEntity as global::Telephone)) == null)
			{
				return 0U;
			}
			return telephone.AnsweringMessageId;
		}
	}

	// Token: 0x06002092 RID: 8338 RVA: 0x000D70E4 File Offset: 0x000D52E4
	public void ServerInit()
	{
		if (this.PhoneNumber == 0 && !Rust.Application.isLoadingSave)
		{
			this.PhoneNumber = TelephoneManager.GetUnusedTelephoneNumber();
			if (this.AppendGridToName & !string.IsNullOrEmpty(this.PhoneName))
			{
				this.PhoneName = this.PhoneName + " " + PhoneController.PositionToGridCoord(base.transform.position);
			}
			TelephoneManager.RegisterTelephone(this, false);
		}
	}

	// Token: 0x06002093 RID: 8339 RVA: 0x000D714F File Offset: 0x000D534F
	public void PostServerLoad()
	{
		this.currentPlayer = null;
		base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		TelephoneManager.RegisterTelephone(this, false);
	}

	// Token: 0x06002094 RID: 8340 RVA: 0x000D7172 File Offset: 0x000D5372
	public void DoServerDestroy()
	{
		TelephoneManager.DeregisterTelephone(this);
	}

	// Token: 0x06002095 RID: 8341 RVA: 0x000D717A File Offset: 0x000D537A
	public void ClearCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.ClearCurrentUser();
	}

	// Token: 0x06002096 RID: 8342 RVA: 0x000D7182 File Offset: 0x000D5382
	public void ClearCurrentUser()
	{
		if (this.currentPlayer != null)
		{
			this.currentPlayer.SetActiveTelephone(null);
			this.currentPlayer = null;
		}
		base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06002097 RID: 8343 RVA: 0x000D71B8 File Offset: 0x000D53B8
	public void SetCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (this.currentPlayer == player)
		{
			return;
		}
		this.UpdateServerPlayer(player);
		if (this.serverState == global::Telephone.CallState.Dialing || this.serverState == global::Telephone.CallState.Ringing || this.serverState == global::Telephone.CallState.InProcess)
		{
			this.ServerHangUp(default(global::BaseEntity.RPCMessage));
		}
	}

	// Token: 0x06002098 RID: 8344 RVA: 0x000D720C File Offset: 0x000D540C
	private void UpdateServerPlayer(global::BasePlayer newPlayer)
	{
		if (this.currentPlayer == newPlayer)
		{
			return;
		}
		if (this.currentPlayer != null)
		{
			this.currentPlayer.SetActiveTelephone(null);
		}
		this.currentPlayer = newPlayer;
		base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, this.currentPlayer != null, false, true);
		if (this.currentPlayer != null)
		{
			this.currentPlayer.SetActiveTelephone(this);
		}
	}

	// Token: 0x06002099 RID: 8345 RVA: 0x000D7284 File Offset: 0x000D5484
	public void InitiateCall(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		int number = msg.read.Int32();
		this.CallPhone(number);
	}

	// Token: 0x0600209A RID: 8346 RVA: 0x000D72B8 File Offset: 0x000D54B8
	public void CallPhone(int number)
	{
		if (number == this.PhoneNumber)
		{
			this.OnDialFailed(global::Telephone.DialFailReason.CallSelf);
			return;
		}
		if (TelephoneManager.GetCurrentActiveCalls() + 1 > TelephoneManager.MaxConcurrentCalls)
		{
			this.OnDialFailed(global::Telephone.DialFailReason.NetworkBusy);
			return;
		}
		PhoneController telephone = TelephoneManager.GetTelephone(number);
		if (!(telephone != null))
		{
			this.OnDialFailed(global::Telephone.DialFailReason.WrongNumber);
			return;
		}
		if (telephone.serverState == global::Telephone.CallState.Idle && telephone.CanReceiveCall())
		{
			this.SetPhoneState(global::Telephone.CallState.Dialing);
			this.lastDialedNumber = number;
			this.activeCallTo = telephone;
			this.activeCallTo.ReceiveCallFrom(this);
			return;
		}
		this.OnDialFailed(global::Telephone.DialFailReason.Engaged);
		telephone.OnIncomingCallWhileBusy();
	}

	// Token: 0x0600209B RID: 8347 RVA: 0x000D7343 File Offset: 0x000D5543
	private bool CanReceiveCall()
	{
		return (!this.RequirePower || this.IsPowered()) && (!this.RequireParent || base.baseEntity.HasParent());
	}

	// Token: 0x0600209C RID: 8348 RVA: 0x000D7370 File Offset: 0x000D5570
	public void AnswerPhone(global::BaseEntity.RPCMessage msg)
	{
		if (base.IsInvoking(new Action(this.TimeOutDialing)))
		{
			base.CancelInvoke(new Action(this.TimeOutDialing));
		}
		if (this.activeCallTo == null)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		this.UpdateServerPlayer(player);
		this.BeginCall();
		this.activeCallTo.BeginCall();
	}

	// Token: 0x0600209D RID: 8349 RVA: 0x000D73D1 File Offset: 0x000D55D1
	public void ReceiveCallFrom(PhoneController t)
	{
		this.activeCallTo = t;
		this.SetPhoneState(global::Telephone.CallState.Ringing);
		base.Invoke(new Action(this.TimeOutDialing), this.CallWaitingTime);
	}

	// Token: 0x0600209E RID: 8350 RVA: 0x000D73F9 File Offset: 0x000D55F9
	private void TimeOutDialing()
	{
		if (this.activeCallTo != null)
		{
			this.activeCallTo.ServerPlayAnsweringMessage(this);
		}
		this.SetPhoneState(global::Telephone.CallState.Idle);
	}

	// Token: 0x0600209F RID: 8351 RVA: 0x000D741C File Offset: 0x000D561C
	public void OnDialFailed(global::Telephone.DialFailReason reason)
	{
		this.SetPhoneState(global::Telephone.CallState.Idle);
		base.baseEntity.ClientRPC<int>(null, "ClientOnDialFailed", (int)reason);
		this.activeCallTo = null;
		if (base.IsInvoking(new Action(this.TimeOutCall)))
		{
			base.CancelInvoke(new Action(this.TimeOutCall));
		}
		if (base.IsInvoking(new Action(this.TriggerTimeOut)))
		{
			base.CancelInvoke(new Action(this.TriggerTimeOut));
		}
		if (base.IsInvoking(new Action(this.TimeOutDialing)))
		{
			base.CancelInvoke(new Action(this.TimeOutDialing));
		}
	}

	// Token: 0x060020A0 RID: 8352 RVA: 0x000D74BC File Offset: 0x000D56BC
	public void ServerPlayAnsweringMessage(PhoneController fromPhone)
	{
		NetworkableId arg = default(NetworkableId);
		uint num = 0U;
		uint arg2 = 0U;
		if (this.activeCallTo != null && this.activeCallTo.cachedCassette != null)
		{
			arg = this.activeCallTo.cachedCassette.net.ID;
			num = this.activeCallTo.cachedCassette.AudioId;
			if (num == 0U)
			{
				arg2 = StringPool.Get(this.activeCallTo.cachedCassette.PreloadedAudio.name);
			}
		}
		if (arg.IsValid)
		{
			base.baseEntity.ClientRPC<NetworkableId, uint, uint, int, int>(null, "ClientPlayAnsweringMessage", arg, num, arg2, fromPhone.HasVoicemailSlot() ? 1 : 0, this.activeCallTo.PhoneNumber);
			base.Invoke(new Action(this.TriggerTimeOut), this.activeCallTo.cachedCassette.MaxCassetteLength);
			return;
		}
		this.OnDialFailed(global::Telephone.DialFailReason.TimedOut);
	}

	// Token: 0x060020A1 RID: 8353 RVA: 0x000D7599 File Offset: 0x000D5799
	private void TriggerTimeOut()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.TimedOut);
	}

	// Token: 0x060020A2 RID: 8354 RVA: 0x000D75A4 File Offset: 0x000D57A4
	public void SetPhoneStateWithPlayer(global::Telephone.CallState state)
	{
		this.serverState = state;
		base.baseEntity.ClientRPC<int, int>(null, "SetClientState", (int)this.serverState, (this.activeCallTo != null) ? this.activeCallTo.PhoneNumber : 0);
		MobilePhone mobilePhone;
		if ((mobilePhone = (base.baseEntity as MobilePhone)) != null)
		{
			mobilePhone.ToggleRinging(state == global::Telephone.CallState.Ringing);
		}
	}

	// Token: 0x060020A3 RID: 8355 RVA: 0x000D7604 File Offset: 0x000D5804
	private void SetPhoneState(global::Telephone.CallState state)
	{
		if (state == global::Telephone.CallState.Idle && this.currentPlayer == null)
		{
			base.baseEntity.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
		}
		this.serverState = state;
		base.baseEntity.ClientRPC<int, int>(null, "SetClientState", (int)this.serverState, (this.activeCallTo != null) ? this.activeCallTo.PhoneNumber : 0);
		global::Telephone telephone;
		if ((telephone = (base.baseEntity as global::Telephone)) != null)
		{
			telephone.MarkDirtyForceUpdateOutputs();
		}
		MobilePhone mobilePhone;
		if ((mobilePhone = (base.baseEntity as MobilePhone)) != null)
		{
			mobilePhone.ToggleRinging(state == global::Telephone.CallState.Ringing);
		}
	}

	// Token: 0x060020A4 RID: 8356 RVA: 0x000D76A0 File Offset: 0x000D58A0
	public void BeginCall()
	{
		if (this.IsMobile && this.activeCallTo != null && !this.activeCallTo.RequirePower)
		{
			this.currentPlayer != null;
		}
		this.SetPhoneStateWithPlayer(global::Telephone.CallState.InProcess);
		base.Invoke(new Action(this.TimeOutCall), (float)TelephoneManager.MaxCallLength);
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x000D76FC File Offset: 0x000D58FC
	public void ServerHangUp(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		this.ServerHangUp();
	}

	// Token: 0x060020A6 RID: 8358 RVA: 0x000D7718 File Offset: 0x000D5918
	public void ServerHangUp()
	{
		if (this.activeCallTo != null)
		{
			this.activeCallTo.RemoteHangUp();
		}
		this.SelfHangUp();
	}

	// Token: 0x060020A7 RID: 8359 RVA: 0x000D7739 File Offset: 0x000D5939
	private void SelfHangUp()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.SelfHangUp);
	}

	// Token: 0x060020A8 RID: 8360 RVA: 0x000D7742 File Offset: 0x000D5942
	private void RemoteHangUp()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.RemoteHangUp);
	}

	// Token: 0x060020A9 RID: 8361 RVA: 0x000D774B File Offset: 0x000D594B
	private void TimeOutCall()
	{
		this.OnDialFailed(global::Telephone.DialFailReason.TimeOutDuringCall);
	}

	// Token: 0x060020AA RID: 8362 RVA: 0x000D7754 File Offset: 0x000D5954
	public void OnReceivedVoiceFromUser(byte[] data)
	{
		if (this.activeCallTo != null)
		{
			this.activeCallTo.OnReceivedDataFromConnectedPhone(data);
		}
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x000D7770 File Offset: 0x000D5970
	public void OnReceivedDataFromConnectedPhone(byte[] data)
	{
		base.baseEntity.ClientRPCEx<int, byte[]>(new SendInfo(global::BaseNetworkable.GetConnectionsWithin(base.transform.position, 15f))
		{
			priority = Priority.Immediate
		}, null, "OnReceivedVoice", data.Length, data);
	}

	// Token: 0x060020AC RID: 8364 RVA: 0x000D77B7 File Offset: 0x000D59B7
	public void OnIncomingCallWhileBusy()
	{
		base.baseEntity.ClientRPC(null, "OnIncomingCallDuringCall");
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x000D77CA File Offset: 0x000D59CA
	public void DestroyShared()
	{
		if (this.isServer && this.serverState != global::Telephone.CallState.Idle && this.activeCallTo != null)
		{
			this.activeCallTo.RemoteHangUp();
		}
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x000D77F8 File Offset: 0x000D59F8
	public void UpdatePhoneName(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		string text = msg.read.String(256);
		if (text.Length > 20)
		{
			text = text.Substring(0, 20);
		}
		this.PhoneName = text;
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x000D7854 File Offset: 0x000D5A54
	public void Server_RequestPhoneDirectory(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		int page = msg.read.Int32();
		using (PhoneDirectory phoneDirectory = Pool.Get<PhoneDirectory>())
		{
			TelephoneManager.GetPhoneDirectory(this.PhoneNumber, page, 12, phoneDirectory);
			base.baseEntity.ClientRPC<PhoneDirectory>(null, "ReceivePhoneDirectory", phoneDirectory);
		}
	}

	// Token: 0x060020B0 RID: 8368 RVA: 0x000D78C4 File Offset: 0x000D5AC4
	public void Server_AddSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		if (this.savedNumbers == null)
		{
			this.savedNumbers = Pool.Get<PhoneDirectory>();
		}
		if (this.savedNumbers.entries == null)
		{
			this.savedNumbers.entries = Pool.GetList<PhoneDirectory.DirectoryEntry>();
		}
		int num = msg.read.Int32();
		string text = msg.read.String(256);
		if (!this.IsSavedContactValid(text, num))
		{
			return;
		}
		if (this.savedNumbers.entries.Count >= 10)
		{
			return;
		}
		PhoneDirectory.DirectoryEntry directoryEntry = Pool.Get<PhoneDirectory.DirectoryEntry>();
		directoryEntry.phoneName = text;
		directoryEntry.phoneNumber = num;
		directoryEntry.ShouldPool = false;
		this.savedNumbers.ShouldPool = false;
		this.savedNumbers.entries.Add(directoryEntry);
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060020B1 RID: 8369 RVA: 0x000D7998 File Offset: 0x000D5B98
	public void Server_RemoveSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this.currentPlayer)
		{
			return;
		}
		uint number = msg.read.UInt32();
		if (this.savedNumbers.entries.RemoveAll((PhoneDirectory.DirectoryEntry p) => (long)p.phoneNumber == (long)((ulong)number)) > 0)
		{
			base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060020B2 RID: 8370 RVA: 0x000D79FB File Offset: 0x000D5BFB
	public string GetDirectoryName()
	{
		return this.PhoneName;
	}

	// Token: 0x060020B3 RID: 8371 RVA: 0x000D7A04 File Offset: 0x000D5C04
	private static string PositionToGridCoord(Vector3 position)
	{
		Vector2 a = new Vector2(TerrainMeta.NormalizeX(position.x), TerrainMeta.NormalizeZ(position.z));
		float num = TerrainMeta.Size.x / 1024f;
		int num2 = 7;
		Vector2 vector = a * num * (float)num2;
		float num3 = Mathf.Floor(vector.x) + 1f;
		float num4 = Mathf.Floor(num * (float)num2 - vector.y);
		string text = string.Empty;
		float num5 = num3 / 26f;
		float num6 = num3 % 26f;
		if (num6 == 0f)
		{
			num6 = 26f;
		}
		if (num5 > 1f)
		{
			text += Convert.ToChar(64 + (int)num5).ToString();
		}
		text += Convert.ToChar(64 + (int)num6).ToString();
		return string.Format("{0}{1}", text, num4);
	}

	// Token: 0x060020B4 RID: 8372 RVA: 0x000D7AEC File Offset: 0x000D5CEC
	public void WatchForDisconnects()
	{
		bool flag = false;
		if (this.currentPlayer != null)
		{
			if (this.currentPlayer.IsSleeping())
			{
				flag = true;
			}
			if (this.currentPlayer.IsDead())
			{
				flag = true;
			}
			if (Vector3.Distance(base.transform.position, this.currentPlayer.transform.position) > 5f)
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			this.ServerHangUp();
			this.ClearCurrentUser();
		}
	}

	// Token: 0x060020B5 RID: 8373 RVA: 0x000D7B63 File Offset: 0x000D5D63
	public void OnParentChanged(global::BaseEntity newParent)
	{
		if (newParent != null && newParent is global::BasePlayer)
		{
			TelephoneManager.RegisterTelephone(this, true);
			return;
		}
		TelephoneManager.DeregisterTelephone(this);
	}

	// Token: 0x060020B6 RID: 8374 RVA: 0x000D7B8A File Offset: 0x000D5D8A
	private bool HasVoicemailSlot()
	{
		return this.MaxVoicemailSlots > 0;
	}

	// Token: 0x060020B7 RID: 8375 RVA: 0x000D7B98 File Offset: 0x000D5D98
	public void ServerSendVoicemail(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		byte[] data = msg.read.BytesWithSize(10485760U);
		PhoneController telephone = TelephoneManager.GetTelephone(msg.read.Int32());
		if (telephone == null)
		{
			return;
		}
		if (!global::Cassette.IsOggValid(data, telephone.cachedCassette))
		{
			return;
		}
		telephone.SaveVoicemail(data, msg.player.displayName);
	}

	// Token: 0x060020B8 RID: 8376 RVA: 0x000D7C04 File Offset: 0x000D5E04
	public void SaveVoicemail(byte[] data, string playerName)
	{
		uint audioId = FileStorage.server.Store(data, FileStorage.Type.ogg, base.baseEntity.net.ID, 0U);
		if (this.savedVoicemail == null)
		{
			this.savedVoicemail = Pool.GetList<ProtoBuf.VoicemailEntry>();
		}
		ProtoBuf.VoicemailEntry voicemailEntry = Pool.Get<ProtoBuf.VoicemailEntry>();
		voicemailEntry.audioId = audioId;
		voicemailEntry.timestamp = DateTime.Now.ToBinary();
		voicemailEntry.userName = playerName;
		voicemailEntry.ShouldPool = false;
		this.savedVoicemail.Add(voicemailEntry);
		while (this.savedVoicemail.Count > this.MaxVoicemailSlots)
		{
			FileStorage.server.Remove(this.savedVoicemail[0].audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
			this.savedVoicemail.RemoveAt(0);
		}
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060020B9 RID: 8377 RVA: 0x000D7CD5 File Offset: 0x000D5ED5
	public void ServerPlayVoicemail(global::BaseEntity.RPCMessage msg)
	{
		base.baseEntity.ClientRPC<int, uint>(null, "ClientToggleVoicemail", 1, msg.read.UInt32());
	}

	// Token: 0x060020BA RID: 8378 RVA: 0x000D7CF4 File Offset: 0x000D5EF4
	public void ServerStopVoicemail(global::BaseEntity.RPCMessage msg)
	{
		base.baseEntity.ClientRPC<int, uint>(null, "ClientToggleVoicemail", 0, msg.read.UInt32());
	}

	// Token: 0x060020BB RID: 8379 RVA: 0x000D7D14 File Offset: 0x000D5F14
	public void ServerDeleteVoicemail(global::BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		for (int i = 0; i < this.savedVoicemail.Count; i++)
		{
			if (this.savedVoicemail[i].audioId == num)
			{
				ProtoBuf.VoicemailEntry voicemailEntry = this.savedVoicemail[i];
				FileStorage.server.Remove(voicemailEntry.audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
				voicemailEntry.ShouldPool = true;
				Pool.Free<ProtoBuf.VoicemailEntry>(ref voicemailEntry);
				this.savedVoicemail.RemoveAt(i);
				base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
		}
	}

	// Token: 0x060020BC RID: 8380 RVA: 0x000D7DAC File Offset: 0x000D5FAC
	public void DeleteAllVoicemail()
	{
		if (this.savedVoicemail == null)
		{
			return;
		}
		foreach (ProtoBuf.VoicemailEntry voicemailEntry in this.savedVoicemail)
		{
			voicemailEntry.ShouldPool = true;
			FileStorage.server.Remove(voicemailEntry.audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
		}
		Pool.FreeList<ProtoBuf.VoicemailEntry>(ref this.savedVoicemail);
	}

	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x060020BD RID: 8381 RVA: 0x000D7E34 File Offset: 0x000D6034
	public int MaxVoicemailSlots
	{
		get
		{
			if (!(this.cachedCassette != null))
			{
				return 0;
			}
			return this.cachedCassette.MaximumVoicemailSlots;
		}
	}

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x060020BE RID: 8382 RVA: 0x000D7E51 File Offset: 0x000D6051
	// (set) Token: 0x060020BF RID: 8383 RVA: 0x000D7E7E File Offset: 0x000D607E
	public global::BasePlayer currentPlayer
	{
		get
		{
			if (this.currentPlayerRef.IsValid(this.isServer))
			{
				return this.currentPlayerRef.Get(this.isServer).ToPlayer();
			}
			return null;
		}
		set
		{
			this.currentPlayerRef.Set(value);
		}
	}

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x060020C0 RID: 8384 RVA: 0x000D7E8C File Offset: 0x000D608C
	private bool isServer
	{
		get
		{
			return base.baseEntity != null && base.baseEntity.isServer;
		}
	}

	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x060020C1 RID: 8385 RVA: 0x000D7EA9 File Offset: 0x000D60A9
	// (set) Token: 0x060020C2 RID: 8386 RVA: 0x000D7EB1 File Offset: 0x000D60B1
	public int lastDialedNumber { get; set; }

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x060020C3 RID: 8387 RVA: 0x000D7EBA File Offset: 0x000D60BA
	// (set) Token: 0x060020C4 RID: 8388 RVA: 0x000D7EC2 File Offset: 0x000D60C2
	public PhoneDirectory savedNumbers { get; set; }

	// Token: 0x170002BB RID: 699
	// (get) Token: 0x060020C5 RID: 8389 RVA: 0x000D6C9A File Offset: 0x000D4E9A
	public global::BaseEntity ParentEntity
	{
		get
		{
			return base.baseEntity;
		}
	}

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x060020C6 RID: 8390 RVA: 0x000D7ECC File Offset: 0x000D60CC
	private global::Cassette cachedCassette
	{
		get
		{
			global::Telephone telephone;
			if (!(base.baseEntity != null) || (telephone = (base.baseEntity as global::Telephone)) == null)
			{
				return null;
			}
			return telephone.cachedCassette;
		}
	}

	// Token: 0x060020C7 RID: 8391 RVA: 0x000D7F00 File Offset: 0x000D6100
	private bool IsPowered()
	{
		global::IOEntity ioentity;
		return base.baseEntity != null && (ioentity = (base.baseEntity as global::IOEntity)) != null && ioentity.IsPowered();
	}

	// Token: 0x060020C8 RID: 8392 RVA: 0x000D7F32 File Offset: 0x000D6132
	public bool IsSavedContactValid(string contactName, int contactNumber)
	{
		return contactName.Length > 0 && contactName.Length <= 20 && contactNumber >= 10000000 && contactNumber < 100000000;
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
	}
}
