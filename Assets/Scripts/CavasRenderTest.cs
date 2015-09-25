using UnityEngine;
using System.Collections;

public class CavasRenderTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Color32 color = new Color32(255, 0, 0, 255);
		//color = new Color32();
		UIVertex[] data = new UIVertex[] {
			new UIVertex() { position = new Vector3(0f, 0f, 0f), color = color  },
			new UIVertex() { position = new Vector3(0f, 50f, 0f), color = color },
			new UIVertex() { position = new Vector3(50f, 50f, 50f), color = color },
			new UIVertex() { position = new Vector3(50f, 0f, 50f), color = color }
		};
		
		CanvasRenderer canvasRender = GetComponent<CanvasRenderer>();
		canvasRender.Clear();
		Material defaultMaterial = new Material(Shader.Find ("UI/Default"));
		canvasRender.SetMaterial(defaultMaterial, null);
		canvasRender.SetVertices(data, data.Length);
		Debug.Log("start");
	}
	
	// Update is called once per frame
	void Update () {

	}
}
