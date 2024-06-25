﻿using UnityEngine;

public class ScrollTexture : MonoBehaviour {

	public Material ScrollableMaterial;
	public string TextureBase = "_MainTex";
	public Vector2 ScrollVectorSpeed = new Vector2( 1.0f, 1.0f );
	private Vector2 uvOffset = Vector2.zero;

	void Update() 
	{
		uvOffset += ( ScrollVectorSpeed * Time.deltaTime );
		ScrollableMaterial.SetTextureOffset(TextureBase, uvOffset);
	}
}
