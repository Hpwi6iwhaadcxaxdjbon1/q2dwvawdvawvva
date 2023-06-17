using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Facepunch;
using Facepunch.Math;
using Facepunch.Rust;
using Facepunch.Sqlite;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000747 RID: 1863
public class UserPersistance : IDisposable
{
	// Token: 0x04002A3E RID: 10814
	private static Facepunch.Sqlite.Database blueprints;

	// Token: 0x04002A3F RID: 10815
	private static Facepunch.Sqlite.Database deaths;

	// Token: 0x04002A40 RID: 10816
	private static Facepunch.Sqlite.Database identities;

	// Token: 0x04002A41 RID: 10817
	private static Facepunch.Sqlite.Database tokens;

	// Token: 0x04002A42 RID: 10818
	private static Facepunch.Sqlite.Database playerState;

	// Token: 0x04002A43 RID: 10819
	private static Dictionary<ulong, string> nameCache;

	// Token: 0x04002A44 RID: 10820
	private static Dictionary<ulong, string> wipeIdCache;

	// Token: 0x04002A45 RID: 10821
	[TupleElementNames(new string[]
	{
		"Token",
		"Locked"
	})]
	private static MruDictionary<ulong, ValueTuple<int, bool>> tokenCache;

	// Token: 0x06003428 RID: 13352 RVA: 0x0014324C File Offset: 0x0014144C
	public UserPersistance(string strFolder)
	{
		UserPersistance.blueprints = new Facepunch.Sqlite.Database();
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		string arg = strFolder + "/player.blueprints.";
		if (activeGameMode != null && activeGameMode.wipeBpsOnProtocol)
		{
			arg = arg + 238 + ".";
		}
		UserPersistance.blueprints.Open(arg + 5 + ".db", false);
		if (!UserPersistance.blueprints.TableExists("data"))
		{
			UserPersistance.blueprints.Execute("CREATE TABLE data ( userid TEXT PRIMARY KEY, info BLOB, updated INTEGER )");
		}
		UserPersistance.deaths = new Facepunch.Sqlite.Database();
		UserPersistance.deaths.Open(string.Concat(new object[]
		{
			strFolder,
			"/player.deaths.",
			5,
			".db"
		}), false);
		if (!UserPersistance.deaths.TableExists("data"))
		{
			UserPersistance.deaths.Execute("CREATE TABLE data ( userid TEXT, born INTEGER, died INTEGER, info BLOB )");
			UserPersistance.deaths.Execute("CREATE INDEX IF NOT EXISTS userindex ON data ( userid )");
			UserPersistance.deaths.Execute("CREATE INDEX IF NOT EXISTS diedindex ON data ( died )");
		}
		UserPersistance.identities = new Facepunch.Sqlite.Database();
		UserPersistance.identities.Open(string.Concat(new object[]
		{
			strFolder,
			"/player.identities.",
			5,
			".db"
		}), false);
		if (!UserPersistance.identities.TableExists("data"))
		{
			UserPersistance.identities.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, username TEXT )");
		}
		UserPersistance.tokens = new Facepunch.Sqlite.Database();
		UserPersistance.tokens.Open(strFolder + "/player.tokens.db", false);
		if (!UserPersistance.tokens.TableExists("data"))
		{
			UserPersistance.tokens.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, token INT, locked BOOLEAN DEFAULT 0 )");
		}
		if (!UserPersistance.tokens.ColumnExists("data", "locked"))
		{
			UserPersistance.tokens.Execute("ALTER TABLE data ADD COLUMN locked BOOLEAN DEFAULT 0");
		}
		UserPersistance.playerState = new Facepunch.Sqlite.Database();
		UserPersistance.playerState.Open(string.Concat(new object[]
		{
			strFolder,
			"/player.states.",
			238,
			".db"
		}), false);
		if (!UserPersistance.playerState.TableExists("data"))
		{
			UserPersistance.playerState.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, state BLOB )");
		}
		UserPersistance.nameCache = new Dictionary<ulong, string>();
		UserPersistance.tokenCache = new MruDictionary<ulong, ValueTuple<int, bool>>(500, null);
		UserPersistance.wipeIdCache = new Dictionary<ulong, string>();
	}

	// Token: 0x06003429 RID: 13353 RVA: 0x001434A4 File Offset: 0x001416A4
	public virtual void Dispose()
	{
		if (UserPersistance.blueprints != null)
		{
			UserPersistance.blueprints.Close();
			UserPersistance.blueprints = null;
		}
		if (UserPersistance.deaths != null)
		{
			UserPersistance.deaths.Close();
			UserPersistance.deaths = null;
		}
		if (UserPersistance.identities != null)
		{
			UserPersistance.identities.Close();
			UserPersistance.identities = null;
		}
		if (UserPersistance.tokens != null)
		{
			UserPersistance.tokens.Close();
			UserPersistance.tokens = null;
		}
		if (UserPersistance.playerState != null)
		{
			UserPersistance.playerState.Close();
			UserPersistance.playerState = null;
		}
	}

	// Token: 0x0600342A RID: 13354 RVA: 0x00143524 File Offset: 0x00141724
	public PersistantPlayer GetPlayerInfo(ulong playerID)
	{
		PersistantPlayer persistantPlayer = this.FetchFromDatabase(playerID);
		if (persistantPlayer == null)
		{
			persistantPlayer = Pool.Get<PersistantPlayer>();
		}
		if (persistantPlayer.unlockedItems == null)
		{
			persistantPlayer.unlockedItems = Pool.GetList<int>();
		}
		return persistantPlayer;
	}

	// Token: 0x0600342B RID: 13355 RVA: 0x00143558 File Offset: 0x00141758
	private PersistantPlayer FetchFromDatabase(ulong playerID)
	{
		try
		{
			byte[] array = UserPersistance.blueprints.QueryBlob<string>("SELECT info FROM data WHERE userid = ?", playerID.ToString());
			if (array != null)
			{
				return PersistantPlayer.Deserialize(array);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error loading player blueprints: (" + ex.Message + ")");
		}
		return null;
	}

	// Token: 0x0600342C RID: 13356 RVA: 0x001435BC File Offset: 0x001417BC
	public void SetPlayerInfo(ulong playerID, PersistantPlayer info)
	{
		using (TimeWarning.New("SetPlayerInfo", 0))
		{
			byte[] arg;
			using (TimeWarning.New("ToProtoBytes", 0))
			{
				arg = info.ToProtoBytes();
			}
			UserPersistance.blueprints.Execute<string, byte[], int>("INSERT OR REPLACE INTO data ( userid, info, updated ) VALUES ( ?, ?, ? )", playerID.ToString(), arg, Epoch.Current);
		}
	}

	// Token: 0x0600342D RID: 13357 RVA: 0x00143638 File Offset: 0x00141838
	public void AddLifeStory(ulong playerID, PlayerLifeStory lifeStory)
	{
		if (UserPersistance.deaths == null)
		{
			return;
		}
		if (lifeStory == null)
		{
			return;
		}
		using (TimeWarning.New("AddLifeStory", 0))
		{
			byte[] arg;
			using (TimeWarning.New("ToProtoBytes", 0))
			{
				arg = lifeStory.ToProtoBytes();
			}
			UserPersistance.deaths.Execute<string, int, int, byte[]>("INSERT INTO data ( userid, born, died, info ) VALUES ( ?, ?, ?, ? )", playerID.ToString(), (int)lifeStory.timeBorn, (int)lifeStory.timeDied, arg);
		}
	}

	// Token: 0x0600342E RID: 13358 RVA: 0x001436C8 File Offset: 0x001418C8
	public PlayerLifeStory GetLastLifeStory(ulong playerID)
	{
		if (UserPersistance.deaths == null)
		{
			return null;
		}
		PlayerLifeStory result;
		using (TimeWarning.New("GetLastLifeStory", 0))
		{
			try
			{
				byte[] array = UserPersistance.deaths.QueryBlob<string>("SELECT info FROM data WHERE userid = ? ORDER BY died DESC LIMIT 1", playerID.ToString());
				if (array == null)
				{
					return null;
				}
				PlayerLifeStory playerLifeStory = PlayerLifeStory.Deserialize(array);
				playerLifeStory.ShouldPool = false;
				return playerLifeStory;
			}
			catch (Exception ex)
			{
				Debug.LogError("Error loading lifestory from database: (" + ex.Message + ")");
			}
			result = null;
		}
		return result;
	}

	// Token: 0x0600342F RID: 13359 RVA: 0x00143760 File Offset: 0x00141960
	public string GetPlayerName(ulong playerID)
	{
		if (playerID == 0UL)
		{
			return null;
		}
		string result;
		if (UserPersistance.nameCache.TryGetValue(playerID, out result))
		{
			return result;
		}
		string text = UserPersistance.identities.QueryString<ulong>("SELECT username FROM data WHERE userid = ?", playerID);
		UserPersistance.nameCache[playerID] = text;
		return text;
	}

	// Token: 0x06003430 RID: 13360 RVA: 0x001437A4 File Offset: 0x001419A4
	public void SetPlayerName(ulong playerID, string name)
	{
		if (playerID == 0UL || string.IsNullOrEmpty(name))
		{
			return;
		}
		if (string.IsNullOrEmpty(this.GetPlayerName(playerID)))
		{
			UserPersistance.identities.Execute<ulong, string>("INSERT INTO data ( userid, username ) VALUES ( ?, ? )", playerID, name);
		}
		else
		{
			UserPersistance.identities.Execute<string, ulong>("UPDATE data SET username = ? WHERE userid = ?", name, playerID);
		}
		UserPersistance.nameCache[playerID] = name;
	}

	// Token: 0x06003431 RID: 13361 RVA: 0x001437FC File Offset: 0x001419FC
	public int GetOrGenerateAppToken(ulong playerID, out bool locked)
	{
		if (UserPersistance.tokens == null)
		{
			locked = false;
			return 0;
		}
		int result;
		using (TimeWarning.New("GetOrGenerateAppToken", 0))
		{
			ValueTuple<int, bool> valueTuple;
			if (UserPersistance.tokenCache.TryGetValue(playerID, out valueTuple))
			{
				locked = valueTuple.Item2;
				result = valueTuple.Item1;
			}
			else
			{
				int num = UserPersistance.tokens.QueryInt<ulong>("SELECT token FROM data WHERE userid = ?", playerID);
				if (num != 0)
				{
					bool flag = UserPersistance.tokens.QueryInt<ulong>("SELECT locked FROM data WHERE userid = ?", playerID) != 0;
					UserPersistance.tokenCache.Add(playerID, new ValueTuple<int, bool>(num, flag));
					locked = flag;
					result = num;
				}
				else
				{
					int num2 = UserPersistance.GenerateAppToken();
					UserPersistance.tokens.Execute<ulong, int>("INSERT INTO data ( userid, token ) VALUES ( ?, ? )", playerID, num2);
					UserPersistance.tokenCache.Add(playerID, new ValueTuple<int, bool>(num2, false));
					locked = false;
					result = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x06003432 RID: 13362 RVA: 0x001438D8 File Offset: 0x00141AD8
	public void RegenerateAppToken(ulong playerID)
	{
		if (UserPersistance.tokens == null)
		{
			return;
		}
		using (TimeWarning.New("RegenerateAppToken", 0))
		{
			UserPersistance.tokenCache.Remove(playerID);
			bool arg = UserPersistance.tokens.QueryInt<ulong>("SELECT locked FROM data WHERE userid = ?", playerID) != 0;
			int num = UserPersistance.GenerateAppToken();
			UserPersistance.tokens.Execute<ulong, int, bool>("INSERT OR REPLACE INTO data ( userid, token, locked ) VALUES ( ?, ?, ? )", playerID, num, arg);
			UserPersistance.tokenCache.Add(playerID, new ValueTuple<int, bool>(num, false));
		}
	}

	// Token: 0x06003433 RID: 13363 RVA: 0x00143960 File Offset: 0x00141B60
	private static int GenerateAppToken()
	{
		int num = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		if (num == 0)
		{
			num++;
		}
		return num;
	}

	// Token: 0x06003434 RID: 13364 RVA: 0x00143988 File Offset: 0x00141B88
	public bool SetAppTokenLocked(ulong playerID, bool locked)
	{
		if (UserPersistance.tokens == null)
		{
			return false;
		}
		bool flag;
		this.GetOrGenerateAppToken(playerID, out flag);
		if (flag == locked)
		{
			return false;
		}
		UserPersistance.tokens.Execute<int, ulong>("UPDATE data SET locked = ? WHERE userid = ?", locked ? 1 : 0, playerID);
		UserPersistance.tokenCache.Remove(playerID);
		return true;
	}

	// Token: 0x06003435 RID: 13365 RVA: 0x001439D1 File Offset: 0x00141BD1
	public byte[] GetPlayerState(ulong playerID)
	{
		if (playerID == 0UL)
		{
			return null;
		}
		return UserPersistance.playerState.QueryBlob<ulong>("SELECT state FROM data WHERE userid = ?", playerID);
	}

	// Token: 0x06003436 RID: 13366 RVA: 0x001439E8 File Offset: 0x00141BE8
	public void SetPlayerState(ulong playerID, byte[] state)
	{
		if (playerID == 0UL || state == null)
		{
			return;
		}
		UserPersistance.playerState.Execute<ulong, byte[]>("INSERT OR REPLACE INTO data ( userid, state ) VALUES ( ?, ? )", playerID, state);
	}

	// Token: 0x06003437 RID: 13367 RVA: 0x00143A04 File Offset: 0x00141C04
	public string GetUserWipeId(ulong playerID)
	{
		if (playerID <= 10000000UL)
		{
			return null;
		}
		string text;
		if (UserPersistance.wipeIdCache.TryGetValue(playerID, out text))
		{
			return text;
		}
		text = (playerID.ToString() + SaveRestore.WipeId).Sha256().HexString();
		UserPersistance.wipeIdCache[playerID] = text;
		Analytics.Azure.OnPlayerInitializedWipeId(playerID, text);
		return text;
	}

	// Token: 0x06003438 RID: 13368 RVA: 0x00143A5D File Offset: 0x00141C5D
	public void ResetPlayerState(ulong playerID)
	{
		if (playerID == 0UL)
		{
			return;
		}
		UserPersistance.playerState.Execute<ulong>("DELETE FROM data WHERE userid = ?", playerID);
	}
}
