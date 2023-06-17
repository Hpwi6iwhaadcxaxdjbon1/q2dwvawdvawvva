using System;
using System.Collections.Generic;
using System.IO;
using Facepunch.Utility;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;

namespace Facepunch.Unity
{
	// Token: 0x02000AFE RID: 2814
	public static class RenderInfo
	{
		// Token: 0x060044B0 RID: 17584 RVA: 0x00193454 File Offset: 0x00191654
		public static void GenerateReport()
		{
			Renderer[] array = UnityEngine.Object.FindObjectsOfType<Renderer>();
			List<RenderInfo.RendererInstance> list = new List<RenderInfo.RendererInstance>();
			Renderer[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RenderInfo.RendererInstance item = RenderInfo.RendererInstance.From(array2[i]);
				list.Add(item);
			}
			string text = string.Format(Application.dataPath + "/../RenderInfo-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
			string contents = JsonConvert.SerializeObject(list, Formatting.Indented);
			File.WriteAllText(text, contents);
			string text2 = Application.streamingAssetsPath + "/RenderInfo.exe";
			string text3 = "\"" + text + "\"";
			Debug.Log("Launching " + text2 + " " + text3);
			Os.StartProcess(text2, text3);
		}

		// Token: 0x02000F83 RID: 3971
		public struct RendererInstance
		{
			// Token: 0x04004FFA RID: 20474
			public bool IsVisible;

			// Token: 0x04004FFB RID: 20475
			public bool CastShadows;

			// Token: 0x04004FFC RID: 20476
			public bool Enabled;

			// Token: 0x04004FFD RID: 20477
			public bool RecieveShadows;

			// Token: 0x04004FFE RID: 20478
			public float Size;

			// Token: 0x04004FFF RID: 20479
			public float Distance;

			// Token: 0x04005000 RID: 20480
			public int BoneCount;

			// Token: 0x04005001 RID: 20481
			public int MaterialCount;

			// Token: 0x04005002 RID: 20482
			public int VertexCount;

			// Token: 0x04005003 RID: 20483
			public int TriangleCount;

			// Token: 0x04005004 RID: 20484
			public int SubMeshCount;

			// Token: 0x04005005 RID: 20485
			public int BlendShapeCount;

			// Token: 0x04005006 RID: 20486
			public string RenderType;

			// Token: 0x04005007 RID: 20487
			public string MeshName;

			// Token: 0x04005008 RID: 20488
			public string ObjectName;

			// Token: 0x04005009 RID: 20489
			public string EntityName;

			// Token: 0x0400500A RID: 20490
			public ulong EntityId;

			// Token: 0x0400500B RID: 20491
			public bool UpdateWhenOffscreen;

			// Token: 0x0400500C RID: 20492
			public int ParticleCount;

			// Token: 0x060054DD RID: 21725 RVA: 0x001B65A4 File Offset: 0x001B47A4
			public static RenderInfo.RendererInstance From(Renderer renderer)
			{
				RenderInfo.RendererInstance result = default(RenderInfo.RendererInstance);
				result.IsVisible = renderer.isVisible;
				result.CastShadows = (renderer.shadowCastingMode > ShadowCastingMode.Off);
				result.RecieveShadows = renderer.receiveShadows;
				result.Enabled = (renderer.enabled && renderer.gameObject.activeInHierarchy);
				result.Size = renderer.bounds.size.magnitude;
				result.Distance = Vector3.Distance(renderer.bounds.center, Camera.main.transform.position);
				result.MaterialCount = renderer.sharedMaterials.Length;
				result.RenderType = renderer.GetType().Name;
				BaseEntity baseEntity = renderer.gameObject.ToBaseEntity();
				if (baseEntity)
				{
					result.EntityName = baseEntity.PrefabName;
					if (baseEntity.net != null)
					{
						result.EntityId = baseEntity.net.ID.Value;
					}
				}
				else
				{
					result.ObjectName = renderer.transform.GetRecursiveName("");
				}
				if (renderer is MeshRenderer)
				{
					result.BoneCount = 0;
					MeshFilter component = renderer.GetComponent<MeshFilter>();
					if (component)
					{
						result.ReadMesh(component.sharedMesh);
					}
				}
				if (renderer is SkinnedMeshRenderer)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
					result.ReadMesh(skinnedMeshRenderer.sharedMesh);
					result.UpdateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
				}
				if (renderer is ParticleSystemRenderer)
				{
					ParticleSystem component2 = renderer.GetComponent<ParticleSystem>();
					if (component2)
					{
						result.MeshName = component2.name;
						result.ParticleCount = component2.particleCount;
					}
				}
				return result;
			}

			// Token: 0x060054DE RID: 21726 RVA: 0x001B6754 File Offset: 0x001B4954
			public void ReadMesh(UnityEngine.Mesh mesh)
			{
				if (mesh == null)
				{
					this.MeshName = "<NULL>";
					return;
				}
				this.VertexCount = mesh.vertexCount;
				this.SubMeshCount = mesh.subMeshCount;
				this.BlendShapeCount = mesh.blendShapeCount;
				this.MeshName = mesh.name;
			}
		}
	}
}
