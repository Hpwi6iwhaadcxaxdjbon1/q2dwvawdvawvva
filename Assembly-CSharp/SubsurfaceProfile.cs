using System;
using Rust;
using UnityEngine;

// Token: 0x0200071C RID: 1820
public class SubsurfaceProfile : ScriptableObject
{
	// Token: 0x04002983 RID: 10627
	private static SubsurfaceProfileTexture profileTexture = new SubsurfaceProfileTexture();

	// Token: 0x04002984 RID: 10628
	public SubsurfaceProfileData Data = SubsurfaceProfileData.Default;

	// Token: 0x04002985 RID: 10629
	private int id = -1;

	// Token: 0x17000433 RID: 1075
	// (get) Token: 0x06003310 RID: 13072 RVA: 0x0013A44A File Offset: 0x0013864A
	public static Texture2D Texture
	{
		get
		{
			if (SubsurfaceProfile.profileTexture == null)
			{
				return null;
			}
			return SubsurfaceProfile.profileTexture.Texture;
		}
	}

	// Token: 0x17000434 RID: 1076
	// (get) Token: 0x06003311 RID: 13073 RVA: 0x0013A45F File Offset: 0x0013865F
	public int Id
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x06003312 RID: 13074 RVA: 0x0013A467 File Offset: 0x00138667
	private void OnEnable()
	{
		this.id = SubsurfaceProfile.profileTexture.AddProfile(this.Data, this);
	}

	// Token: 0x06003313 RID: 13075 RVA: 0x0013A480 File Offset: 0x00138680
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		SubsurfaceProfile.profileTexture.RemoveProfile(this.id);
	}

	// Token: 0x06003314 RID: 13076 RVA: 0x0013A49A File Offset: 0x0013869A
	public void Update()
	{
		SubsurfaceProfile.profileTexture.UpdateProfile(this.id, this.Data);
	}
}
