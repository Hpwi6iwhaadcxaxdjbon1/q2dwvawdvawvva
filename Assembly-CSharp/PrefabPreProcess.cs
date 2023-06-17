using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using VLB;

// Token: 0x02000557 RID: 1367
public class PrefabPreProcess : IPrefabProcessor
{
	// Token: 0x04002254 RID: 8788
	public static Type[] clientsideOnlyTypes = new Type[]
	{
		typeof(IClientComponent),
		typeof(SkeletonSkinLod),
		typeof(ImageEffectLayer),
		typeof(NGSS_Directional),
		typeof(VolumetricDustParticles),
		typeof(VolumetricLightBeam),
		typeof(Cloth),
		typeof(MeshFilter),
		typeof(Renderer),
		typeof(AudioLowPassFilter),
		typeof(AudioSource),
		typeof(AudioListener),
		typeof(ParticleSystemRenderer),
		typeof(ParticleSystem),
		typeof(ParticleEmitFromParentObject),
		typeof(ImpostorShadows),
		typeof(Light),
		typeof(LODGroup),
		typeof(Animator),
		typeof(AnimationEvents),
		typeof(PlayerVoiceSpeaker),
		typeof(VoiceProcessor),
		typeof(PlayerVoiceRecorder),
		typeof(ParticleScaler),
		typeof(PostEffectsBase),
		typeof(TOD_ImageEffect),
		typeof(TOD_Scattering),
		typeof(TOD_Rays),
		typeof(Tree),
		typeof(Projector),
		typeof(HttpImage),
		typeof(EventTrigger),
		typeof(StandaloneInputModule),
		typeof(UIBehaviour),
		typeof(Canvas),
		typeof(CanvasRenderer),
		typeof(CanvasGroup),
		typeof(GraphicRaycaster)
	};

	// Token: 0x04002255 RID: 8789
	public static Type[] serversideOnlyTypes = new Type[]
	{
		typeof(IServerComponent),
		typeof(NavMeshObstacle)
	};

	// Token: 0x04002256 RID: 8790
	public bool isClientside;

	// Token: 0x04002257 RID: 8791
	public bool isServerside;

	// Token: 0x04002258 RID: 8792
	public bool isBundling;

	// Token: 0x04002259 RID: 8793
	internal Dictionary<string, GameObject> prefabList = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x0400225A RID: 8794
	private List<Component> destroyList = new List<Component>();

	// Token: 0x0400225B RID: 8795
	private List<GameObject> cleanupList = new List<GameObject>();

	// Token: 0x06002A22 RID: 10786 RVA: 0x00100930 File Offset: 0x000FEB30
	public PrefabPreProcess(bool clientside, bool serverside, bool bundling = false)
	{
		this.isClientside = clientside;
		this.isServerside = serverside;
		this.isBundling = bundling;
	}

	// Token: 0x06002A23 RID: 10787 RVA: 0x00100980 File Offset: 0x000FEB80
	public GameObject Find(string strPrefab)
	{
		GameObject gameObject;
		if (!this.prefabList.TryGetValue(strPrefab, out gameObject))
		{
			return null;
		}
		if (gameObject == null)
		{
			this.prefabList.Remove(strPrefab);
			return null;
		}
		return gameObject;
	}

	// Token: 0x06002A24 RID: 10788 RVA: 0x001009B8 File Offset: 0x000FEBB8
	public bool NeedsProcessing(GameObject go)
	{
		if (go.CompareTag("NoPreProcessing"))
		{
			return false;
		}
		if (this.HasComponents<IPrefabPreProcess>(go.transform))
		{
			return true;
		}
		if (this.HasComponents<IPrefabPostProcess>(go.transform))
		{
			return true;
		}
		if (this.HasComponents<IEditorComponent>(go.transform))
		{
			return true;
		}
		if (!this.isClientside)
		{
			if (PrefabPreProcess.clientsideOnlyTypes.Any((Type type) => this.HasComponents(go.transform, type)))
			{
				return true;
			}
			if (this.HasComponents<IClientComponentEx>(go.transform))
			{
				return true;
			}
		}
		if (!this.isServerside)
		{
			if (PrefabPreProcess.serversideOnlyTypes.Any((Type type) => this.HasComponents(go.transform, type)))
			{
				return true;
			}
			if (this.HasComponents<IServerComponentEx>(go.transform))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002A25 RID: 10789 RVA: 0x00100A9C File Offset: 0x000FEC9C
	public void ProcessObject(string name, GameObject go, bool resetLocalTransform = true)
	{
		if (!this.isClientside)
		{
			foreach (Type t in PrefabPreProcess.clientsideOnlyTypes)
			{
				this.DestroyComponents(t, go, this.isClientside, this.isServerside);
			}
			foreach (IClientComponentEx clientComponentEx in this.FindComponents<IClientComponentEx>(go.transform))
			{
				clientComponentEx.PreClientComponentCull(this);
			}
		}
		if (!this.isServerside)
		{
			foreach (Type t2 in PrefabPreProcess.serversideOnlyTypes)
			{
				this.DestroyComponents(t2, go, this.isClientside, this.isServerside);
			}
			foreach (IServerComponentEx serverComponentEx in this.FindComponents<IServerComponentEx>(go.transform))
			{
				serverComponentEx.PreServerComponentCull(this);
			}
		}
		this.DestroyComponents(typeof(IEditorComponent), go, this.isClientside, this.isServerside);
		if (resetLocalTransform)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
		}
		List<Transform> list = this.FindComponents<Transform>(go.transform);
		list.Reverse();
		MeshColliderCookingOptions meshColliderCookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices;
		MeshColliderCookingOptions cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase;
		MeshColliderCookingOptions meshColliderCookingOptions2 = (MeshColliderCookingOptions)(-1);
		foreach (MeshCollider meshCollider in this.FindComponents<MeshCollider>(go.transform))
		{
			if (meshCollider.cookingOptions == meshColliderCookingOptions || meshCollider.cookingOptions == meshColliderCookingOptions2)
			{
				meshCollider.cookingOptions = cookingOptions;
			}
		}
		foreach (IPrefabPreProcess prefabPreProcess in this.FindComponents<IPrefabPreProcess>(go.transform))
		{
			prefabPreProcess.PreProcess(this, go, name, this.isServerside, this.isClientside, this.isBundling);
		}
		foreach (Transform transform in list)
		{
			if (transform && transform.gameObject)
			{
				if (this.isServerside && transform.gameObject.CompareTag("Server Cull"))
				{
					this.RemoveComponents(transform.gameObject);
					this.NominateForDeletion(transform.gameObject);
				}
				if (this.isClientside)
				{
					bool flag = transform.gameObject.CompareTag("Client Cull");
					bool flag2 = transform != go.transform && transform.gameObject.GetComponent<BaseEntity>() != null;
					if (flag || flag2)
					{
						this.RemoveComponents(transform.gameObject);
						this.NominateForDeletion(transform.gameObject);
					}
				}
			}
		}
		this.RunCleanupQueue();
		foreach (IPrefabPostProcess prefabPostProcess in this.FindComponents<IPrefabPostProcess>(go.transform))
		{
			prefabPostProcess.PostProcess(this, go, name, this.isServerside, this.isClientside, this.isBundling);
		}
	}

	// Token: 0x06002A26 RID: 10790 RVA: 0x00100E1C File Offset: 0x000FF01C
	public void Process(string name, GameObject go)
	{
		if (!UnityEngine.Application.isPlaying)
		{
			return;
		}
		if (go.CompareTag("NoPreProcessing"))
		{
			return;
		}
		GameObject hierarchyGroup = this.GetHierarchyGroup();
		GameObject gameObject = go;
		go = Instantiate.GameObject(gameObject, hierarchyGroup.transform);
		go.name = gameObject.name;
		if (this.NeedsProcessing(go))
		{
			this.ProcessObject(name, go, true);
		}
		this.AddPrefab(name, go);
	}

	// Token: 0x06002A27 RID: 10791 RVA: 0x00100E7C File Offset: 0x000FF07C
	public void Invalidate(string name)
	{
		GameObject gameObject;
		if (this.prefabList.TryGetValue(name, out gameObject))
		{
			this.prefabList.Remove(name);
			if (gameObject != null)
			{
				UnityEngine.Object.DestroyImmediate(gameObject, true);
			}
		}
	}

	// Token: 0x06002A28 RID: 10792 RVA: 0x00100EB6 File Offset: 0x000FF0B6
	public GameObject GetHierarchyGroup()
	{
		if (this.isClientside && this.isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Generic", false, true);
		}
		if (this.isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Server", false, true);
		}
		return HierarchyUtil.GetRoot("PrefabPreProcess - Client", false, true);
	}

	// Token: 0x06002A29 RID: 10793 RVA: 0x00100EF6 File Offset: 0x000FF0F6
	public void AddPrefab(string name, GameObject go)
	{
		go.SetActive(false);
		this.prefabList.Add(name, go);
	}

	// Token: 0x06002A2A RID: 10794 RVA: 0x00100F0C File Offset: 0x000FF10C
	private void DestroyComponents(Type t, GameObject go, bool client, bool server)
	{
		List<Component> list = new List<Component>();
		this.FindComponents(go.transform, list, t);
		list.Reverse();
		foreach (Component component in list)
		{
			RealmedRemove component2 = component.GetComponent<RealmedRemove>();
			if (!(component2 != null) || component2.ShouldDelete(component, client, server))
			{
				if (!component.gameObject.CompareTag("persist"))
				{
					this.NominateForDeletion(component.gameObject);
				}
				UnityEngine.Object.DestroyImmediate(component, true);
			}
		}
	}

	// Token: 0x06002A2B RID: 10795 RVA: 0x00100FB0 File Offset: 0x000FF1B0
	private bool ShouldExclude(Transform transform)
	{
		return transform.GetComponent<BaseEntity>() != null;
	}

	// Token: 0x06002A2C RID: 10796 RVA: 0x00100FC4 File Offset: 0x000FF1C4
	private bool HasComponents<T>(Transform transform)
	{
		if (transform.GetComponent<T>() != null)
		{
			return true;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2) && this.HasComponents<T>(transform2))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002A2D RID: 10797 RVA: 0x0010103C File Offset: 0x000FF23C
	private bool HasComponents(Transform transform, Type t)
	{
		if (transform.GetComponent(t) != null)
		{
			return true;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2) && this.HasComponents(transform2, t))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002A2E RID: 10798 RVA: 0x001010B4 File Offset: 0x000FF2B4
	public List<T> FindComponents<T>(Transform transform)
	{
		List<T> list = new List<T>();
		this.FindComponents<T>(transform, list);
		return list;
	}

	// Token: 0x06002A2F RID: 10799 RVA: 0x001010D0 File Offset: 0x000FF2D0
	public void FindComponents<T>(Transform transform, List<T> list)
	{
		list.AddRange(transform.GetComponents<T>());
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2))
			{
				this.FindComponents<T>(transform2, list);
			}
		}
	}

	// Token: 0x06002A30 RID: 10800 RVA: 0x0010113C File Offset: 0x000FF33C
	public List<Component> FindComponents(Transform transform, Type t)
	{
		List<Component> list = new List<Component>();
		this.FindComponents(transform, list, t);
		return list;
	}

	// Token: 0x06002A31 RID: 10801 RVA: 0x0010115C File Offset: 0x000FF35C
	public void FindComponents(Transform transform, List<Component> list, Type t)
	{
		list.AddRange(transform.GetComponents(t));
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2))
			{
				this.FindComponents(transform2, list, t);
			}
		}
	}

	// Token: 0x06002A32 RID: 10802 RVA: 0x001011C8 File Offset: 0x000FF3C8
	public void RemoveComponent(Component c)
	{
		if (c == null)
		{
			return;
		}
		this.destroyList.Add(c);
	}

	// Token: 0x06002A33 RID: 10803 RVA: 0x001011E0 File Offset: 0x000FF3E0
	public void RemoveComponents(GameObject gameObj)
	{
		foreach (Component component in gameObj.GetComponents<Component>())
		{
			if (!(component is Transform))
			{
				this.destroyList.Add(component);
			}
		}
	}

	// Token: 0x06002A34 RID: 10804 RVA: 0x0010121A File Offset: 0x000FF41A
	public void NominateForDeletion(GameObject gameObj)
	{
		this.cleanupList.Add(gameObj);
	}

	// Token: 0x06002A35 RID: 10805 RVA: 0x00101228 File Offset: 0x000FF428
	private void RunCleanupQueue()
	{
		foreach (Component obj in this.destroyList)
		{
			UnityEngine.Object.DestroyImmediate(obj, true);
		}
		this.destroyList.Clear();
		foreach (GameObject go in this.cleanupList)
		{
			this.DoCleanup(go);
		}
		this.cleanupList.Clear();
	}

	// Token: 0x06002A36 RID: 10806 RVA: 0x001012D4 File Offset: 0x000FF4D4
	private void DoCleanup(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		if (go.GetComponentsInChildren<Component>(true).Length > 1)
		{
			return;
		}
		Transform parent = go.transform.parent;
		if (parent == null)
		{
			return;
		}
		if (parent.name.StartsWith("PrefabPreProcess - "))
		{
			return;
		}
		UnityEngine.Object.DestroyImmediate(go, true);
	}
}
