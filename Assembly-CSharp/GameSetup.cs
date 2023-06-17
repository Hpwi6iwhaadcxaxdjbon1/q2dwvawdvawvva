using System;
using System.Collections;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000516 RID: 1302
public class GameSetup : MonoBehaviour
{
	// Token: 0x0400217C RID: 8572
	public static bool RunOnce;

	// Token: 0x0400217D RID: 8573
	public bool startServer = true;

	// Token: 0x0400217E RID: 8574
	public string clientConnectCommand = "client.connect 127.0.0.1:28015";

	// Token: 0x0400217F RID: 8575
	public bool loadMenu = true;

	// Token: 0x04002180 RID: 8576
	public bool loadLevel;

	// Token: 0x04002181 RID: 8577
	public string loadLevelScene = "";

	// Token: 0x04002182 RID: 8578
	public bool loadSave;

	// Token: 0x04002183 RID: 8579
	public string loadSaveFile = "";

	// Token: 0x04002184 RID: 8580
	public string initializationCommands = "";

	// Token: 0x0600298C RID: 10636 RVA: 0x000FEAE4 File Offset: 0x000FCCE4
	protected void Awake()
	{
		if (GameSetup.RunOnce)
		{
			GameManager.Destroy(base.gameObject, 0f);
			return;
		}
		GameManifest.Load();
		GameManifest.LoadAssets();
		GameSetup.RunOnce = true;
		if (Bootstrap.needsSetup)
		{
			Bootstrap.Init_Tier0();
			Bootstrap.Init_Systems();
			Bootstrap.Init_Config();
		}
		if (this.initializationCommands.Length > 0)
		{
			foreach (string text in this.initializationCommands.Split(new char[]
			{
				';'
			}))
			{
				ConsoleSystem.Run(ConsoleSystem.Option.Server, text.Trim(), Array.Empty<object>());
			}
		}
		base.StartCoroutine(this.DoGameSetup());
	}

	// Token: 0x0600298D RID: 10637 RVA: 0x000FEB89 File Offset: 0x000FCD89
	private IEnumerator DoGameSetup()
	{
		Rust.Application.isLoading = true;
		TerrainMeta.InitNoTerrain(false);
		ItemManager.Initialize();
		LevelManager.CurrentLevelName = SceneManager.GetActiveScene().name;
		if (this.loadLevel && !string.IsNullOrEmpty(this.loadLevelScene))
		{
			Network.Net.sv.Reset();
			ConVar.Server.level = this.loadLevelScene;
			LoadingScreen.Update("LOADING SCENE");
			UnityEngine.Application.LoadLevelAdditive(this.loadLevelScene);
			LoadingScreen.Update(this.loadLevelScene.ToUpper() + " LOADED");
		}
		if (this.startServer)
		{
			yield return base.StartCoroutine(this.StartServer());
		}
		yield return null;
		Rust.Application.isLoading = false;
		yield break;
	}

	// Token: 0x0600298E RID: 10638 RVA: 0x000FEB98 File Offset: 0x000FCD98
	private IEnumerator StartServer()
	{
		ConVar.GC.collect();
		ConVar.GC.unload();
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return base.StartCoroutine(Bootstrap.StartServer(this.loadSave, this.loadSaveFile, true));
		yield break;
	}
}
