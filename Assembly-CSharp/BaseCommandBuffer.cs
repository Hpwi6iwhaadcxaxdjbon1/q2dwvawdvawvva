using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020008EA RID: 2282
public class BaseCommandBuffer : MonoBehaviour
{
	// Token: 0x040032AC RID: 12972
	private Dictionary<Camera, Dictionary<int, CommandBuffer>> cameras = new Dictionary<Camera, Dictionary<int, CommandBuffer>>();

	// Token: 0x060037A5 RID: 14245 RVA: 0x0014DBAC File Offset: 0x0014BDAC
	protected CommandBuffer GetCommandBuffer(string name, Camera camera, CameraEvent cameraEvent)
	{
		Dictionary<int, CommandBuffer> dictionary;
		if (!this.cameras.TryGetValue(camera, out dictionary))
		{
			dictionary = new Dictionary<int, CommandBuffer>();
			this.cameras.Add(camera, dictionary);
		}
		CommandBuffer commandBuffer;
		if (dictionary.TryGetValue((int)cameraEvent, out commandBuffer))
		{
			commandBuffer.Clear();
		}
		else
		{
			commandBuffer = new CommandBuffer();
			commandBuffer.name = name;
			dictionary.Add((int)cameraEvent, commandBuffer);
			this.CleanupCamera(name, camera, cameraEvent);
			camera.AddCommandBuffer(cameraEvent, commandBuffer);
		}
		return commandBuffer;
	}

	// Token: 0x060037A6 RID: 14246 RVA: 0x0014DC18 File Offset: 0x0014BE18
	protected void CleanupCamera(string name, Camera camera, CameraEvent cameraEvent)
	{
		foreach (CommandBuffer commandBuffer in camera.GetCommandBuffers(cameraEvent))
		{
			if (commandBuffer.name == name)
			{
				camera.RemoveCommandBuffer(cameraEvent, commandBuffer);
			}
		}
	}

	// Token: 0x060037A7 RID: 14247 RVA: 0x0014DC58 File Offset: 0x0014BE58
	protected void CleanupCommandBuffer(Camera camera, CameraEvent cameraEvent)
	{
		Dictionary<int, CommandBuffer> dictionary;
		if (!this.cameras.TryGetValue(camera, out dictionary))
		{
			return;
		}
		CommandBuffer buffer;
		if (!dictionary.TryGetValue((int)cameraEvent, out buffer))
		{
			return;
		}
		camera.RemoveCommandBuffer(cameraEvent, buffer);
	}

	// Token: 0x060037A8 RID: 14248 RVA: 0x0014DC8C File Offset: 0x0014BE8C
	protected void Cleanup()
	{
		foreach (KeyValuePair<Camera, Dictionary<int, CommandBuffer>> keyValuePair in this.cameras)
		{
			Camera key = keyValuePair.Key;
			Dictionary<int, CommandBuffer> value = keyValuePair.Value;
			if (key)
			{
				foreach (KeyValuePair<int, CommandBuffer> keyValuePair2 in value)
				{
					int key2 = keyValuePair2.Key;
					CommandBuffer value2 = keyValuePair2.Value;
					key.RemoveCommandBuffer((CameraEvent)key2, value2);
				}
			}
		}
	}
}
