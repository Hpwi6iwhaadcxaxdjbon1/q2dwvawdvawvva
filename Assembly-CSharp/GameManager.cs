using System;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200050D RID: 1293
public class GameManager
{
	// Token: 0x04002152 RID: 8530
	public static GameManager server = new GameManager(false, true);

	// Token: 0x04002153 RID: 8531
	internal PrefabPreProcess preProcessed;

	// Token: 0x04002154 RID: 8532
	internal PrefabPoolCollection pool;

	// Token: 0x04002155 RID: 8533
	private bool Clientside;

	// Token: 0x04002156 RID: 8534
	private bool Serverside;

	// Token: 0x06002947 RID: 10567 RVA: 0x000FD3B1 File Offset: 0x000FB5B1
	public void Reset()
	{
		this.pool.Clear(null);
	}

	// Token: 0x06002948 RID: 10568 RVA: 0x000FD3BF File Offset: 0x000FB5BF
	public GameManager(bool clientside, bool serverside)
	{
		this.Clientside = clientside;
		this.Serverside = serverside;
		this.preProcessed = new PrefabPreProcess(clientside, serverside, false);
		this.pool = new PrefabPoolCollection();
	}

	// Token: 0x06002949 RID: 10569 RVA: 0x000FD3F0 File Offset: 0x000FB5F0
	public GameObject FindPrefab(uint prefabID)
	{
		string text = StringPool.Get(prefabID);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return this.FindPrefab(text);
	}

	// Token: 0x0600294A RID: 10570 RVA: 0x000FD415 File Offset: 0x000FB615
	public GameObject FindPrefab(BaseEntity ent)
	{
		if (ent == null)
		{
			return null;
		}
		return this.FindPrefab(ent.PrefabName);
	}

	// Token: 0x0600294B RID: 10571 RVA: 0x000FD430 File Offset: 0x000FB630
	public GameObject FindPrefab(string strPrefab)
	{
		GameObject gameObject = this.preProcessed.Find(strPrefab);
		if (gameObject != null)
		{
			return gameObject;
		}
		gameObject = FileSystem.LoadPrefab(strPrefab);
		if (gameObject == null)
		{
			return null;
		}
		this.preProcessed.Process(strPrefab, gameObject);
		GameObject gameObject2 = this.preProcessed.Find(strPrefab);
		if (!(gameObject2 != null))
		{
			return gameObject;
		}
		return gameObject2;
	}

	// Token: 0x0600294C RID: 10572 RVA: 0x000FD490 File Offset: 0x000FB690
	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, Vector3 scale, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
		if (gameObject)
		{
			gameObject.transform.localScale = scale;
			if (active)
			{
				gameObject.AwakeFromInstantiate();
			}
		}
		return gameObject;
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x000FD4C8 File Offset: 0x000FB6C8
	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
		if (gameObject && active)
		{
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x000FD4F4 File Offset: 0x000FB6F4
	public GameObject CreatePrefab(string strPrefab, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, Vector3.zero, Quaternion.identity);
		if (gameObject && active)
		{
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x000FD528 File Offset: 0x000FB728
	public GameObject CreatePrefab(string strPrefab, Transform parent, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, parent.position, parent.rotation);
		if (gameObject)
		{
			gameObject.transform.SetParent(parent, false);
			gameObject.Identity();
			if (active)
			{
				gameObject.AwakeFromInstantiate();
			}
		}
		return gameObject;
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x000FD570 File Offset: 0x000FB770
	public BaseEntity CreateEntity(string strPrefab, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion), bool startActive = true)
	{
		if (string.IsNullOrEmpty(strPrefab))
		{
			return null;
		}
		GameObject gameObject = this.CreatePrefab(strPrefab, pos, rot, startActive);
		if (gameObject == null)
		{
			return null;
		}
		BaseEntity component = gameObject.GetComponent<BaseEntity>();
		if (component == null)
		{
			Debug.LogError("CreateEntity called on a prefab that isn't an entity! " + strPrefab);
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		if (component.CompareTag("CannotBeCreated"))
		{
			Debug.LogWarning("CreateEntity called on a prefab that has the CannotBeCreated tag set. " + strPrefab);
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		return component;
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x000FD5EC File Offset: 0x000FB7EC
	private GameObject Instantiate(string strPrefab, Vector3 pos, Quaternion rot)
	{
		if (!strPrefab.IsLower())
		{
			Debug.LogWarning("Converting prefab name to lowercase: " + strPrefab);
			strPrefab = strPrefab.ToLower();
		}
		GameObject gameObject = this.FindPrefab(strPrefab);
		if (!gameObject)
		{
			Debug.LogError("Couldn't find prefab \"" + strPrefab + "\"");
			return null;
		}
		GameObject gameObject2 = this.pool.Pop(StringPool.Get(strPrefab), pos, rot);
		if (gameObject2 == null)
		{
			gameObject2 = Facepunch.Instantiate.GameObject(gameObject, pos, rot);
			gameObject2.name = strPrefab;
		}
		else
		{
			gameObject2.transform.localScale = gameObject.transform.localScale;
		}
		if (!this.Clientside && this.Serverside && gameObject2.transform.parent == null)
		{
			SceneManager.MoveGameObjectToScene(gameObject2, Rust.Server.EntityScene);
		}
		return gameObject2;
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x000FD6B4 File Offset: 0x000FB8B4
	public static void Destroy(Component component, float delay = 0f)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + component.name);
		}
		UnityEngine.Object.Destroy(component, delay);
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x000FD6DF File Offset: 0x000FB8DF
	public static void Destroy(GameObject instance, float delay = 0f)
	{
		if (!instance)
		{
			return;
		}
		if (instance.GetComponent<BaseEntity>().IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + instance.name);
		}
		UnityEngine.Object.Destroy(instance, delay);
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x000FD713 File Offset: 0x000FB913
	public static void DestroyImmediate(Component component, bool allowDestroyingAssets = false)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + component.name);
		}
		UnityEngine.Object.DestroyImmediate(component, allowDestroyingAssets);
	}

	// Token: 0x06002955 RID: 10581 RVA: 0x000FD73E File Offset: 0x000FB93E
	public static void DestroyImmediate(GameObject instance, bool allowDestroyingAssets = false)
	{
		if (instance.GetComponent<BaseEntity>().IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + instance.name);
		}
		UnityEngine.Object.DestroyImmediate(instance, allowDestroyingAssets);
	}

	// Token: 0x06002956 RID: 10582 RVA: 0x000FD76C File Offset: 0x000FB96C
	public void Retire(GameObject instance)
	{
		if (!instance)
		{
			return;
		}
		using (TimeWarning.New("GameManager.Retire", 0))
		{
			if (instance.GetComponent<BaseEntity>().IsValid())
			{
				Debug.LogError("Trying to retire an entity without killing it first: " + instance.name);
			}
			if (!Rust.Application.isQuitting && ConVar.Pool.enabled && instance.SupportsPooling())
			{
				this.pool.Push(instance);
			}
			else
			{
				UnityEngine.Object.Destroy(instance);
			}
		}
	}
}
