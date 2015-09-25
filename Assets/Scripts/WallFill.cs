using UnityEngine;
using System.Collections.Generic;

public class WallFill {

	private CanvasRenderer canvasRender;
	private Material wallMaterial;

	public WallFill(Canvas canvas, string name){
		GameObject go = new GameObject(name, typeof(RectTransform));
		go.transform.parent = canvas.transform;
		this.canvasRender = go.AddComponent<CanvasRenderer>();
		wallMaterial = new Material(Shader.Find("UI/Default"));
	}

	public void Draw(IList<Vector3> outter, IList<Vector3> inner){
		if(outter.Count != inner.Count){
			Debug.Log("Outter'count not equal Inner");
			return;
		}

		List<UIVertex> list = new List<UIVertex>();
		UIVertex[] rect = new UIVertex[4];
		for(int i=0; i < outter.Count-1; i++){
			rect[0].position = outter[i];
			rect[0].color = Color.white;
			rect[1].position = outter[i+1];
			rect[1].color = Color.white;
			rect[2].position = inner[i+1];
			rect[2].color = Color.white;
			rect[3].position = inner[i];
			rect[3].color = Color.white;
			list.AddRange(rect);
		}

		canvasRender.Clear();
		canvasRender.SetMaterial(wallMaterial, null);
		canvasRender.GetMaterial().color = Color.red;
		canvasRender.SetVertices(list);
		//Debug.Log("cout: " +  list.Count);
	}
}
