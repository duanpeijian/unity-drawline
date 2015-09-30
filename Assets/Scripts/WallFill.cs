using UnityEngine;
using System.Collections.Generic;

public class WallFill {

	private CanvasRenderer canvasRender;
	private Material wallMaterial;
	private List<UIVertex> list;

	public WallFill(Canvas canvas, string name){
		GameObject go = new GameObject(name, typeof(RectTransform));
		go.transform.parent = canvas.transform;
		this.canvasRender = go.AddComponent<CanvasRenderer>();
		wallMaterial = new Material(Shader.Find("UI/Default"));
		list = new List<UIVertex>();
	}


	private int mSize = 0;

	public void Resize(int size){
		while(list.Count < size){
			list.Add(new UIVertex());
		}

		this.mSize = size;
	}

	public void Add(IList<Vector3> outter, IList<Vector3> inner){
		if(outter.Count != inner.Count){
			Debug.Log("Outter'count not equal Inner");
			return;
		}

		int startIndex = this.mSize;

		int count = startIndex + (outter.Count-1)*4;
		if(count > list.Count){
			list.AddRange(new UIVertex[count - list.Count]);
		}

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

			for(int j=0; j < 4; j++){
				list[startIndex + i*4 + j] = rect[j];
				this.mSize++;
			}
		}
	}

	public void Draw(){
		canvasRender.Clear();
		canvasRender.SetMaterial(wallMaterial, null);
		canvasRender.GetMaterial().color = Color.red;
		canvasRender.SetVertices(list);
		//Debug.Log("cout: " +  list.Count);
	}
}
