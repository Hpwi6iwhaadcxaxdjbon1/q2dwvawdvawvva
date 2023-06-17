using System;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009F4 RID: 2548
	public abstract class BaseHandler<T> : IHandler, Pool.IPooled where T : class
	{
		// Token: 0x040036E2 RID: 14050
		private TokenBucketList<ulong> _playerBuckets;

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06003CF6 RID: 15606 RVA: 0x0016652A File Offset: 0x0016472A
		protected virtual double TokenCost
		{
			get
			{
				return 1.0;
			}
		}

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06003CF7 RID: 15607 RVA: 0x00166535 File Offset: 0x00164735
		// (set) Token: 0x06003CF8 RID: 15608 RVA: 0x0016653D File Offset: 0x0016473D
		public IConnection Client { get; private set; }

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06003CF9 RID: 15609 RVA: 0x00166546 File Offset: 0x00164746
		// (set) Token: 0x06003CFA RID: 15610 RVA: 0x0016654E File Offset: 0x0016474E
		public AppRequest Request { get; private set; }

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06003CFB RID: 15611 RVA: 0x00166557 File Offset: 0x00164757
		// (set) Token: 0x06003CFC RID: 15612 RVA: 0x0016655F File Offset: 0x0016475F
		public T Proto { get; private set; }

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06003CFD RID: 15613 RVA: 0x00166568 File Offset: 0x00164768
		// (set) Token: 0x06003CFE RID: 15614 RVA: 0x00166570 File Offset: 0x00164770
		private protected ulong UserId { protected get; private set; }

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06003CFF RID: 15615 RVA: 0x00166579 File Offset: 0x00164779
		// (set) Token: 0x06003D00 RID: 15616 RVA: 0x00166581 File Offset: 0x00164781
		private protected global::BasePlayer Player { protected get; private set; }

		// Token: 0x06003D01 RID: 15617 RVA: 0x0016658A File Offset: 0x0016478A
		public void Initialize(TokenBucketList<ulong> playerBuckets, IConnection client, AppRequest request, T proto)
		{
			this._playerBuckets = playerBuckets;
			this.Client = client;
			this.Request = request;
			this.Proto = proto;
		}

		// Token: 0x06003D02 RID: 15618 RVA: 0x001665AC File Offset: 0x001647AC
		public virtual void EnterPool()
		{
			this._playerBuckets = null;
			this.Client = null;
			if (this.Request != null)
			{
				this.Request.Dispose();
				this.Request = null;
			}
			this.Proto = default(T);
			this.UserId = 0UL;
			this.Player = null;
		}

		// Token: 0x06003D03 RID: 15619 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}

		// Token: 0x06003D04 RID: 15620 RVA: 0x00166600 File Offset: 0x00164800
		public virtual ValidationResult Validate()
		{
			bool flag;
			int orGenerateAppToken = SingletonComponent<ServerMgr>.Instance.persistance.GetOrGenerateAppToken(this.Request.playerId, out flag);
			if (this.Request.playerId == 0UL || this.Request.playerToken != orGenerateAppToken)
			{
				return ValidationResult.NotFound;
			}
			if (flag)
			{
				return ValidationResult.Banned;
			}
			ServerUsers.User user = ServerUsers.Get(this.Request.playerId);
			if (((user != null) ? user.group : ServerUsers.UserGroup.None) == ServerUsers.UserGroup.Banned)
			{
				return ValidationResult.Banned;
			}
			TokenBucketList<ulong> playerBuckets = this._playerBuckets;
			TokenBucket tokenBucket = (playerBuckets != null) ? playerBuckets.Get(this.Request.playerId) : null;
			if (tokenBucket != null && tokenBucket.TryTake(this.TokenCost))
			{
				this.UserId = this.Request.playerId;
				this.Player = (global::BasePlayer.FindByID(this.UserId) ?? global::BasePlayer.FindSleeping(this.UserId));
				this.Client.Subscribe(new PlayerTarget(this.UserId));
				return ValidationResult.Success;
			}
			if (tokenBucket == null || !tokenBucket.IsNaughty)
			{
				return ValidationResult.RateLimit;
			}
			return ValidationResult.Rejected;
		}

		// Token: 0x06003D05 RID: 15621
		public abstract void Execute();

		// Token: 0x06003D06 RID: 15622 RVA: 0x001666F4 File Offset: 0x001648F4
		protected void SendSuccess()
		{
			AppSuccess success = Pool.Get<AppSuccess>();
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.success = success;
			this.Send(appResponse);
		}

		// Token: 0x06003D07 RID: 15623 RVA: 0x0016671C File Offset: 0x0016491C
		public void SendError(string code)
		{
			AppError appError = Pool.Get<AppError>();
			appError.error = code;
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.error = appError;
			this.Send(appResponse);
		}

		// Token: 0x06003D08 RID: 15624 RVA: 0x0016674C File Offset: 0x0016494C
		public void SendFlag(bool value)
		{
			AppFlag appFlag = Pool.Get<AppFlag>();
			appFlag.value = value;
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.flag = appFlag;
			this.Send(appResponse);
		}

		// Token: 0x06003D09 RID: 15625 RVA: 0x0016677A File Offset: 0x0016497A
		protected void Send(AppResponse response)
		{
			response.seq = this.Request.seq;
			this.Client.Send(response);
		}
	}
}
