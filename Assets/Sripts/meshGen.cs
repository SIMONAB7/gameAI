using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class meshGen {
    
	public static dataMesh GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve) {
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);
		float topLeftX = (width - 1)/ -2f;
		float topLeftZ = (height - 1)/ 2f;
		
		dataMesh dataMesh = new dataMesh (width, height);
		int vertexIndex = 0;

		for (int y = 0; y < height; y++) {
			for(int x = 0; x < width; x++) {
				
				float adjustedHeight =heightCurve.Evaluate(heightMap[x,y]) * heightMultiplier; //adjusts vertex position for consistent alignment and height scaling
				dataMesh.vertices [vertexIndex] = new Vector3 (topLeftX + x, heightCurve.Evaluate(heightMap[x,y]) * heightMultiplier, topLeftZ - y);
				dataMesh.uv[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
				
				if (x < width - 1 && y < height - 1) {
					dataMesh.triangleAdd(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
					dataMesh.triangleAdd(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
				}
				
				vertexIndex++;
			}
		}
		return dataMesh;
	}
}

public class dataMesh { 
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uv;

	int triangleIndex;

	public dataMesh(int meshWidth, int meshHeight) {
		vertices = new Vector3[meshWidth * meshHeight];
		uv = new Vector2[meshWidth * meshHeight];
		triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
	}

	public void triangleAdd(int a, int b, int c) {
		triangles [triangleIndex] = a;
		triangles [triangleIndex + 1] = b;
		triangles [triangleIndex + 2] = c;
		triangleIndex += 3;
	}

	public Mesh meshCreate() {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.RecalculateNormals();
		return mesh;
	}
	
}
