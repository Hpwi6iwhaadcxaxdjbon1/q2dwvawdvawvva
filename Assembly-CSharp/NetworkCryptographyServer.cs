using System;
using Network;

// Token: 0x02000620 RID: 1568
public class NetworkCryptographyServer : NetworkCryptography
{
	// Token: 0x06002E2C RID: 11820 RVA: 0x00115025 File Offset: 0x00113225
	protected override void EncryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		if (connection.encryptionLevel > 1U)
		{
			EACServer.Encrypt(connection, src, ref dst);
			return;
		}
		Craptography.XOR(2392U, src, ref dst);
	}

	// Token: 0x06002E2D RID: 11821 RVA: 0x00115045 File Offset: 0x00113245
	protected override void DecryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		if (connection.encryptionLevel > 1U)
		{
			EACServer.Decrypt(connection, src, ref dst);
			return;
		}
		Craptography.XOR(2392U, src, ref dst);
	}
}
