using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000799 RID: 1945
public class ChildrenFromScene : MonoBehaviour
{
	// Token: 0x04002B3B RID: 11067
	public string SceneName;

	// Token: 0x04002B3C RID: 11068
	public bool StartChildrenDisabled;

	// Token: 0x060034FA RID: 13562 RVA: 0x001463A2 File Offset: 0x001445A2
	private IEnumerator Start()
	{
		Debug.LogWarning("WARNING: CHILDRENFROMSCENE(" + this.SceneName + ") - WE SHOULDN'T BE USING THIS SHITTY COMPONENT NOW WE HAVE AWESOME PREFABS", base.gameObject);
		if (!SceneManager.GetSceneByName(this.SceneName).isLoaded)
		{
			yield return SceneManager.LoadSceneAsync(this.SceneName, LoadSceneMode.Additive);
		}
		Scene sceneByName = SceneManager.GetSceneByName(this.SceneName);
		foreach (GameObject gameObject in sceneByName.GetRootGameObjects())
		{
			gameObject.transform.SetParent(base.transform, false);
			gameObject.Identity();
			RectTransform rectTransform = gameObject.transform as RectTransform;
			if (rectTransform)
			{
				rectTransform.pivot = Vector2.zero;
				rectTransform.anchoredPosition = Vector2.zero;
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.sizeDelta = Vector2.one;
			}
			SingletonComponent[] componentsInChildren = gameObject.GetComponentsInChildren<SingletonComponent>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].SingletonSetup();
			}
			if (this.StartChildrenDisabled)
			{
				gameObject.SetActive(false);
			}
		}
		SceneManager.UnloadSceneAsync(sceneByName);
		yield break;
	}
}
