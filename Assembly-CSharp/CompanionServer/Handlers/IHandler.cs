using System;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009FB RID: 2555
	public interface IHandler : Pool.IPooled
	{
		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06003D18 RID: 15640
		AppRequest Request { get; }

		// Token: 0x06003D19 RID: 15641
		ValidationResult Validate();

		// Token: 0x06003D1A RID: 15642
		void Execute();

		// Token: 0x06003D1B RID: 15643
		void SendError(string code);
	}
}
