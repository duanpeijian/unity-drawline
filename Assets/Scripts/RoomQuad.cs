using UnityEngine;
using System.Collections.Generic;
using ClipperLib;

namespace Construction {
	using Path = List<IntPoint>; 
	using Paths = List<List<IntPoint>>;

	public class RoomQuad {

		public static Vector2[] GetVertex(){
			Vector2[] quad = new Vector2[] {
				new Vector3(0.5f, 0.5f),
				new Vector3(-0.5f, 0.5f),
				new Vector3(-0.5f, -0.5f),
				new Vector3(0.5f, -0.5f),
				new Vector3(0.5f, 0.5f)
			};

			for(int i=0; i < quad.Length; i++){
				Vector3 scale = new Vector3(4, 4);
				Vector3 p = quad[i];
				p.Scale(scale);
				quad[i] = p;
			}

			return quad;
		}
		
		public static bool GetPoint(Vector2[] quad, bool outter, out List<Vector2> list, bool isClose=true){
			List<IntPoint> quadLines = new List<IntPoint>();
			for(int i=0; i < quad.Length; i++){
				Vector3 from = quad[i];
				IntPoint to = new IntPoint(from.x * DrawHelper.ClipScalling, from.y * DrawHelper.ClipScalling);
				quadLines.Add(to);

				//				IntPoint to = quad[i+1];
				//				to = new IntPoint(to.X * scale.X * scalling, to.Y * scale.Y * scalling);
				//				quadLines.Add(to);
			}

			EndType endType = EndType.etOpenButt;
			if(isClose){
				endType = EndType.etClosedPolygon;
			}

			//Debug.Log("endType: " + endType);

			ClipperOffset co = new ClipperOffset();
			co.AddPath(quadLines, JoinType.jtMiter, endType);
			Paths solution = new Paths();
			double delta = -DrawHelper.WallThick/2 * DrawHelper.ClipScalling;
			if(outter){
				delta = -delta;
			}

			co.MiterLimit = 8;
			co.Execute(ref solution, delta);

			list = new List<Vector2>();

			if(solution.Count > 0){
				foreach(IntPoint p in solution[0]){
					Vector3 re = new Vector3(p.X * 1.0f/DrawHelper.ClipScalling, p.Y * 1.0f/DrawHelper.ClipScalling, 0f);

					if(!isClose){
						//Debug.Log("result: " + re);
					}

					list.Add(re);
				}
				list.Add(list[0]);
				return Clipper.Orientation(solution[0]);
			}
			else{
				Debug.LogError("no solution..");
			}

			return true;
		}
	}
}
