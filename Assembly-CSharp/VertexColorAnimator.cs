using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009A4 RID: 2468
public class VertexColorAnimator : MonoBehaviour
{
	// Token: 0x0400354B RID: 13643
	public List<MeshHolder> animationMeshes;

	// Token: 0x0400354C RID: 13644
	public List<float> animationKeyframes;

	// Token: 0x0400354D RID: 13645
	public float timeScale = 2f;

	// Token: 0x0400354E RID: 13646
	public int mode;

	// Token: 0x0400354F RID: 13647
	private float elapsedTime;

	// Token: 0x06003ADC RID: 15068 RVA: 0x0015CF6C File Offset: 0x0015B16C
	public void initLists()
	{
		this.animationMeshes = new List<MeshHolder>();
		this.animationKeyframes = new List<float>();
	}

	// Token: 0x06003ADD RID: 15069 RVA: 0x0015CF84 File Offset: 0x0015B184
	public void addMesh(Mesh mesh, float atPosition)
	{
		MeshHolder meshHolder = new MeshHolder();
		meshHolder.setAnimationData(mesh);
		this.animationMeshes.Add(meshHolder);
		this.animationKeyframes.Add(atPosition);
	}

	// Token: 0x06003ADE RID: 15070 RVA: 0x0015CFB6 File Offset: 0x0015B1B6
	private void Start()
	{
		this.elapsedTime = 0f;
	}

	// Token: 0x06003ADF RID: 15071 RVA: 0x0015CFC3 File Offset: 0x0015B1C3
	public void replaceKeyframe(int frameIndex, Mesh mesh)
	{
		this.animationMeshes[frameIndex].setAnimationData(mesh);
	}

	// Token: 0x06003AE0 RID: 15072 RVA: 0x0015CFD7 File Offset: 0x0015B1D7
	public void deleteKeyframe(int frameIndex)
	{
		this.animationMeshes.RemoveAt(frameIndex);
		this.animationKeyframes.RemoveAt(frameIndex);
	}

	// Token: 0x06003AE1 RID: 15073 RVA: 0x0015CFF4 File Offset: 0x0015B1F4
	public void scrobble(float scrobblePos)
	{
		if (this.animationMeshes.Count == 0)
		{
			return;
		}
		Color[] array = new Color[base.GetComponent<MeshFilter>().sharedMesh.colors.Length];
		int num = 0;
		for (int i = 0; i < this.animationKeyframes.Count; i++)
		{
			if (scrobblePos >= this.animationKeyframes[i])
			{
				num = i;
			}
		}
		if (num >= this.animationKeyframes.Count - 1)
		{
			base.GetComponent<VertexColorStream>().setColors(this.animationMeshes[num]._colors);
			return;
		}
		float num2 = this.animationKeyframes[num + 1] - this.animationKeyframes[num];
		float num3 = this.animationKeyframes[num];
		float t = (scrobblePos - num3) / num2;
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = Color.Lerp(this.animationMeshes[num]._colors[j], this.animationMeshes[num + 1]._colors[j], t);
		}
		base.GetComponent<VertexColorStream>().setColors(array);
	}

	// Token: 0x06003AE2 RID: 15074 RVA: 0x0015D114 File Offset: 0x0015B314
	private void Update()
	{
		if (this.mode == 0)
		{
			this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
		}
		else if (this.mode == 1)
		{
			this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
			if (this.elapsedTime > 1f)
			{
				this.elapsedTime = 0f;
			}
		}
		else if (this.mode == 2)
		{
			if (Mathf.FloorToInt(Time.fixedTime / this.timeScale) % 2 == 0)
			{
				this.elapsedTime += Time.fixedDeltaTime / this.timeScale;
			}
			else
			{
				this.elapsedTime -= Time.fixedDeltaTime / this.timeScale;
			}
		}
		Color[] array = new Color[base.GetComponent<MeshFilter>().sharedMesh.colors.Length];
		int num = 0;
		for (int i = 0; i < this.animationKeyframes.Count; i++)
		{
			if (this.elapsedTime >= this.animationKeyframes[i])
			{
				num = i;
			}
		}
		if (num < this.animationKeyframes.Count - 1)
		{
			float num2 = this.animationKeyframes[num + 1] - this.animationKeyframes[num];
			float num3 = this.animationKeyframes[num];
			float t = (this.elapsedTime - num3) / num2;
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = Color.Lerp(this.animationMeshes[num]._colors[j], this.animationMeshes[num + 1]._colors[j], t);
			}
		}
		else
		{
			array = this.animationMeshes[num]._colors;
		}
		base.GetComponent<VertexColorStream>().setColors(array);
	}
}
