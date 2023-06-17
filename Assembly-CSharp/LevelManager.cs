using System;
using System.Collections;
using Network;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200054C RID: 1356
public static class LevelManager
{
	// Token: 0x04002223 RID: 8739
	public static string CurrentLevelName;

	// Token: 0x1700038A RID: 906
	// (get) Token: 0x060029E5 RID: 10725 RVA: 0x000FFD9C File Offset: 0x000FDF9C
	public static bool isLoaded
	{
		get
		{
			return LevelManager.CurrentLevelName != null && !(LevelManager.CurrentLevelName == "") && !(LevelManager.CurrentLevelName == "Empty") && !(LevelManager.CurrentLevelName == "MenuBackground");
		}
	}

	// Token: 0x060029E6 RID: 10726 RVA: 0x000FFDEC File Offset: 0x000FDFEC
	public static bool IsValid(string strName)
	{
		return Application.CanStreamedLevelBeLoaded(strName);
	}

	// Token: 0x060029E7 RID: 10727 RVA: 0x000FFDF4 File Offset: 0x000FDFF4
	public static void LoadLevel(string strName, bool keepLoadingScreenOpen = true)
	{
		if (strName == "proceduralmap")
		{
			strName = "Procedural Map";
		}
		LevelManager.CurrentLevelName = strName;
		Net.sv.Reset();
		SceneManager.LoadScene(strName, LoadSceneMode.Single);
	}

	// Token: 0x060029E8 RID: 10728 RVA: 0x000FFE21 File Offset: 0x000FE021
	public static IEnumerator LoadLevelAsync(string strName, bool keepLoadingScreenOpen = true)
	{
		if (strName == "proceduralmap")
		{
			strName = "Procedural Map";
		}
		LevelManager.CurrentLevelName = strName;
		Net.sv.Reset();
		yield return null;
		yield return SceneManager.LoadSceneAsync(strName, LoadSceneMode.Single);
		yield return null;
		yield return null;
		yield break;
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x000FFE30 File Offset: 0x000FE030
	public static void UnloadLevel(bool loadingScreen = true)
	{
		LevelManager.CurrentLevelName = null;
		SceneManager.LoadScene("Empty");
	}
}
