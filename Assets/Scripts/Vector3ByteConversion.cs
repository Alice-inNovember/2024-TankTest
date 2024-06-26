using System;
using UnityEngine;

public class Vector3ByteConversion : MonoBehaviour
{
	// Convert Vector3 to byte array
	public static byte[] Vector3ToBytes(Vector3 vector)
	{
		byte[] bytes = new byte[12]; // 3 floats * 4 bytes per float
		Buffer.BlockCopy(BitConverter.GetBytes(vector.x), 0, bytes, 0, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(vector.y), 0, bytes, 4, 4);
		Buffer.BlockCopy(BitConverter.GetBytes(vector.z), 0, bytes, 8, 4);
		return bytes;
	}

	// Convert byte array to Vector3
	public static Vector3 BytesToVector3(byte[] bytes)
	{
		if (bytes.Length != 12)
			throw new ArgumentException("Byte array length must be 12 to convert to Vector3");

		float x = BitConverter.ToSingle(bytes, 0);
		float y = BitConverter.ToSingle(bytes, 4);
		float z = BitConverter.ToSingle(bytes, 8);
		return new Vector3(x, y, z);
	}

	void Start()
	{
		// Example usage
		Vector3 originalVector = new Vector3(1.0f, 2.0f, 3.0f);
		byte[] vectorBytes = Vector3ToBytes(originalVector);
		Vector3 convertedVector = BytesToVector3(vectorBytes);

		Debug.Log("Original Vector: " + originalVector);
		Debug.Log("Converted Vector: " + convertedVector);
	}
}