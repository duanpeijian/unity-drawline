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

	public void Add(IList<Vector2> outter, IList<Vector2> inner, IList<Color> colors = null){
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

			rect[0].position = DrawHelper.Vector2To3(outter[i]);
			rect[0].color = color;
			rect[1].position = DrawHelper.Vector2To3(outter[i+1]);
			rect[1].color = color;
			rect[2].position = DrawHelper.Vector2To3(inner[i+1]);
			rect[2].color = color;
			rect[3].position = DrawHelper.Vector2To3(inner[i]);
			rect[3].color = color;

			for(int j=0; j < 4; j++){
				vertlist[startIndex + i*4 + j] = rect[j];
			}

			this.mSize++;
		}
	}

	void SetupOutLine(VectorLine outline, List<Vector2> list){
		int startIndex = outline.points3.Count;
		outline.Resize(startIndex + (list.Count-1)*2);
		for(int i = 0, j=startIndex; i < list.Count-1; i++, j=j+2){
			outline.points3[j] = DrawHelper.Vector2To3(list[i]);
			outline.points3[j+1] = DrawHelper.Vector2To3(list[i+1]);
		}
	}
	
	public void SetupSegment(List<Wall2D> wallList, float wallThick, VectorLine outline, Parallel parallel){

		//this.walls.AddRange(wallList);

		List<Color> colors = new List<Color>();
		foreach(Wall2D wall in wallList){
			this.walls.Add(wall);
			colors.Add(wall.Color);
		}

		Vector2[] room = DrawHelper.DiscreteToContinue(wallList);
		
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

			List<Vector2> outter = parallel.Execute(room, wallThick, false, true);
			List<Vector2> inner = parallel.Execute(room, wallThick, true, true);
			
			Add(outter, inner, colors);
			
			if(inner.Count > 0){
				SetupOutLine(outline, inner);
			}
			
			if(outter.Count > 0){
				SetupOutLine(outline, outter);
			}
		}
		else{
			List<Vector2> outter = parallel.Execute(room, wallThick, false);
			List<Vector2> inner = parallel.Execute(room, wallThick, true);
			
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
		//Debug.Log("cout: " +  list.Count);
		//canvasRender.SetVertices(vertlist);
		SetVertices(vertlist.ToArray(), vertlist.Count);
	}

	private Mesh mesh = new Mesh();

	private void SetVertices (UIVertex[] vertices, int size)
	{
		List<Vector3> list = new List<Vector3> ();
		List<Color32> list2 = new List<Color32> ();
		List<Vector2> list3 = new List<Vector2> ();
		List<Vector2> list4 = new List<Vector2> ();
		List<Vector3> list5 = new List<Vector3> ();
		List<Vector4> list6 = new List<Vector4> ();
		List<int> list7 = new List<int> ();
		for (int i = 0; i < size; i += 4)
		{
			for (int j = 0; j < 4; j++)
			{
				list.Add (vertices [i + j].position);
				list2.Add (vertices [i + j].color);
				list3.Add (vertices [i + j].uv0);
				list4.Add (vertices [i + j].uv1);
				list5.Add (vertices [i + j].normal);
				list6.Add (vertices [i + j].tangent);
			}
			list7.Add (i);
			list7.Add (i + 1);
			list7.Add (i + 2);
			list7.Add (i + 2);
			list7.Add (i + 3);
			list7.Add (i);
		}

		mesh.SetVertices (list);
		mesh.SetColors (list2);
		mesh.SetNormals (list5);
		mesh.SetTangents (list6);
		mesh.SetUVs (0, list3);
		mesh.SetUVs (1, list4);
		mesh.SetIndices (list7.ToArray (), MeshTopology.Triangles, 0);
		canvasRender.SetMesh (mesh);
	}

	public bool Select(Vector2 wordPos, out Wall2D wall){
		IntPoint mousePos = new IntPoint(wordPos.x * DrawHelper.ClipScalling, wordPos.y * DrawHelper.ClipScalling);

		List<IntPoint> path = new List<IntPoint>();
		//Debug.Log("wall count: " + this.mSize);
		for(int i=0; i < this.mSize; i++){
			path.Clear();

			for(int j=0; j < 4; j++){
				UIVertex p = vertlist[i*4 + j];
				//IntPoint ip = new IntPoint(p.position.x * DrawHelper.ClipScalling, p.position.y * DrawHelper.ClipScalling);
				Vector2 pos = DrawHelper.Vector3To2(p.position);
				IntPoint ip = new IntPoint(pos.x * DrawHelper.ClipScalling, pos.y * DrawHelper.ClipScalling);

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
