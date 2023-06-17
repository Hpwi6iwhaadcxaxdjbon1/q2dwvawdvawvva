using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Fleck;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009E5 RID: 2533
	public class Listener : IDisposable, IBroadcastSender<Connection, AppBroadcast>
	{
		// Token: 0x040036A0 RID: 13984
		private static readonly ByteArrayStream Stream = new ByteArrayStream();

		// Token: 0x040036A1 RID: 13985
		private readonly TokenBucketList<IPAddress> _ipTokenBuckets;

		// Token: 0x040036A2 RID: 13986
		private readonly BanList<IPAddress> _ipBans;

		// Token: 0x040036A3 RID: 13987
		private readonly TokenBucketList<ulong> _playerTokenBuckets;

		// Token: 0x040036A4 RID: 13988
		private readonly TokenBucketList<ulong> _pairingTokenBuckets;

		// Token: 0x040036A5 RID: 13989
		private readonly Queue<Listener.Message> _messageQueue;

		// Token: 0x040036A6 RID: 13990
		private readonly WebSocketServer _server;

		// Token: 0x040036A7 RID: 13991
		private readonly Stopwatch _stopwatch;

		// Token: 0x040036A8 RID: 13992
		private RealTimeSince _lastCleanup;

		// Token: 0x040036A9 RID: 13993
		private long _nextConnectionId;

		// Token: 0x040036AA RID: 13994
		public readonly IPAddress Address;

		// Token: 0x040036AB RID: 13995
		public readonly int Port;

		// Token: 0x040036AC RID: 13996
		public readonly ConnectionLimiter Limiter;

		// Token: 0x040036AD RID: 13997
		public readonly SubscriberList<PlayerTarget, Connection, AppBroadcast> PlayerSubscribers;

		// Token: 0x040036AE RID: 13998
		public readonly SubscriberList<EntityTarget, Connection, AppBroadcast> EntitySubscribers;

		// Token: 0x040036AF RID: 13999
		public readonly SubscriberList<CameraTarget, Connection, AppBroadcast> CameraSubscribers;

		// Token: 0x06003C96 RID: 15510 RVA: 0x00164248 File Offset: 0x00162448
		public Listener(IPAddress ipAddress, int port)
		{
			this.Address = ipAddress;
			this.Port = port;
			this.Limiter = new ConnectionLimiter();
			this._ipTokenBuckets = new TokenBucketList<IPAddress>(50.0, 15.0);
			this._ipBans = new BanList<IPAddress>();
			this._playerTokenBuckets = new TokenBucketList<ulong>(25.0, 3.0);
			this._pairingTokenBuckets = new TokenBucketList<ulong>(5.0, 0.1);
			this._messageQueue = new Queue<Listener.Message>();
			SynchronizationContext syncContext = SynchronizationContext.Current;
			this._server = new WebSocketServer(string.Format("ws://{0}:{1}/", this.Address, this.Port), true);
			this._server.Start(delegate(IWebSocketConnection socket)
			{
				IPAddress address = socket.ConnectionInfo.ClientIpAddress;
				if (!this.Limiter.TryAdd(address) || this._ipBans.IsBanned(address))
				{
					socket.Close();
					return;
				}
				long connectionId = Interlocked.Increment(ref this._nextConnectionId);
				Connection conn = new Connection(connectionId, this, socket);
				socket.OnClose = delegate()
				{
					this.Limiter.Remove(address);
					syncContext.Post(delegate(object c)
					{
						((Connection)c).OnClose();
					}, conn);
				};
				socket.OnBinary = new BinaryDataHandler(conn.OnMessage);
				socket.OnError = new Action<Exception>(UnityEngine.Debug.LogError);
			});
			this._stopwatch = new Stopwatch();
			this.PlayerSubscribers = new SubscriberList<PlayerTarget, Connection, AppBroadcast>(this, null);
			this.EntitySubscribers = new SubscriberList<EntityTarget, Connection, AppBroadcast>(this, null);
			this.CameraSubscribers = new SubscriberList<CameraTarget, Connection, AppBroadcast>(this, new double?((double)30));
		}

		// Token: 0x06003C97 RID: 15511 RVA: 0x00164380 File Offset: 0x00162580
		public void Dispose()
		{
			WebSocketServer server = this._server;
			if (server == null)
			{
				return;
			}
			server.Dispose();
		}

		// Token: 0x06003C98 RID: 15512 RVA: 0x00164394 File Offset: 0x00162594
		internal void Enqueue(Connection connection, MemoryBuffer data)
		{
			Queue<Listener.Message> messageQueue = this._messageQueue;
			lock (messageQueue)
			{
				if (!App.update || this._messageQueue.Count >= App.queuelimit)
				{
					data.Dispose();
				}
				else
				{
					Listener.Message item = new Listener.Message(connection, data);
					this._messageQueue.Enqueue(item);
				}
			}
		}

		// Token: 0x06003C99 RID: 15513 RVA: 0x00164408 File Offset: 0x00162608
		public void Update()
		{
			if (!App.update)
			{
				return;
			}
			using (TimeWarning.New("CompanionServer.MessageQueue", 0))
			{
				Queue<Listener.Message> messageQueue = this._messageQueue;
				lock (messageQueue)
				{
					this._stopwatch.Restart();
					while (this._messageQueue.Count > 0 && this._stopwatch.Elapsed.TotalMilliseconds < 5.0)
					{
						Listener.Message message = this._messageQueue.Dequeue();
						this.Dispatch(message);
					}
				}
			}
			if (this._lastCleanup >= 3f)
			{
				this._lastCleanup = 0f;
				this._ipTokenBuckets.Cleanup();
				this._ipBans.Cleanup();
				this._playerTokenBuckets.Cleanup();
				this._pairingTokenBuckets.Cleanup();
			}
		}

		// Token: 0x06003C9A RID: 15514 RVA: 0x00164508 File Offset: 0x00162708
		private void Dispatch(Listener.Message message)
		{
			Listener.<>c__DisplayClass21_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.message = message;
			using (CS$<>8__locals1.message.Buffer)
			{
				try
				{
					Listener.Stream.SetData(CS$<>8__locals1.message.Buffer.Data, 0, CS$<>8__locals1.message.Buffer.Length);
					CS$<>8__locals1.request = AppRequest.Deserialize(Listener.Stream);
				}
				catch
				{
					DebugEx.LogWarning(string.Format("Malformed companion packet from {0}", CS$<>8__locals1.message.Connection.Address), StackTraceLogType.None);
					CS$<>8__locals1.message.Connection.Close();
					throw;
				}
			}
			CompanionServer.Handlers.IHandler handler;
			if (!this.<Dispatch>g__Handle|21_15<AppEmpty, Info>((AppRequest r) => r.getInfo, out handler, ref CS$<>8__locals1))
			{
				if (!this.<Dispatch>g__Handle|21_15<AppEmpty, CompanionServer.Handlers.Time>((AppRequest r) => r.getTime, out handler, ref CS$<>8__locals1))
				{
					if (!this.<Dispatch>g__Handle|21_15<AppEmpty, Map>((AppRequest r) => r.getMap, out handler, ref CS$<>8__locals1))
					{
						if (!this.<Dispatch>g__Handle|21_15<AppEmpty, TeamInfo>((AppRequest r) => r.getTeamInfo, out handler, ref CS$<>8__locals1))
						{
							if (!this.<Dispatch>g__Handle|21_15<AppEmpty, TeamChat>((AppRequest r) => r.getTeamChat, out handler, ref CS$<>8__locals1))
							{
								if (!this.<Dispatch>g__Handle|21_15<AppSendMessage, SendTeamChat>((AppRequest r) => r.sendTeamMessage, out handler, ref CS$<>8__locals1))
								{
									if (!this.<Dispatch>g__Handle|21_15<AppEmpty, EntityInfo>((AppRequest r) => r.getEntityInfo, out handler, ref CS$<>8__locals1))
									{
										if (!this.<Dispatch>g__Handle|21_15<AppSetEntityValue, SetEntityValue>((AppRequest r) => r.setEntityValue, out handler, ref CS$<>8__locals1))
										{
											if (!this.<Dispatch>g__Handle|21_15<AppEmpty, CheckSubscription>((AppRequest r) => r.checkSubscription, out handler, ref CS$<>8__locals1))
											{
												if (!this.<Dispatch>g__Handle|21_15<AppFlag, SetSubscription>((AppRequest r) => r.setSubscription, out handler, ref CS$<>8__locals1))
												{
													if (!this.<Dispatch>g__Handle|21_15<AppEmpty, MapMarkers>((AppRequest r) => r.getMapMarkers, out handler, ref CS$<>8__locals1))
													{
														if (!this.<Dispatch>g__Handle|21_15<AppPromoteToLeader, PromoteToLeader>((AppRequest r) => r.promoteToLeader, out handler, ref CS$<>8__locals1))
														{
															if (!this.<Dispatch>g__Handle|21_15<AppCameraSubscribe, CameraSubscribe>((AppRequest r) => r.cameraSubscribe, out handler, ref CS$<>8__locals1))
															{
																if (!this.<Dispatch>g__Handle|21_15<AppEmpty, CameraUnsubscribe>((AppRequest r) => r.cameraUnsubscribe, out handler, ref CS$<>8__locals1))
																{
																	if (!this.<Dispatch>g__Handle|21_15<AppCameraInput, CameraInput>((AppRequest r) => r.cameraInput, out handler, ref CS$<>8__locals1))
																	{
																		AppResponse appResponse = Facepunch.Pool.Get<AppResponse>();
																		appResponse.seq = CS$<>8__locals1.request.seq;
																		appResponse.error = Facepunch.Pool.Get<AppError>();
																		appResponse.error.error = "unhandled";
																		CS$<>8__locals1.message.Connection.Send(appResponse);
																		CS$<>8__locals1.request.Dispose();
																		return;
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			try
			{
				ValidationResult validationResult = handler.Validate();
				if (validationResult == ValidationResult.Rejected)
				{
					CS$<>8__locals1.message.Connection.Close();
				}
				else if (validationResult != ValidationResult.Success)
				{
					handler.SendError(validationResult.ToErrorCode());
				}
				else
				{
					handler.Execute();
				}
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogError(string.Format("AppRequest threw an exception: {0}", arg));
				handler.SendError("server_error");
			}
			Facepunch.Pool.FreeDynamic<CompanionServer.Handlers.IHandler>(ref handler);
		}

		// Token: 0x06003C9B RID: 15515 RVA: 0x00164938 File Offset: 0x00162B38
		public void BroadcastTo(List<Connection> targets, AppBroadcast broadcast)
		{
			MemoryBuffer broadcastBuffer = Listener.GetBroadcastBuffer(broadcast);
			foreach (Connection connection in targets)
			{
				connection.SendRaw(broadcastBuffer.DontDispose());
			}
			broadcastBuffer.Dispose();
		}

		// Token: 0x06003C9C RID: 15516 RVA: 0x00164998 File Offset: 0x00162B98
		private static MemoryBuffer GetBroadcastBuffer(AppBroadcast broadcast)
		{
			MemoryBuffer memoryBuffer = new MemoryBuffer(65536);
			Listener.Stream.SetData(memoryBuffer.Data, 0, memoryBuffer.Length);
			AppMessage appMessage = Facepunch.Pool.Get<AppMessage>();
			appMessage.broadcast = broadcast;
			appMessage.ToProto(Listener.Stream);
			if (appMessage.ShouldPool)
			{
				appMessage.Dispose();
			}
			return memoryBuffer.Slice((int)Listener.Stream.Position);
		}

		// Token: 0x06003C9D RID: 15517 RVA: 0x00164A02 File Offset: 0x00162C02
		public bool CanSendPairingNotification(ulong playerId)
		{
			return this._pairingTokenBuckets.Get(playerId).TryTake(1.0);
		}

		// Token: 0x06003C9F RID: 15519 RVA: 0x00164A2C File Offset: 0x00162C2C
		[CompilerGenerated]
		private bool <Dispatch>g__Handle|21_15<TProto, THandler>(Func<AppRequest, TProto> protoSelector, out CompanionServer.Handlers.IHandler requestHandler, ref Listener.<>c__DisplayClass21_0 A_3) where TProto : class where THandler : BaseHandler<TProto>, new()
		{
			TProto tproto = protoSelector(A_3.request);
			if (tproto == null)
			{
				requestHandler = null;
				return false;
			}
			THandler thandler = Facepunch.Pool.Get<THandler>();
			thandler.Initialize(this._playerTokenBuckets, A_3.message.Connection, A_3.request, tproto);
			requestHandler = thandler;
			return true;
		}

		// Token: 0x02000EE9 RID: 3817
		private struct Message
		{
			// Token: 0x04004D94 RID: 19860
			public readonly Connection Connection;

			// Token: 0x04004D95 RID: 19861
			public readonly MemoryBuffer Buffer;

			// Token: 0x060053B9 RID: 21433 RVA: 0x001B3593 File Offset: 0x001B1793
			public Message(Connection connection, MemoryBuffer buffer)
			{
				this.Connection = connection;
				this.Buffer = buffer;
			}
		}
	}
}
