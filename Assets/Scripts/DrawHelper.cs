using UnityEngine;
using System.Collections.Generic;
using System;

public class DrawHelper {

	private static int mUnitPixels = 80;

	public static int unitPixels {
		get {
			return mUnitPixels;
		}
	}

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

	public static Action OnScaleChanged = null;

	public static void SetScale(Camera camera2d, int unitPixels){
		mUnitPixels = unitPixels;

		scale = 1f / unitPixels;
		camera2d.orthographicSize = scale * camera2d.pixelHeight / 2;

		if(OnScaleChanged != null){
			OnScaleChanged();
		}
	}

	public static Vector2 Vector3To2(Vector3 pos){
		return new Vector2(pos.x, pos.z);
	}
	
	public static Vector3 Vector2To3(Vector2 pos){
		return new Vector3(pos.x, 0f, pos.y);
	}

	public static Rect GetRealRect(Camera camera2d){
		Vector2 size = new Vector2(scale*camera2d.pixelWidth, 2*camera2d.orthographicSize);
		Transform trans = camera2d.transform;
		Vector2 offset = Vector3To2(trans.position);
		Rect realRect = new Rect(new Vector2(-size.x/2 + offset.x, -size.y/2 + offset.y), size);
		return realRect;
	}

	public static void MoveRealRect(Camera camera2d, Vector2 realDelta){
		Vector3 delta = Vector2To3(realDelta);
		camera2d.transform.position = camera2d.transform.position + delta * -1f;
	}

	public static Vector2 PixelToReal(Camera camera2d, Vector2 mousePos){
		Rect rect = camera2d.pixelRect;
		Rect realRect = GetRealRect(camera2d);
		float x = (mousePos.x - rect.xMin) * scale + realRect.xMin;
		float y = (mousePos.y - rect.yMin) * scale + realRect.yMin;
		return new Vector2(x, y);
	}

	public static List<Wall2D> ContinueToDiscrete(Vector2[] points){
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
	
	public static Vector2[] DiscreteToContinue(List<Wall2D> wallList, int startIndex  = 0){
		int length = wallList.Count;
		
		Vector2[] room = null;
		
		if(length > 0){
			room = new Vector2[length - startIndex + 1];
			int i=0;
			for(int j=startIndex; j < length; j++){
				room[i++] = wallList[j].StartPos;
			}
			
			room[i] = wallList[length-1].EndPos;
		}
		
		return room;
	}

}
