using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020004F0 RID: 1264
public class CreationGibSpawner : BaseMonoBehaviour
{
	// Token: 0x040020DE RID: 8414
	private GameObject gibSource;

	// Token: 0x040020DF RID: 8415
	public GameObject gibsInstance;

	// Token: 0x040020E0 RID: 8416
	public float startTime;

	// Token: 0x040020E1 RID: 8417
	public float duration = 1f;

	// Token: 0x040020E2 RID: 8418
	public float buildScaleAdditionalAmount = 0.5f;

	// Token: 0x040020E3 RID: 8419
	[Tooltip("Entire object will be scaled on xyz during duration by this curve")]
	public AnimationCurve scaleCurve;

	// Token: 0x040020E4 RID: 8420
	[Tooltip("Object will be pushed out along transform.forward/right/up based on build direction by this amount")]
	public AnimationCurve buildCurve;

	// Token: 0x040020E5 RID: 8421
	[Tooltip("Additional scaling to apply to object based on build direction")]
	public AnimationCurve buildScaleCurve;

	// Token: 0x040020E6 RID: 8422
	public AnimationCurve xCurve;

	// Token: 0x040020E7 RID: 8423
	public AnimationCurve yCurve;

	// Token: 0x040020E8 RID: 8424
	public AnimationCurve zCurve;

	// Token: 0x040020E9 RID: 8425
	public Vector3[] spawnPositions;

	// Token: 0x040020EA RID: 8426
	public GameObject[] particles;

	// Token: 0x040020EB RID: 8427
	public float[] gibProgress;

	// Token: 0x040020EC RID: 8428
	public PhysicMaterial physMaterial;

	// Token: 0x040020ED RID: 8429
	public List<Transform> gibs;

	// Token: 0x040020EE RID: 8430
	public bool started;

	// Token: 0x040020EF RID: 8431
	public GameObjectRef placeEffect;

	// Token: 0x040020F0 RID: 8432
	public GameObject smokeEffect;

	// Token: 0x040020F1 RID: 8433
	public float effectSpacing = 0.2f;

	// Token: 0x040020F2 RID: 8434
	public bool invert;

	// Token: 0x040020F3 RID: 8435
	public Vector3 buildDirection;

	// Token: 0x040020F4 RID: 8436
	[Horizontal(1, 0)]
	public CreationGibSpawner.GibReplacement[] GibReplacements;

	// Token: 0x040020F5 RID: 8437
	public CreationGibSpawner.EffectMaterialPair[] effectLookup;

	// Token: 0x040020F6 RID: 8438
	private float startDelay;

	// Token: 0x040020F7 RID: 8439
	public List<CreationGibSpawner.ConditionalGibSource> conditionalGibSources = new List<CreationGibSpawner.ConditionalGibSource>();

	// Token: 0x040020F8 RID: 8440
	private float nextEffectTime = float.NegativeInfinity;

	// Token: 0x060028DF RID: 10463 RVA: 0x000FB9FC File Offset: 0x000F9BFC
	public GameObjectRef GetEffectForMaterial(PhysicMaterial mat)
	{
		foreach (CreationGibSpawner.EffectMaterialPair effectMaterialPair in this.effectLookup)
		{
			if (effectMaterialPair.material == mat)
			{
				return effectMaterialPair.effect;
			}
		}
		return this.effectLookup[0].effect;
	}

	// Token: 0x060028E0 RID: 10464 RVA: 0x000FBA44 File Offset: 0x000F9C44
	public void SetDelay(float newDelay)
	{
		this.startDelay = newDelay;
	}

	// Token: 0x060028E1 RID: 10465 RVA: 0x000FBA4D File Offset: 0x000F9C4D
	public void FinishSpawn()
	{
		if (this.startDelay == 0f)
		{
			this.Init();
			return;
		}
		base.Invoke(new Action(this.Init), this.startDelay);
	}

	// Token: 0x060028E2 RID: 10466 RVA: 0x000FBA7B File Offset: 0x000F9C7B
	public float GetProgress(float delay)
	{
		if (!this.started)
		{
			return 0f;
		}
		if (this.duration == 0f)
		{
			return 1f;
		}
		return Mathf.Clamp01((Time.time - (this.startTime + delay)) / this.duration);
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x000FBAB8 File Offset: 0x000F9CB8
	public void AddConditionalGibSource(GameObject cGibSource, Vector3 pos, Quaternion rot)
	{
		Debug.Log("Adding conditional gib source");
		CreationGibSpawner.ConditionalGibSource item;
		item.source = cGibSource;
		item.pos = pos;
		item.rot = rot;
		this.conditionalGibSources.Add(item);
	}

	// Token: 0x060028E4 RID: 10468 RVA: 0x000FBAF4 File Offset: 0x000F9CF4
	public void SetGibSource(GameObject newGibSource)
	{
		GameObject gameObject = newGibSource;
		for (int i = 0; i < this.GibReplacements.Length; i++)
		{
			if (this.GibReplacements[i].oldGib == newGibSource)
			{
				gameObject = this.GibReplacements[i].newGib;
				break;
			}
		}
		this.gibSource = gameObject;
	}

	// Token: 0x060028E5 RID: 10469 RVA: 0x000FBB44 File Offset: 0x000F9D44
	private int SortsGibs(Transform a, Transform b)
	{
		MeshRenderer component = a.GetComponent<MeshRenderer>();
		MeshRenderer component2 = b.GetComponent<MeshRenderer>();
		if (!this.invert)
		{
			float num = (component == null) ? a.localPosition.y : component.bounds.center.y;
			float value = (component2 == null) ? b.localPosition.y : component2.bounds.center.y;
			return num.CompareTo(value);
		}
		float value2 = (component == null) ? a.localPosition.y : component.bounds.center.y;
		return ((component2 == null) ? b.localPosition.y : component2.bounds.center.y).CompareTo(value2);
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x000FBC28 File Offset: 0x000F9E28
	public void Init()
	{
		this.started = true;
		this.startTime = Time.time;
		this.gibsInstance = UnityEngine.Object.Instantiate<GameObject>(this.gibSource, base.transform.position, base.transform.rotation);
		List<Transform> list = this.gibsInstance.GetComponentsInChildren<Transform>().ToList<Transform>();
		list.Remove(this.gibsInstance.transform);
		list.Sort(new Comparison<Transform>(this.SortsGibs));
		this.gibs = list;
		this.spawnPositions = new Vector3[this.gibs.Count];
		this.gibProgress = new float[this.gibs.Count];
		this.particles = new GameObject[this.gibs.Count];
		for (int i = 0; i < this.gibs.Count; i++)
		{
			Transform transform = this.gibs[i];
			this.spawnPositions[i] = transform.localPosition;
			this.gibProgress[i] = 0f;
			this.particles[i] = null;
			transform.localScale = Vector3.one * this.scaleCurve.Evaluate(0f);
			float x = this.spawnPositions[i].x;
			transform.transform.position += base.transform.right * this.GetPushDir(this.spawnPositions[i], transform) * this.buildCurve.Evaluate(0f) * this.buildDirection.x;
			transform.transform.position += base.transform.up * this.yCurve.Evaluate(0f);
			transform.transform.position += base.transform.forward * this.zCurve.Evaluate(0f);
		}
		base.Invoke(new Action(this.DestroyMe), this.duration + 0.05f);
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x000FBE5D File Offset: 0x000FA05D
	public float GetPushDir(Vector3 spawnPos, Transform theGib)
	{
		if (spawnPos.x < 0f)
		{
			return 1f;
		}
		return -1f;
	}

	// Token: 0x060028E8 RID: 10472 RVA: 0x000FBE77 File Offset: 0x000FA077
	public void DestroyMe()
	{
		UnityEngine.Object.Destroy(this.gibsInstance);
	}

	// Token: 0x060028E9 RID: 10473 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public float GetStartDelay(Transform gib)
	{
		return 0f;
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x000FBE84 File Offset: 0x000FA084
	public void Update()
	{
		if (!this.started)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		int num = Mathf.CeilToInt((float)this.gibs.Count / 10f);
		for (int i = 0; i < this.gibs.Count; i++)
		{
			Transform transform = this.gibs[i];
			if (!(transform == base.transform))
			{
				if (deltaTime <= 0f)
				{
					break;
				}
				float num2 = 0.33f;
				float num3 = num2 / ((float)this.gibs.Count * num2) * (this.duration - num2);
				float num4 = (float)i * num3;
				if (Time.time - this.startTime >= num4)
				{
					MeshFilter component = transform.GetComponent<MeshFilter>();
					int seed = UnityEngine.Random.seed;
					UnityEngine.Random.seed = i + this.gibs.Count;
					bool flag = num <= 1 || UnityEngine.Random.Range(0, num) == 0;
					UnityEngine.Random.seed = seed;
					if (flag && this.particles[i] == null && component != null && component.sharedMesh != null)
					{
						if (component.sharedMesh.bounds.size.magnitude == 0f)
						{
							goto IL_3B8;
						}
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.smokeEffect);
						gameObject.transform.SetParent(transform);
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localScale = Vector3.one;
						gameObject.transform.localRotation = Quaternion.identity;
						ParticleSystem component2 = gameObject.GetComponent<ParticleSystem>();
						MeshRenderer component3 = component.GetComponent<MeshRenderer>();
						ParticleSystem.ShapeModule shape = component2.shape;
						shape.shapeType = ParticleSystemShapeType.Box;
						shape.boxThickness = component3.bounds.extents;
						this.particles[i] = gameObject;
					}
					float num5 = Mathf.Clamp01(this.gibProgress[i] / num2);
					float num6 = Mathf.Clamp01((num5 + Time.deltaTime) / num2);
					this.gibProgress[i] = this.gibProgress[i] + Time.deltaTime;
					float num7 = this.scaleCurve.Evaluate(num6);
					transform.transform.localScale = new Vector3(num7, num7, num7);
					transform.transform.localScale += this.buildDirection * this.buildScaleCurve.Evaluate(num6) * this.buildScaleAdditionalAmount;
					transform.transform.localPosition = this.spawnPositions[i];
					transform.transform.position += base.transform.right * this.GetPushDir(this.spawnPositions[i], transform) * this.buildCurve.Evaluate(num6) * this.buildDirection.x;
					transform.transform.position += base.transform.up * this.buildCurve.Evaluate(num6) * this.buildDirection.y;
					transform.transform.position += base.transform.forward * this.buildCurve.Evaluate(num6) * this.buildDirection.z;
					if (num6 >= 1f && num6 > num5 && Time.time > this.nextEffectTime)
					{
						this.nextEffectTime = Time.time + this.effectSpacing;
						if (this.particles[i] != null)
						{
							this.particles[i].GetComponent<ParticleSystem>();
							this.particles[i].transform.SetParent(null, true);
							this.particles[i].BroadcastOnParentDestroying();
						}
					}
				}
			}
			IL_3B8:;
		}
	}

	// Token: 0x02000D2D RID: 3373
	[Serializable]
	public class GibReplacement
	{
		// Token: 0x04004670 RID: 18032
		public GameObject oldGib;

		// Token: 0x04004671 RID: 18033
		public GameObject newGib;
	}

	// Token: 0x02000D2E RID: 3374
	[Serializable]
	public class EffectMaterialPair
	{
		// Token: 0x04004672 RID: 18034
		public PhysicMaterial material;

		// Token: 0x04004673 RID: 18035
		public GameObjectRef effect;
	}

	// Token: 0x02000D2F RID: 3375
	[Serializable]
	public struct ConditionalGibSource
	{
		// Token: 0x04004674 RID: 18036
		public GameObject source;

		// Token: 0x04004675 RID: 18037
		public Vector3 pos;

		// Token: 0x04004676 RID: 18038
		public Quaternion rot;
	}
}
