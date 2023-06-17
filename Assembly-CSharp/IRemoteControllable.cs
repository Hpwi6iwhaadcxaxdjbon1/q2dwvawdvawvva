using System;
using UnityEngine;

// Token: 0x02000101 RID: 257
public interface IRemoteControllable
{
	// Token: 0x06001589 RID: 5513
	Transform GetEyes();

	// Token: 0x0600158A RID: 5514
	float GetFovScale();

	// Token: 0x0600158B RID: 5515
	BaseEntity GetEnt();

	// Token: 0x0600158C RID: 5516
	string GetIdentifier();

	// Token: 0x0600158D RID: 5517
	float Health();

	// Token: 0x0600158E RID: 5518
	float MaxHealth();

	// Token: 0x0600158F RID: 5519
	void UpdateIdentifier(string newID, bool clientSend = false);

	// Token: 0x06001590 RID: 5520
	void RCSetup();

	// Token: 0x06001591 RID: 5521
	void RCShutdown();

	// Token: 0x06001592 RID: 5522
	bool CanControl(ulong playerID);

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x06001593 RID: 5523
	bool RequiresMouse { get; }

	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x06001594 RID: 5524
	float MaxRange { get; }

	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x06001595 RID: 5525
	RemoteControllableControls RequiredControls { get; }

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x06001596 RID: 5526
	CameraViewerId? ControllingViewerId { get; }

	// Token: 0x06001597 RID: 5527
	void UserInput(InputState inputState, CameraViewerId viewerID);

	// Token: 0x06001598 RID: 5528
	bool InitializeControl(CameraViewerId viewerID);

	// Token: 0x06001599 RID: 5529
	void StopControl(CameraViewerId viewerID);

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x0600159A RID: 5530
	bool CanPing { get; }
}
