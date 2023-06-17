using System;
using CompanionServer.Cameras;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers
{
	// Token: 0x020009F5 RID: 2549
	public class CameraInput : BaseHandler<AppCameraInput>
	{
		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06003D0B RID: 15627 RVA: 0x00166799 File Offset: 0x00164999
		protected override double TokenCost
		{
			get
			{
				return 0.01;
			}
		}

		// Token: 0x06003D0C RID: 15628 RVA: 0x001667A4 File Offset: 0x001649A4
		public override void Execute()
		{
			if (!CameraRenderer.enabled)
			{
				base.SendError("not_enabled");
				return;
			}
			if (base.Client.CurrentCamera == null || !base.Client.IsControllingCamera)
			{
				base.SendError("no_camera");
				return;
			}
			InputState inputState = base.Client.InputState;
			if (inputState == null)
			{
				inputState = new InputState();
				base.Client.InputState = inputState;
			}
			InputMessage inputMessage = Pool.Get<InputMessage>();
			inputMessage.buttons = base.Proto.buttons;
			inputMessage.mouseDelta = CameraInput.Sanitize(base.Proto.mouseDelta);
			inputMessage.aimAngles = Vector3.zero;
			inputState.Flip(inputMessage);
			Pool.Free<InputMessage>(ref inputMessage);
			base.Client.CurrentCamera.UserInput(inputState, new CameraViewerId(base.Client.ControllingSteamId, base.Client.ConnectionId));
			base.SendSuccess();
		}

		// Token: 0x06003D0D RID: 15629 RVA: 0x00166888 File Offset: 0x00164A88
		private static Vector3 Sanitize(Vector3 value)
		{
			return new Vector3(CameraInput.Sanitize(value.x), CameraInput.Sanitize(value.y), CameraInput.Sanitize(value.z));
		}

		// Token: 0x06003D0E RID: 15630 RVA: 0x001668B0 File Offset: 0x00164AB0
		private static float Sanitize(float value)
		{
			if (float.IsNaN(value) || float.IsInfinity(value))
			{
				return 0f;
			}
			return Mathf.Clamp(value, -100f, 100f);
		}
	}
}
