using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ConVar;
using Facepunch;
using Fleck;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009E1 RID: 2529
	public class Connection : IConnection
	{
		// Token: 0x04003692 RID: 13970
		private static readonly MemoryStream MessageStream = new MemoryStream(1048576);

		// Token: 0x04003693 RID: 13971
		private readonly Listener _listener;

		// Token: 0x04003694 RID: 13972
		private readonly IWebSocketConnection _connection;

		// Token: 0x04003695 RID: 13973
		private PlayerTarget? _subscribedPlayer;

		// Token: 0x04003696 RID: 13974
		private readonly HashSet<EntityTarget> _subscribedEntities;

		// Token: 0x04003697 RID: 13975
		private IRemoteControllable _currentCamera;

		// Token: 0x04003698 RID: 13976
		private ulong _cameraViewerSteamId;

		// Token: 0x04003699 RID: 13977
		private bool _isControllingCamera;

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06003C6B RID: 15467 RVA: 0x001639AC File Offset: 0x00161BAC
		// (set) Token: 0x06003C6C RID: 15468 RVA: 0x001639B4 File Offset: 0x00161BB4
		public long ConnectionId { get; private set; }

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06003C6D RID: 15469 RVA: 0x001639BD File Offset: 0x00161BBD
		public IPAddress Address
		{
			get
			{
				return this._connection.ConnectionInfo.ClientIpAddress;
			}
		}

		// Token: 0x06003C6E RID: 15470 RVA: 0x001639CF File Offset: 0x00161BCF
		public Connection(long connectionId, Listener listener, IWebSocketConnection connection)
		{
			this.ConnectionId = connectionId;
			this._listener = listener;
			this._connection = connection;
			this._subscribedEntities = new HashSet<EntityTarget>();
		}

		// Token: 0x06003C6F RID: 15471 RVA: 0x001639F8 File Offset: 0x00161BF8
		public void OnClose()
		{
			if (this._subscribedPlayer != null)
			{
				this._listener.PlayerSubscribers.Remove(this._subscribedPlayer.Value, this);
				this._subscribedPlayer = null;
			}
			foreach (EntityTarget key in this._subscribedEntities)
			{
				this._listener.EntitySubscribers.Remove(key, this);
			}
			this._subscribedEntities.Clear();
			IRemoteControllable currentCamera = this._currentCamera;
			if (currentCamera != null)
			{
				currentCamera.StopControl(new CameraViewerId(this._cameraViewerSteamId, this.ConnectionId));
			}
			CameraTarget key2;
			if (Connection.TryGetCameraTarget(this._currentCamera, out key2))
			{
				this._listener.CameraSubscribers.Remove(key2, this);
			}
			this._currentCamera = null;
			this._cameraViewerSteamId = 0UL;
			this._isControllingCamera = false;
		}

		// Token: 0x06003C70 RID: 15472 RVA: 0x00163AF0 File Offset: 0x00161CF0
		public void OnMessage(System.Span<byte> data)
		{
			if (!App.update || App.queuelimit <= 0 || data.Length > App.maxmessagesize)
			{
				return;
			}
			MemoryBuffer buffer = new MemoryBuffer(data.Length);
			data.CopyTo(buffer);
			this._listener.Enqueue(this, buffer.Slice(data.Length));
		}

		// Token: 0x06003C71 RID: 15473 RVA: 0x00163B50 File Offset: 0x00161D50
		public void Close()
		{
			IWebSocketConnection connection = this._connection;
			if (connection == null)
			{
				return;
			}
			connection.Close();
		}

		// Token: 0x06003C72 RID: 15474 RVA: 0x00163B64 File Offset: 0x00161D64
		public void Send(AppResponse response)
		{
			AppMessage appMessage = Facepunch.Pool.Get<AppMessage>();
			appMessage.response = response;
			Connection.MessageStream.Position = 0L;
			appMessage.ToProto(Connection.MessageStream);
			int num = (int)Connection.MessageStream.Position;
			Connection.MessageStream.Position = 0L;
			MemoryBuffer memoryBuffer = new MemoryBuffer(num);
			Connection.MessageStream.Read(memoryBuffer.Data, 0, num);
			if (appMessage.ShouldPool)
			{
				appMessage.Dispose();
			}
			this.SendRaw(memoryBuffer.Slice(num));
		}

		// Token: 0x06003C73 RID: 15475 RVA: 0x00163BE8 File Offset: 0x00161DE8
		public void Subscribe(PlayerTarget target)
		{
			PlayerTarget? subscribedPlayer = this._subscribedPlayer;
			if (subscribedPlayer != null && (subscribedPlayer == null || subscribedPlayer.GetValueOrDefault() == target))
			{
				return;
			}
			this.EndViewing();
			if (this._subscribedPlayer != null)
			{
				this._listener.PlayerSubscribers.Remove(this._subscribedPlayer.Value, this);
				this._subscribedPlayer = null;
			}
			this._listener.PlayerSubscribers.Add(target, this);
			this._subscribedPlayer = new PlayerTarget?(target);
		}

		// Token: 0x06003C74 RID: 15476 RVA: 0x00163C7F File Offset: 0x00161E7F
		public void Subscribe(EntityTarget target)
		{
			if (this._subscribedEntities.Add(target))
			{
				this._listener.EntitySubscribers.Add(target, this);
			}
		}

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06003C75 RID: 15477 RVA: 0x00163CA1 File Offset: 0x00161EA1
		public IRemoteControllable CurrentCamera
		{
			get
			{
				return this._currentCamera;
			}
		}

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06003C76 RID: 15478 RVA: 0x00163CA9 File Offset: 0x00161EA9
		public bool IsControllingCamera
		{
			get
			{
				return this._isControllingCamera;
			}
		}

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06003C77 RID: 15479 RVA: 0x00163CB1 File Offset: 0x00161EB1
		public ulong ControllingSteamId
		{
			get
			{
				return this._cameraViewerSteamId;
			}
		}

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06003C78 RID: 15480 RVA: 0x00163CB9 File Offset: 0x00161EB9
		// (set) Token: 0x06003C79 RID: 15481 RVA: 0x00163CC1 File Offset: 0x00161EC1
		public InputState InputState { get; set; }

		// Token: 0x06003C7A RID: 15482 RVA: 0x00163CCC File Offset: 0x00161ECC
		public bool BeginViewing(IRemoteControllable camera)
		{
			if (this._subscribedPlayer == null)
			{
				return false;
			}
			CameraTarget key;
			if (!Connection.TryGetCameraTarget(camera, out key))
			{
				if (this._currentCamera == camera)
				{
					IRemoteControllable currentCamera = this._currentCamera;
					if (currentCamera != null)
					{
						currentCamera.StopControl(new CameraViewerId(this._cameraViewerSteamId, this.ConnectionId));
					}
					this._currentCamera = null;
					this._isControllingCamera = false;
					this._cameraViewerSteamId = 0UL;
				}
				return false;
			}
			if (this._currentCamera == camera)
			{
				this._listener.CameraSubscribers.Add(key, this);
				return true;
			}
			CameraTarget key2;
			if (Connection.TryGetCameraTarget(this._currentCamera, out key2))
			{
				this._listener.CameraSubscribers.Remove(key2, this);
				this._currentCamera.StopControl(new CameraViewerId(this._cameraViewerSteamId, this.ConnectionId));
				this._currentCamera = null;
				this._isControllingCamera = false;
				this._cameraViewerSteamId = 0UL;
			}
			ulong steamId = this._subscribedPlayer.Value.SteamId;
			if (!camera.CanControl(steamId))
			{
				return false;
			}
			this._listener.CameraSubscribers.Add(key, this);
			this._currentCamera = camera;
			this._isControllingCamera = this._currentCamera.InitializeControl(new CameraViewerId(steamId, this.ConnectionId));
			this._cameraViewerSteamId = steamId;
			InputState inputState = this.InputState;
			if (inputState != null)
			{
				inputState.Clear();
			}
			return true;
		}

		// Token: 0x06003C7B RID: 15483 RVA: 0x00163E14 File Offset: 0x00162014
		public void EndViewing()
		{
			CameraTarget key;
			if (Connection.TryGetCameraTarget(this._currentCamera, out key))
			{
				this._listener.CameraSubscribers.Remove(key, this);
			}
			IRemoteControllable currentCamera = this._currentCamera;
			if (currentCamera != null)
			{
				currentCamera.StopControl(new CameraViewerId(this._cameraViewerSteamId, this.ConnectionId));
			}
			this._currentCamera = null;
			this._isControllingCamera = false;
			this._cameraViewerSteamId = 0UL;
		}

		// Token: 0x06003C7C RID: 15484 RVA: 0x00163E7C File Offset: 0x0016207C
		public void SendRaw(MemoryBuffer data)
		{
			try
			{
				this._connection.Send(data);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failed to send message to app client {0}: {1}", this._connection.ConnectionInfo.ClientIpAddress, arg));
			}
		}

		// Token: 0x06003C7D RID: 15485 RVA: 0x00163ECC File Offset: 0x001620CC
		private static bool TryGetCameraTarget(IRemoteControllable camera, out CameraTarget target)
		{
			global::BaseEntity baseEntity = (camera != null) ? camera.GetEnt() : null;
			if (camera.IsUnityNull<IRemoteControllable>() || baseEntity == null || !baseEntity.IsValid())
			{
				target = default(CameraTarget);
				return false;
			}
			target = new CameraTarget(baseEntity.net.ID);
			return true;
		}
	}
}
