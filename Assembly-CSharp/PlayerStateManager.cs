using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000742 RID: 1858
public class PlayerStateManager
{
	// Token: 0x04002A19 RID: 10777
	private readonly MruDictionary<ulong, PlayerState> _cache;

	// Token: 0x04002A1A RID: 10778
	private readonly UserPersistance _persistence;

	// Token: 0x060033D6 RID: 13270 RVA: 0x0013F245 File Offset: 0x0013D445
	public PlayerStateManager(UserPersistance persistence)
	{
		this._cache = new MruDictionary<ulong, PlayerState>(1000, new Action<ulong, PlayerState>(this.FreeOldState));
		this._persistence = persistence;
	}

	// Token: 0x060033D7 RID: 13271 RVA: 0x0013F270 File Offset: 0x0013D470
	public PlayerState Get(ulong playerId)
	{
		PlayerState result;
		using (TimeWarning.New("PlayerStateManager.Get", 0))
		{
			PlayerState playerState;
			if (this._cache.TryGetValue(playerId, out playerState))
			{
				result = playerState;
			}
			else
			{
				byte[] playerState2 = this._persistence.GetPlayerState(playerId);
				PlayerState playerState3;
				if (playerState2 != null && playerState2.Length != 0)
				{
					try
					{
						playerState3 = PlayerState.Deserialize(playerState2);
						this.OnPlayerStateLoaded(playerState3);
						this._cache.Add(playerId, playerState3);
						return playerState3;
					}
					catch (Exception arg)
					{
						Debug.LogError(string.Format("Failed to load player state for {0}: {1}", playerId, arg));
					}
				}
				playerState3 = Pool.Get<PlayerState>();
				this._cache.Add(playerId, playerState3);
				result = playerState3;
			}
		}
		return result;
	}

	// Token: 0x060033D8 RID: 13272 RVA: 0x0013F330 File Offset: 0x0013D530
	public void Save(ulong playerId)
	{
		PlayerState state;
		if (!this._cache.TryGetValue(playerId, out state))
		{
			return;
		}
		this.SaveState(playerId, state);
	}

	// Token: 0x060033D9 RID: 13273 RVA: 0x0013F358 File Offset: 0x0013D558
	private void SaveState(ulong playerId, PlayerState state)
	{
		using (TimeWarning.New("PlayerStateManager.SaveState", 0))
		{
			try
			{
				byte[] state2 = PlayerState.SerializeToBytes(state);
				this._persistence.SetPlayerState(playerId, state2);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failed to save player state for {0}: {1}", playerId, arg));
			}
		}
	}

	// Token: 0x060033DA RID: 13274 RVA: 0x0013F3C8 File Offset: 0x0013D5C8
	private void FreeOldState(ulong playerId, PlayerState state)
	{
		this.SaveState(playerId, state);
		state.Dispose();
	}

	// Token: 0x060033DB RID: 13275 RVA: 0x0013F3D8 File Offset: 0x0013D5D8
	public void Reset(ulong playerId)
	{
		this._cache.Remove(playerId);
		this._persistence.ResetPlayerState(playerId);
	}

	// Token: 0x060033DC RID: 13276 RVA: 0x0013F3F2 File Offset: 0x0013D5F2
	private void OnPlayerStateLoaded(PlayerState state)
	{
		state.unHostileTimestamp = Math.Min(state.unHostileTimestamp, TimeEx.currentTimestamp + 1800.0);
	}
}
