using UnityEngine;
using System.Collections;

//This class animates the Uvs used in the LineRenderer so it looks better
public class ScrollingUvs : MonoBehaviour
{
	public Vector2 uvAnimationRate = new Vector2(0.0f, 0.0f);
	Vector2 offset = Vector2.zero;
	Vector2 startOffset;


	void OnEnable()
	{
		startOffset = GetComponent<Renderer>().material.GetTextureOffset("_MainTex");
	}

	void OnDisable()
	{
		offset = Vector2.zero;
		if (GetComponent<Renderer>().enabled)
		{
			GetComponent<Renderer>().material.SetTextureOffset("_MainTex", startOffset);

		}
	}

	void Update()
	{
		offset += (uvAnimationRate * Time.deltaTime);
		if (GetComponent<Renderer>().enabled)
		{
			GetComponent<Renderer>().material.SetTextureOffset("_MainTex", (startOffset + offset));

		}
		if (offset.y > 10)
			offset = Vector2.zero;
	}

}