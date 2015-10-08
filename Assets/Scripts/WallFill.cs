using UnityEngine;
using System.Collections.Generic;
using Vectrosity;
using Construction;
using ClipperLib;

public class WallFill {

	private CanvasRenderer canvasRender;
	private Material wallMaterial;
	private List<UIVertex> vertlist;
	private List<Wall2D> walls;

	public WallFill(Canvas canvas, string name){
		GameObject go = new GameObject(name, typeof(RectTransform));
		go.transform.parent = canvas.transform;
		this.canvasRender = go.AddComponent<CanvasRenderer>();
		wallMaterial = new Material(Shader.Find("UI/Default"));
		vertlist = new List<UIVertex>();
		walls = new List<Wall2D>();
	}

	private int mSize = 0;

	void Resize(int size){
		while(vertlist.Count < size*4){
			vertlist.Add(new UIVertex());
		}
		
		this.mSize = size;
	}

	public void Clear(){
		walls.Clear();
		Resize(0);
	}

	public void Add(IList<Vector3> outter, IList<Vector3> inner, IList<Color> colors = null){
		if(outter.Count != inner.Count){
			Debug.Log("Outter's count not equal Inner");
			return;
		}

		if(colors != null && colors.Count != outter.Count-1){
			Debug.Log("colors' count not valid");
			return;
		}

		int startIndex = this.mSize*4;

		int count = startIndex + (outter.Count-1)*4;
		if(count > vertlist.Count){
			vertlist.AddRange(new UIVertex[count - vertlist.Count]);
		}

		UIVertex[] rect = new UIVertex[4];
		for(int i=0; i < outter.Count-1; i++){
			Color color = colors != null ? colors[i] : Color.white;

			rect[0].position = outter[i];
			rect[0].color = color;
			rect[1].position = outter[i+1];
			rect[1].color = color;
			rect[2].position = inner[i+1];
			rect[2].color = color;
			rect[3].position = inner[i];
			rect[3].color = color;

			for(int j=0; j < 4; j++){
				vertlist[startIndex + i*4 + j] = rect[j];
			}

			this.mSize++;
		}
	}

	void SetupOutLine(VectorLine outline, List<Vector3> list){
		int startIndex = outline.points3.Count;
		outline.Resize(startIndex + (list.Count-1)*2);
		for(int i = 0, j=startIndex; i < list.Count-1; i++, j=j+2){
			outline.points3[j] = list[i];
			outline.points3[j+1] = list[i+1];
		}
	}
	
	public void SetupSegment(List<Wall2D> wallList, VectorLine outline, Parallel parallel){

		//this.walls.AddRange(wallList);

		List<Color> colors = new List<Color>();
		foreach(Wall2D wall in wallList){
			this.walls.Add(wall);
			colors.Add(wall.Color);
		}

		Vector3[] room = DrawHelper.DiscreteToContinue(wallList);
		
		//		bool isClose = false;
		//		if(wallList[0].StartPos == wallList[wallList.Count-1].EndPos){
		//			isClose = true;
		//		}
		
		bool isClose = false;
		if(room[0] == room[room.Length-1]){
			isClose = true;
		}
		
		if(isClose){
			
//			List<Vector3> outter;
//			RoomQuad.GetPoint(room, true, out outter);
//			List<Vector3> inner;
//			RoomQuad.GetPoint(room, false, out inner);

			List<Vector3> outter = parallel.Execute(room, false, true);
			List<Vector3> inner = parallel.Execute(room, true, true);
			
			Add(outter, inner, colors);
			
			//			if(inner.Count > 0){
			//				SetupOutLine(outline, inner);
			//			}
			//			
			//			if(outter.Count > 0){
			//				SetupOutLine(outline, outter);
			//			}
			
		}
		else{
			List<Vector3> outter = parallel.Execute(room, false);
			List<Vector3> inner = parallel.Execute(room, true);
			
			Add(outter, inner, colors);
			
			if(inner.Count > 0){
				SetupOutLine(outline, inner);
			}
			
			if(outter.Count > 0){
				SetupOutLine(outline, outter);
			}
		}
		
	}

	public void Draw(){
		canvasRender.Clear();
		canvasRender.SetMaterial(wallMaterial, null);
		canvasRender.GetMaterial().color = Color.white;
		canvasRender.SetVertices(vertlist);
		//Debug.Log("cout: " +  list.Count);
	}

	public bool Select(Vector2 wordPos, out Wall2D wall){
		IntPoint mousePos = new IntPoint(wordPos.x * DrawHelper.ClipScalling, wordPos.y * DrawHelper.ClipScalling);

		List<IntPoint> path = new List<IntPoint>();
		//Debug.Log("wall count: " + this.mSize);
		for(int i=0; i < this.mSize; i++){
			path.Clear();

			for(int j=0; j < 4; j++){
				UIVertex p = vertlist[i*4 + j];
				IntPoint ip = new IntPoint(p.position.x * DrawHelper.ClipScalling, p.position.y * DrawHelper.ClipScalling);
				path.Add(ip);
			}

			if(Clipper.PointInPolygon(mousePos, path) >= 1){
				wall = walls[i];
				return true;
			}
		}

		wall = null;
		return false;
	}
}
