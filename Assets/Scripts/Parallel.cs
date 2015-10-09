using UnityEngine;
using System.Collections.Generic;

public class Parallel {

	private float dist = 0.05f;

	public struct Line {
		public bool isVetical;
		public float k;
		public float b;

	}

	public Line GetOffsetLine(Wall2D wall, Vector2 realDelta){
		Vector2 rightVector = GetOffset(wall.StartPos, wall.EndPos, true, 1.0f);
		float dist = Vector2.Dot(realDelta, rightVector);
		Vector2 offset = rightVector * dist;

		return GetParallelLine(wall.StartPos, wall.EndPos, offset);
	}

	//room is one draw line(continue line);
	public List<Vector3> GetRuler(Vector2[] room, float dist, bool right = true){
		if(room.Length < 2){
			Debug.LogError("vertex count less than 2");
			return null;
		}

		List<Vector3> result = new List<Vector3>();

		for(int i=0; i < room.Length-1; i++){
			Vector2 offset = GetOffset(room[i], room[i+1], right, dist);
			Vector2 p0 = new Vector2(room[i].x + offset.x, room[i].y + offset.y);
			Vector2 p1 = new Vector2(room[i+1].x + offset.x, room[i+1].y + offset.y);
			result.Add(p0);
			result.Add(p1);
		}

		return result;
	}

	private Vector2 GetOffset(Vector3 p0, Vector3 p1, bool right, float dist){
		Vector2 lineVector = p1 - p0;
		Vector3 rightVector = Vector3.Cross(lineVector, Vector3.forward).normalized;
		rightVector = right? rightVector : -rightVector;
		Vector2 offset = new Vector2(rightVector.x, rightVector.y) * dist;
		return offset;
	}

	Line GetParallelLine(Vector2 p0, Vector2 p1, Vector2 offset){
		Vector2 middle = (p0 + p1)/2;
		middle = middle + offset;
		
		//result.Add(middle);
		
		Line line = GetLine(p0, p1);
		if(line.isVetical){
			line.b = middle.x;
		}
		else{
			line.b = middle.y - line.k * middle.x;
		}

		return line;
	}

	public List<Vector3> Execute(Vector3[] room, bool right = true, bool isClose = false){
		if(room.Length < 2){
			Debug.LogError("vertex count less than 2");
			return null;
		}


		List<Line> lines = new List<Line>();

		Vector2 first = Vector2.zero;
		Vector2 last = Vector2.zero;

		List<Vector3> result = new List<Vector3>();

		for(int i=0; i < room.Length-1; i++){
			Vector2 offset = GetOffset(room[i], room[i+1], right, dist);

			if(!isClose){
				if(i == 0){
					first = new Vector2(room[0].x + offset.x, room[0].y + offset.y);
				}
				
				if(i == room.Length-2){
					last = new Vector2(room[i+1].x + offset.x, room[i+1].y + offset.y);
				}
			}

			Line line = GetParallelLine(room[i], room[i+1], offset);

			//Debug.Log(string.Format("line: {0}({1}, {2})", line.isVetical, line.k, line.b));
			lines.Add(line);
		}

		if(isClose){
			Vector2 p;
			if(GetCrossPoint(lines[0], lines[lines.Count-1], out p)){
				first = last = p;
			}
		}

		result.Add(first);

		for(int i=0; i < lines.Count-1; i++){
			Line line1 = lines[i];
			Line line2 = lines[i+1];

			Vector2 p;
			if(GetCrossPoint(line1, line2, out p)){
				result.Add(p);
			}
		}

		result.Add(last);

		return result;
	}

	public bool GetCrossPoint(Line line1, Line line2, out Vector2 p){

		p = Vector2.zero;

		if(line1.isVetical && line2.isVetical){
			Debug.LogError("two line vetical");
			return false;
		}
		
		if(line1.isVetical){
			p.x = line1.b;
			p.y = line2.k * p.x + line2.b;
		}
		else if(line2.isVetical){
			p.x = line2.b;
			p.y = line1.k * p.x + line1.b;
		}
		else if(line2.k == line1.k){
			Debug.LogError("two parallel line");
			return false;
		}
		else{
			p.x = (line1.b - line2.b)/(line2.k - line1.k);
			p.y = line1.k * p.x + line1.b;
		}

		return true;
	}

	//float[0] is k and float[1] is b;
	public Line GetLine(Vector2 p0, Vector2 p1){
		Line line = new Line();
		line.isVetical = false;

		if(Mathf.Approximately(p0.x, p1.x)){
			line.isVetical = true;
			line.b = p0.x;
		}
		else if(Mathf.Approximately(p0.y, p1.y)){
			line.k = 0;
			line.b = p0.y;
		}
		else{
			line.k = (p1.y - p0.y)/(p1.x - p0.x);
			line.b = p0.y - line.k * p0.x;
		}

		return line;
	}
}
