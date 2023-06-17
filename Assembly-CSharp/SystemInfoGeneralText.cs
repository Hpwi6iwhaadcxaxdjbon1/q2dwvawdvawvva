using System;
using System.Text;
using Rust;
using TMPro;
using UnityEngine;

// Token: 0x02000313 RID: 787
public class SystemInfoGeneralText : MonoBehaviour
{
	// Token: 0x040017C1 RID: 6081
	public TextMeshProUGUI text;

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x06001EBC RID: 7868 RVA: 0x000D140C File Offset: 0x000CF60C
	public static string currentInfo
	{
		get
		{
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(false);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("System");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tName: ");
			stringBuilder.Append(SystemInfo.deviceName);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tOS:   ");
			stringBuilder.Append(SystemInfo.operatingSystem);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("CPU");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tModel:  ");
			stringBuilder.Append(SystemInfo.processorType);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tCores:  ");
			stringBuilder.Append(SystemInfo.processorCount);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory: ");
			stringBuilder.Append(SystemInfo.systemMemorySize);
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("GPU");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tModel:  ");
			stringBuilder.Append(SystemInfo.graphicsDeviceName);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tAPI:    ");
			stringBuilder.Append(SystemInfo.graphicsDeviceVersion);
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory: ");
			stringBuilder.Append(SystemInfo.graphicsMemorySize);
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tSM:     ");
			stringBuilder.Append(SystemInfo.graphicsShaderLevel);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("Process");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory:   ");
			stringBuilder.Append(SystemInfoEx.systemMemoryUsed);
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("Mono");
			stringBuilder.AppendLine();
			stringBuilder.Append("\tCollects: ");
			stringBuilder.Append(Rust.GC.CollectionCount());
			stringBuilder.AppendLine();
			stringBuilder.Append("\tMemory:   ");
			stringBuilder.Append(Rust.GC.GetTotalMemory());
			stringBuilder.Append(" MB");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			if (World.Seed > 0U && World.Size > 0U)
			{
				stringBuilder.Append("World");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tSeed:        ");
				if (activeGameMode != null && !activeGameMode.ingameMap)
				{
					stringBuilder.Append("?");
				}
				else
				{
					stringBuilder.Append(World.Seed);
				}
				stringBuilder.AppendLine();
				stringBuilder.Append("\tSize:        ");
				stringBuilder.Append(SystemInfoGeneralText.KM2(World.Size));
				stringBuilder.Append(" km²");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tHeightmap:   ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tWatermap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.WaterMap ? TerrainMeta.WaterMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tSplatmap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.SplatMap ? TerrainMeta.SplatMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tBiomemap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tTopologymap: ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.TopologyMap ? TerrainMeta.TopologyMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
				stringBuilder.Append("\tAlphamap:    ");
				stringBuilder.Append(SystemInfoGeneralText.MB(TerrainMeta.AlphaMap ? TerrainMeta.AlphaMap.GetMemoryUsage() : 0L));
				stringBuilder.Append(" MB");
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine();
			if (!string.IsNullOrEmpty(World.Checksum))
			{
				stringBuilder.AppendLine("Checksum");
				stringBuilder.Append('\t');
				stringBuilder.AppendLine(World.Checksum);
			}
			return stringBuilder.ToString();
		}
	}

	// Token: 0x06001EBD RID: 7869 RVA: 0x000D18BD File Offset: 0x000CFABD
	protected void Update()
	{
		this.text.text = SystemInfoGeneralText.currentInfo;
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x000D18CF File Offset: 0x000CFACF
	private static long MB(long bytes)
	{
		return bytes / 1048576L;
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x000D18D9 File Offset: 0x000CFAD9
	private static long MB(ulong bytes)
	{
		return SystemInfoGeneralText.MB((long)bytes);
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x000D18E1 File Offset: 0x000CFAE1
	private static int KM2(float meters)
	{
		return Mathf.RoundToInt(meters * meters * 1E-06f);
	}
}
