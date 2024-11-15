using UnityEngine;
using System.Collections;

public class MapDisplay : MonoBehaviour {

    public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRender;

    public void drawTexture(Texture2D texture) {
		float scaleFactor = texture.width / (float)texture.height; //updates scale to ensure alignment of texture with mesh dimensions
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height * scaleFactor);
    }

	public void drawMesh (dataMesh dataMesh, Texture2D texture) {
		meshFilter.sharedMesh = dataMesh.meshCreate();
		meshRender.sharedMaterial.mainTexture = texture;
		
	}
	
}
