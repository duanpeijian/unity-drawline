using UnityEngine;
using System.Collections.Generic;

public class DrawHelper {

	private static double mWallThick = 0.1f;
	
	public static double WallThick {
		get {
			return mWallThick;
		}
	}
	
	private static long clipScalling;

	public static long ClipScalling {
		get {
			return clipScalling;
		}
	}
	
	static DrawHelper() {
		clipScalling = (long)Mathf.Pow(10, 6);
	}

	private static float scale;

	public static float Scale {
		get {
			return scale;
		}
		set {
			scale = value;
		}
	}

	public static Rect GetRealRect(Camera camera2d){
		Vector2 size = new Vector2(scale*camera2d.pixelWidth, 2*camera2d.orthographicSize);
		Vector2 offset = new Vector2(camera2d.transform.position.x, camera2d.transform.position.y);
		Rect realRect = new Rect(new Vector2(-size.x/2 + offset.x, -size.y/2 + offset.y), size);
		return realRect;
	}
	
	public static Vector2 PixelToReal(Camera camera2d, Vector2 mousePos){
		Rect rect = camera2d.pixelRect;
		Rect realRect = GetRealRect(camera2d);
		float x = (mousePos.x - rect.xMin) * scale + realRect.xMin;
		float y = (mousePos.y - rect.yMin) * scale + realRect.yMin;
		return new Vector2(x, y);
	}

	public static List<Wall2D> ContinueToDiscrete(Vector3[] points){
		bool isClose = false;
		if(points[0] == points[points.Length-1]){
			isClose = true;
		}

		List<Wall2D> list = new List<Wall2D>();
		Wall2D wall = null;
		for(int i=0; i < points.Length-1; i++){
			Wall2D newWall = new Wall2D(points[i], points[i+1]);
			list.Add(newWall);
			
			if(wall != null){
				wall.WallAtEnd = newWall;
			}
			
			wall = newWall;
		}

		if(isClose){
			wall.WallAtEnd = list[0];
		}
		
		return list;
	}
	
	public static Vector3[] DiscreteToContinue(List<Wall2D> wallList, int startIndex  = 0){
		int length = wallList.Count;
		
		Vector3[] room = null;
		
		if(length > 0){
			room = new Vector3[length - startIndex + 1];
			int i=0;
			for(int j=startIndex; j < length; j++){
				room[i++] = wallList[j].StartPos;
			}
			
			room[i] = wallList[length-1].EndPos;
		}
		
		return room;
	}

}
