using System;
using UnityEngine;

// Token: 0x020006FF RID: 1791
[ExecuteInEditMode]
public class WaterInteraction : MonoBehaviour
{
	// Token: 0x0400290B RID: 10507
	[SerializeField]
	private Texture2D texture;

	// Token: 0x0400290C RID: 10508
	[Range(0f, 1f)]
	public float Displacement = 1f;

	// Token: 0x0400290D RID: 10509
	[Range(0f, 1f)]
	public float Disturbance = 0.5f;

	// Token: 0x04002912 RID: 10514
	private Transform cachedTransform;

	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x0600328A RID: 12938 RVA: 0x001378A5 File Offset: 0x00135AA5
	// (set) Token: 0x0600328B RID: 12939 RVA: 0x001378AD File Offset: 0x00135AAD
	public Texture2D Texture
	{
		get
		{
			return this.texture;
		}
		set
		{
			this.texture = value;
			this.CheckRegister();
		}
	}

	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x0600328C RID: 12940 RVA: 0x001378BC File Offset: 0x00135ABC
	// (set) Token: 0x0600328D RID: 12941 RVA: 0x001378C4 File Offset: 0x00135AC4
	public WaterDynamics.Image Image { get; private set; }

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x0600328E RID: 12942 RVA: 0x001378CD File Offset: 0x00135ACD
	// (set) Token: 0x0600328F RID: 12943 RVA: 0x001378D5 File Offset: 0x00135AD5
	public Vector2 Position { get; private set; } = Vector2.zero;

	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x06003290 RID: 12944 RVA: 0x001378DE File Offset: 0x00135ADE
	// (set) Token: 0x06003291 RID: 12945 RVA: 0x001378E6 File Offset: 0x00135AE6
	public Vector2 Scale { get; private set; } = Vector2.one;

	// Token: 0x17000419 RID: 1049
	// (get) Token: 0x06003292 RID: 12946 RVA: 0x001378EF File Offset: 0x00135AEF
	// (set) Token: 0x06003293 RID: 12947 RVA: 0x001378F7 File Offset: 0x00135AF7
	public float Rotation { get; private set; }

	// Token: 0x06003294 RID: 12948 RVA: 0x00137900 File Offset: 0x00135B00
	protected void OnEnable()
	{
		this.CheckRegister();
		this.UpdateTransform();
	}

	// Token: 0x06003295 RID: 12949 RVA: 0x0013790E File Offset: 0x00135B0E
	protected void OnDisable()
	{
		this.Unregister();
	}

	// Token: 0x06003296 RID: 12950 RVA: 0x00137918 File Offset: 0x00135B18
	public void CheckRegister()
	{
		if (!base.enabled || this.texture == null)
		{
			this.Unregister();
			return;
		}
		if (this.Image == null || this.Image.texture != this.texture)
		{
			this.Register();
		}
	}

	// Token: 0x06003297 RID: 12951 RVA: 0x00137968 File Offset: 0x00135B68
	private void UpdateImage()
	{
		this.Image = new WaterDynamics.Image(this.texture);
	}

	// Token: 0x06003298 RID: 12952 RVA: 0x0013797B File Offset: 0x00135B7B
	private void Register()
	{
		this.UpdateImage();
		WaterDynamics.RegisterInteraction(this);
	}

	// Token: 0x06003299 RID: 12953 RVA: 0x00137989 File Offset: 0x00135B89
	private void Unregister()
	{
		if (this.Image != null)
		{
			WaterDynamics.UnregisterInteraction(this);
			this.Image = null;
		}
	}

	// Token: 0x0600329A RID: 12954 RVA: 0x001379A0 File Offset: 0x00135BA0
	public void UpdateTransform()
	{
		this.cachedTransform = ((this.cachedTransform != null) ? this.cachedTransform : base.transform);
		if (this.cachedTransform.hasChanged)
		{
			Vector3 position = this.cachedTransform.position;
			Vector3 lossyScale = this.cachedTransform.lossyScale;
			this.Position = new Vector2(position.x, position.z);
			this.Scale = new Vector2(lossyScale.x, lossyScale.z);
			this.Rotation = this.cachedTransform.rotation.eulerAngles.y;
			this.cachedTransform.hasChanged = false;
		}
	}
}
