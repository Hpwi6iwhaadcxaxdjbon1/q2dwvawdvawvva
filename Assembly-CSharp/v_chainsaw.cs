using System;
using UnityEngine;

// Token: 0x020001CE RID: 462
public class v_chainsaw : MonoBehaviour
{
	// Token: 0x040011E7 RID: 4583
	public bool bAttacking;

	// Token: 0x040011E8 RID: 4584
	public bool bHitMetal;

	// Token: 0x040011E9 RID: 4585
	public bool bHitWood;

	// Token: 0x040011EA RID: 4586
	public bool bHitFlesh;

	// Token: 0x040011EB RID: 4587
	public bool bEngineOn;

	// Token: 0x040011EC RID: 4588
	public ParticleSystem[] hitMetalFX;

	// Token: 0x040011ED RID: 4589
	public ParticleSystem[] hitWoodFX;

	// Token: 0x040011EE RID: 4590
	public ParticleSystem[] hitFleshFX;

	// Token: 0x040011EF RID: 4591
	public SoundDefinition hitMetalSoundDef;

	// Token: 0x040011F0 RID: 4592
	public SoundDefinition hitWoodSoundDef;

	// Token: 0x040011F1 RID: 4593
	public SoundDefinition hitFleshSoundDef;

	// Token: 0x040011F2 RID: 4594
	public Sound hitSound;

	// Token: 0x040011F3 RID: 4595
	public GameObject hitSoundTarget;

	// Token: 0x040011F4 RID: 4596
	public float hitSoundFadeTime = 0.1f;

	// Token: 0x040011F5 RID: 4597
	public ParticleSystem smokeEffect;

	// Token: 0x040011F6 RID: 4598
	public Animator chainsawAnimator;

	// Token: 0x040011F7 RID: 4599
	public Renderer chainRenderer;

	// Token: 0x040011F8 RID: 4600
	public Material chainlink;

	// Token: 0x040011F9 RID: 4601
	private MaterialPropertyBlock block;

	// Token: 0x040011FA RID: 4602
	private Vector2 saveST;

	// Token: 0x040011FB RID: 4603
	private float chainSpeed;

	// Token: 0x040011FC RID: 4604
	private float chainAmount;

	// Token: 0x040011FD RID: 4605
	public float temp1;

	// Token: 0x040011FE RID: 4606
	public float temp2;

	// Token: 0x06001913 RID: 6419 RVA: 0x000B8ADE File Offset: 0x000B6CDE
	public void OnEnable()
	{
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		this.saveST = this.chainRenderer.sharedMaterial.GetVector("_MainTex_ST");
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x000B8B13 File Offset: 0x000B6D13
	private void Awake()
	{
		this.chainlink = this.chainRenderer.sharedMaterial;
	}

	// Token: 0x06001915 RID: 6421 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x06001916 RID: 6422 RVA: 0x000B8B28 File Offset: 0x000B6D28
	private void ScrollChainTexture()
	{
		float z = this.chainAmount = (this.chainAmount + Time.deltaTime * this.chainSpeed) % 1f;
		this.block.Clear();
		this.block.SetVector("_MainTex_ST", new Vector4(this.saveST.x, this.saveST.y, z, 0f));
		this.chainRenderer.SetPropertyBlock(this.block);
	}

	// Token: 0x06001917 RID: 6423 RVA: 0x000B8BA8 File Offset: 0x000B6DA8
	private void Update()
	{
		this.chainsawAnimator.SetBool("attacking", this.bAttacking);
		this.smokeEffect.enableEmission = this.bEngineOn;
		ParticleSystem[] array;
		if (this.bHitMetal)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			this.DoHitSound(this.hitMetalSoundDef);
			return;
		}
		if (this.bHitWood)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			this.DoHitSound(this.hitWoodSoundDef);
			return;
		}
		if (this.bHitFlesh)
		{
			this.chainsawAnimator.SetBool("attackHit", true);
			array = this.hitMetalFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitWoodFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = false;
			}
			array = this.hitFleshFX;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enableEmission = true;
			}
			this.DoHitSound(this.hitFleshSoundDef);
			return;
		}
		this.chainsawAnimator.SetBool("attackHit", false);
		array = this.hitMetalFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
		array = this.hitWoodFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
		array = this.hitFleshFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = false;
		}
	}

	// Token: 0x06001918 RID: 6424 RVA: 0x000063A5 File Offset: 0x000045A5
	private void DoHitSound(SoundDefinition soundDef)
	{
	}
}
