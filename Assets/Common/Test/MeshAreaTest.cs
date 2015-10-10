using UnityEngine;
using System.Collections.Generic;
using ClipperLib;

public class MeshAreaTest : MonoBehaviour {

	private long clipScalling;
	private Mesh mMesh;

	// Use this for initialization
	void Start () {
		clipScalling = (long)Mathf.Pow(10, 6);

		mMesh = GetComponent<MeshFilter>().mesh;

		//Debug.Log("result: " + Select(Vector3.zero));
		Test(mMesh);
	}

	void Test(Mesh mesh){
		List<IntPoint> solution = GetArea(mesh);
		
		if(solution != null){
			List<Vector2> result = new List<Vector2>();
			
			foreach(IntPoint p in solution){
				result.Add(new Vector2(p.X * 1f/clipScalling, p.Y * 1f/clipScalling));
			}
			
			foreach(Vector2 p in result){
				Debug.Log(string.Format("result: ({0}", p));
			}
		}
	}

	public bool Select(Vector3 wordPos){
		IntPoint p = new IntPoint(wordPos.x * clipScalling, wordPos.z * clipScalling);

		List<IntPoint> poly = GetArea(mMesh);
		if(Clipper.PointInPolygon(p, poly) == 1){
			return true;
		}

		return false;
	}

	bool Approximately(float x, float y){
		return Mathf.Abs(x-y) < 0.001f;
	}
	
	List<IntPoint> GetArea(Mesh mesh){
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;

		List<Vector2> list = new List<Vector2>();

		//Debug.Log("ver count: " + vertices.Length + " triangle: " + triangles.Length);

		float y = float.MinValue;
		for(int i=0; i < triangles.Length; i = i+3){
			Vector3 p0 = vertices[triangles[i]];
			Vector3 p1 = vertices[triangles[i+1]];
			Vector3 p2 = vertices[triangles[i+2]];

			if(Approximately(p0.y, p1.y) && Approximately(p0.y, p2.y)){
				//Debug.Log(string.Format("({0}, {1}, {2})", p0, p1, p2));

				if(y == float.MinValue){
					y = p0.y;
				}

				if(Approximately(p0.y, y)){
					list.Add(new Vector2(p0.x, p0.z));
					list.Add(new Vector2(p1.x, p1.z));
					list.Add(new Vector2(p2.x, p2.z));
				}
			}
		}

		//Debug.Log("list: " + list.Count);

		List<List<IntPoint>> paths = new List<List<IntPoint>>();
		for(int i=0; i < list.Count; i = i+3){
			List<IntPoint> path = new List<IntPoint>();

			for(int j=0; j < 3; j++){
				path.Add(new IntPoint(list[i+j].x * clipScalling, list[i+j].y * clipScalling));
			}

			paths.Add(path);
		}

		Clipper clipper = new Clipper();

		List<List<IntPoint>> solution = new List<List<IntPoint>>();
		clipper.AddPaths(paths, PolyType.ptSubject, true);
		clipper.Execute(ClipType.ctUnion, solution);

		if(solution.Count > 0){
			return solution[0];
		}

		return null;
	}
}
