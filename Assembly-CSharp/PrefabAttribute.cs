using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000912 RID: 2322
public abstract class PrefabAttribute : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x0400331E RID: 13086
	[NonSerialized]
	public Vector3 worldPosition;

	// Token: 0x0400331F RID: 13087
	[NonSerialized]
	public Quaternion worldRotation;

	// Token: 0x04003320 RID: 13088
	[NonSerialized]
	public Vector3 worldForward;

	// Token: 0x04003321 RID: 13089
	[NonSerialized]
	public Vector3 localPosition;

	// Token: 0x04003322 RID: 13090
	[NonSerialized]
	public Vector3 localScale;

	// Token: 0x04003323 RID: 13091
	[NonSerialized]
	public Quaternion localRotation;

	// Token: 0x04003324 RID: 13092
	[NonSerialized]
	public string fullName;

	// Token: 0x04003325 RID: 13093
	[NonSerialized]
	public string hierachyName;

	// Token: 0x04003326 RID: 13094
	[NonSerialized]
	public uint prefabID;

	// Token: 0x04003327 RID: 13095
	[NonSerialized]
	public int instanceID;

	// Token: 0x04003328 RID: 13096
	[NonSerialized]
	public PrefabAttribute.Library prefabAttribute;

	// Token: 0x04003329 RID: 13097
	[NonSerialized]
	public GameManager gameManager;

	// Token: 0x0400332A RID: 13098
	[NonSerialized]
	public bool isServer;

	// Token: 0x0400332B RID: 13099
	public static PrefabAttribute.Library server = new PrefabAttribute.Library(false, true);

	// Token: 0x17000476 RID: 1142
	// (get) Token: 0x06003817 RID: 14359 RVA: 0x0014EF5A File Offset: 0x0014D15A
	public bool isClient
	{
		get
		{
			return !this.isServer;
		}
	}

	// Token: 0x06003818 RID: 14360 RVA: 0x0014EF68 File Offset: 0x0014D168
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		this.fullName = name;
		this.hierachyName = base.transform.GetRecursiveName("");
		this.prefabID = StringPool.Get(name);
		this.instanceID = base.GetInstanceID();
		this.worldPosition = base.transform.position;
		this.worldRotation = base.transform.rotation;
		this.worldForward = base.transform.forward;
		this.localPosition = base.transform.localPosition;
		this.localScale = base.transform.localScale;
		this.localRotation = base.transform.localRotation;
		if (serverside)
		{
			this.prefabAttribute = PrefabAttribute.server;
			this.gameManager = GameManager.server;
			this.isServer = true;
		}
		this.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			PrefabAttribute.server.Add(this.prefabID, this);
		}
		preProcess.RemoveComponent(this);
		preProcess.NominateForDeletion(base.gameObject);
	}

	// Token: 0x06003819 RID: 14361 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}

	// Token: 0x0600381A RID: 14362
	protected abstract Type GetIndexedType();

	// Token: 0x0600381B RID: 14363 RVA: 0x0014F06C File Offset: 0x0014D26C
	public static bool operator ==(PrefabAttribute x, PrefabAttribute y)
	{
		return PrefabAttribute.ComparePrefabAttribute(x, y);
	}

	// Token: 0x0600381C RID: 14364 RVA: 0x0014F075 File Offset: 0x0014D275
	public static bool operator !=(PrefabAttribute x, PrefabAttribute y)
	{
		return !PrefabAttribute.ComparePrefabAttribute(x, y);
	}

	// Token: 0x0600381D RID: 14365 RVA: 0x0014F084 File Offset: 0x0014D284
	public override bool Equals(object o)
	{
		PrefabAttribute y;
		return (y = (o as PrefabAttribute)) != null && PrefabAttribute.ComparePrefabAttribute(this, y);
	}

	// Token: 0x0600381E RID: 14366 RVA: 0x0014F0A4 File Offset: 0x0014D2A4
	public override int GetHashCode()
	{
		if (this.hierachyName == null)
		{
			return base.GetHashCode();
		}
		return this.hierachyName.GetHashCode();
	}

	// Token: 0x0600381F RID: 14367 RVA: 0x0014EA14 File Offset: 0x0014CC14
	public static implicit operator bool(PrefabAttribute exists)
	{
		return exists != null;
	}

	// Token: 0x06003820 RID: 14368 RVA: 0x0014F0C0 File Offset: 0x0014D2C0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool ComparePrefabAttribute(PrefabAttribute x, PrefabAttribute y)
	{
		bool flag = x == null;
		bool flag2 = y == null;
		return (flag && flag2) || (!flag && !flag2 && x.instanceID == y.instanceID);
	}

	// Token: 0x06003821 RID: 14369 RVA: 0x0014F0F6 File Offset: 0x0014D2F6
	public override string ToString()
	{
		if (this == null)
		{
			return "null";
		}
		return this.hierachyName;
	}

	// Token: 0x02000EB8 RID: 3768
	public class AttributeCollection
	{
		// Token: 0x04004CBA RID: 19642
		private Dictionary<Type, List<PrefabAttribute>> attributes = new Dictionary<Type, List<PrefabAttribute>>();

		// Token: 0x04004CBB RID: 19643
		private Dictionary<Type, object> cache = new Dictionary<Type, object>();

		// Token: 0x06005324 RID: 21284 RVA: 0x001B1B80 File Offset: 0x001AFD80
		internal List<PrefabAttribute> Find(Type t)
		{
			List<PrefabAttribute> list;
			if (this.attributes.TryGetValue(t, out list))
			{
				return list;
			}
			list = new List<PrefabAttribute>();
			this.attributes.Add(t, list);
			return list;
		}

		// Token: 0x06005325 RID: 21285 RVA: 0x001B1BB4 File Offset: 0x001AFDB4
		public T[] Find<T>()
		{
			if (this.cache == null)
			{
				this.cache = new Dictionary<Type, object>();
			}
			object obj;
			if (this.cache.TryGetValue(typeof(T), out obj))
			{
				return (T[])obj;
			}
			obj = this.Find(typeof(T)).Cast<T>().ToArray<T>();
			this.cache.Add(typeof(T), obj);
			return (T[])obj;
		}

		// Token: 0x06005326 RID: 21286 RVA: 0x001B1C2B File Offset: 0x001AFE2B
		public void Add(PrefabAttribute attribute)
		{
			List<PrefabAttribute> list = this.Find(attribute.GetIndexedType());
			Assert.IsTrue(!list.Contains(attribute), "AttributeCollection.Add: Adding twice to list");
			list.Add(attribute);
			this.cache = null;
		}
	}

	// Token: 0x02000EB9 RID: 3769
	public class Library
	{
		// Token: 0x04004CBC RID: 19644
		public bool clientside;

		// Token: 0x04004CBD RID: 19645
		public bool serverside;

		// Token: 0x04004CBE RID: 19646
		private Dictionary<uint, PrefabAttribute.AttributeCollection> prefabs = new Dictionary<uint, PrefabAttribute.AttributeCollection>();

		// Token: 0x06005328 RID: 21288 RVA: 0x001B1C78 File Offset: 0x001AFE78
		public Library(bool clientside, bool serverside)
		{
			this.clientside = clientside;
			this.serverside = serverside;
		}

		// Token: 0x06005329 RID: 21289 RVA: 0x001B1C9C File Offset: 0x001AFE9C
		public PrefabAttribute.AttributeCollection Find(uint prefabID, bool warmup = true)
		{
			PrefabAttribute.AttributeCollection attributeCollection;
			if (this.prefabs.TryGetValue(prefabID, out attributeCollection))
			{
				return attributeCollection;
			}
			attributeCollection = new PrefabAttribute.AttributeCollection();
			this.prefabs.Add(prefabID, attributeCollection);
			if (warmup && (!this.clientside || this.serverside))
			{
				if (!this.clientside && this.serverside)
				{
					GameManager.server.FindPrefab(prefabID);
				}
				else if (this.clientside)
				{
					bool flag = this.serverside;
				}
			}
			return attributeCollection;
		}

		// Token: 0x0600532A RID: 21290 RVA: 0x001B1D10 File Offset: 0x001AFF10
		public T Find<T>(uint prefabID) where T : PrefabAttribute
		{
			T[] array = this.Find(prefabID, true).Find<T>();
			if (array.Length == 0)
			{
				return default(T);
			}
			return array[0];
		}

		// Token: 0x0600532B RID: 21291 RVA: 0x001B1D40 File Offset: 0x001AFF40
		public T[] FindAll<T>(uint prefabID) where T : PrefabAttribute
		{
			return this.Find(prefabID, true).Find<T>();
		}

		// Token: 0x0600532C RID: 21292 RVA: 0x001B1D4F File Offset: 0x001AFF4F
		public void Add(uint prefabID, PrefabAttribute attribute)
		{
			this.Find(prefabID, false).Add(attribute);
		}

		// Token: 0x0600532D RID: 21293 RVA: 0x001B1D5F File Offset: 0x001AFF5F
		public void Invalidate(uint prefabID)
		{
			this.prefabs.Remove(prefabID);
		}
	}
}
