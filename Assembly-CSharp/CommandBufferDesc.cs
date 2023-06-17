using System;
using UnityEngine.Rendering;

// Token: 0x02000710 RID: 1808
public class CommandBufferDesc
{
	// Token: 0x1700042E RID: 1070
	// (get) Token: 0x060032F5 RID: 13045 RVA: 0x00139B6C File Offset: 0x00137D6C
	// (set) Token: 0x060032F6 RID: 13046 RVA: 0x00139B74 File Offset: 0x00137D74
	public CameraEvent CameraEvent { get; private set; }

	// Token: 0x1700042F RID: 1071
	// (get) Token: 0x060032F7 RID: 13047 RVA: 0x00139B7D File Offset: 0x00137D7D
	// (set) Token: 0x060032F8 RID: 13048 RVA: 0x00139B85 File Offset: 0x00137D85
	public int OrderId { get; private set; }

	// Token: 0x17000430 RID: 1072
	// (get) Token: 0x060032F9 RID: 13049 RVA: 0x00139B8E File Offset: 0x00137D8E
	// (set) Token: 0x060032FA RID: 13050 RVA: 0x00139B96 File Offset: 0x00137D96
	public Action<CommandBuffer> FillDelegate { get; private set; }

	// Token: 0x060032FB RID: 13051 RVA: 0x00139B9F File Offset: 0x00137D9F
	public CommandBufferDesc(CameraEvent cameraEvent, int orderId, CommandBufferDesc.FillCommandBuffer fill)
	{
		this.CameraEvent = cameraEvent;
		this.OrderId = orderId;
		this.FillDelegate = new Action<CommandBuffer>(fill.Invoke);
	}

	// Token: 0x02000E38 RID: 3640
	// (Invoke) Token: 0x06005242 RID: 21058
	public delegate void FillCommandBuffer(CommandBuffer cb);
}
