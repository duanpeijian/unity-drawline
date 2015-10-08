using UnityEngine;
using Vectrosity;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using Construction;

public enum State2D {
	Idle,
	Draw
}

public class DrawGrid2: MonoBehaviour
{
	private int unitPixels = 100;

	private VectorLine gridLine;
	private Camera camera2d;

	private State2D mState;
	private Vector2 lastMousePos;

	private float wallThick = 0.1f;
	private List<Wall2D> walls = new List<Wall2D>();
	private Vector2[] wall = new Vector2[2];
	private int drawStartIndex = 0;

	private VectorLine drawLine;
	private List<float> widths = new List<float>();
	private bool drawBreak = true;
	private bool isContinue = false;

	private VectorLine rulerLine;
	private Parallel mParallel = new Parallel();

	private List<Wall2D> found = new List<Wall2D>();
	private VectorLine segmentOutline;
	private WallFill segmentFill;

	void Update(){
		segmentFill.Clear();
		segmentOutline.Resize(0);
		var savedWall = Home.Get().Walls;
		found.Clear();
		for(int i=0; i < savedWall.Count; i++){
			Wall2D start = savedWall[i];
			
			if(found.Contains(start)){
				continue;
			}
			
			List<Wall2D> toDraw = new List<Wall2D>();
			Wall2D wall = start;
			toDraw.Add(wall);
			while(wall.WallAtEnd != null && wall.WallAtEnd != start){
				wall = wall.WallAtEnd;
				found.Add(wall);
				toDraw.Add(wall);
			}

//			if(wall.WallAtEnd != null && wall.WallAtEnd == start){
//			}
//			else{
//			}

			segmentFill.SetupSegment(toDraw, segmentOutline, mParallel);
		}

		segmentFill.Draw();

		segmentOutline.SetColor(Color.black);
		segmentOutline.Draw3D();

		if(mState != State2D.Draw){
			return;
		}

		Rect rect = camera2d.pixelRect;
		Vector2 current = Input.mousePosition;
		if(!rect.Contains(current)){
			return;
		}

		if(Input.GetMouseButtonDown(0)){
			if(drawBreak){
				drawBreak = false;
				
				wall[0] = wall[1] = DrawHelper.PixelToReal(camera2d, current);
				
				walls.Clear();
				
				drawStartIndex = walls.Count;
				
				if(walls.Count == 0){
					//walls.Add(new Wall2D(wall[0], wall[1]));
				}
			}
			else{
				if(isContinue){
					Wall2D newWall = new Wall2D(wall[0], wall[1]);
					walls.Add(newWall);
					wall[0] = wall[1];
				}
				else{
					wall[1] = DrawHelper.PixelToReal(camera2d, current);
					Wall2D newWall = new Wall2D(wall[0], wall[1]);
					if(walls.Count >= 1){
						newWall.WallAtStart = walls[walls.Count-1];
					}
					
					Home.Get().AddWall(newWall);
					walls.Add(newWall);
					wall[0] = wall[1];
				}
			}
		}
		
		if(Input.GetMouseButtonDown(1)){
			drawBreak = true;
			
//			Vector3[] room = DiscreteToContinue(walls, drawStartIndex);
//			if(room != null){
//				DrawWall(room);
//			}
		}
		
		if(Input.GetMouseButton(0) || Input.GetMouseButton(1)){
			Vector2 delta = current - lastMousePos;
			
			//				if(mState == State2D.Idle){
			//					OnDrag(delta);
			//				}
			
			//				if(mState == State2D.Draw){
			//					wall[1] = PixelToReal(camera2d, current);
			//					walls.Add(wall);
			//				}
			
			lastMousePos = current;
		}
		
		//stop draw line;
		if(Input.GetMouseButtonDown(2)){
			drawBreak = true;
		}
		
		if(!drawBreak){
			wall[1] = DrawHelper.PixelToReal(camera2d, current);
		}
		
		if(!isContinue){
			var wallSaved = Home.Get().Walls;
			drawLine.Resize(wallSaved.Count*2 + 2);
			
			widths.Clear();
			int i = 0;
			for(; i < wallSaved.Count; i++){
				Wall2D line = wallSaved[i];
				drawLine.points3[i*2] = line.StartPos;
				drawLine.points3[i*2+1] = line.EndPos;
				
				widths.Add(wallThick * unitPixels);
			}
			
			//walls[walls.Count-1][1] = wall[1];
			drawLine.points3[i*2] = wall[0];
			drawLine.points3[i*2+1] = wall[1];
			
			//drawLine.SetWidths(widths);
			drawLine.color = Color.black;
			drawLine.Draw3D();
			
			//draw ruler;
			List<Vector3> ruler = mParallel.GetRuler(wall, 0.1f, false);
			rulerLine.Resize(2);
			rulerLine.points3[0] = ruler[0];
			rulerLine.points3[1] = ruler[1];
			rulerLine.SetColor(Color.blue);
			rulerLine.Draw3D();
		}
		else{
			drawLine.Resize(walls.Count + 2);
			
			widths.Clear();
			int i = 0;
			for(; i < walls.Count; i++){
				Wall2D line = walls[i];
				drawLine.points3[i] = line.StartPos;
				widths.Add(wallThick * unitPixels);
			}
			
			if(i == 0){
				drawLine.points3[i] = wall[0];
			}
			else{
				drawLine.points3[i] = walls[i-1].EndPos;
			}
			
			drawLine.points3[i+1] = wall[1];
			
			drawLine.color = Color.black;
			//drawLine.SetWidths(widths);
			drawLine.Draw3D();
		}
	}

//	private abstract class ControllerState {
//		public abstract void SetMode(State2D mode);
//		public abstract void Enter();
//		public abstract void Exit();
//		public abstract void MouseMove(Vector2 pos);
//		public abstract void Escape();
//	}
//
//	private ControllerState state;
//
//	private void setState(ControllerState state) {
//		if (this.state != null) {
//			this.state.Exit();
//		}
//		this.state = state;
//		this.state.Enter();
//	}
//
//	private ContinueState continueState = new ContinueState();
//	private IdleState idleState = new IdleState();
//
//    private IdleState getIdleState() {
//		return idleState;
//	}
//
//	private class IdleState : ControllerState {
//		private Vector2 lastMousePos;
//
//		
//		public override void SetMode(State2D mode) {
//			if(mode == State2D.Draw){
//				this.Escape();
//				//setState(getIdleState());
//			}
//		}
//		
//		public override void Enter(){
//			lastMousePos = Input.mousePosition;
//		}
//		
//		public override void MouseMove(Vector2 current){
//			Vector2 delta = current - lastMousePos;
//
////			Vector2 realDelta = delta * scale;
////			camera2d.transform.position = camera2d.transform.position + new Vector3(-realDelta.x, -realDelta.y, 0f);
////			MakeGrid2();
//
//			lastMousePos = current;
//		}
//		
//		public override void Escape(){
//			
//		}
//		public override void Exit ()
//		{
//
//		}
//
//	}
//
//	public class ContinueState {
//
//	}

	private void OnDrag(Vector2 delta){
		if(mState == State2D.Idle){
			Vector2 realDelta = delta * DrawHelper.Scale;
			camera2d.transform.position = camera2d.transform.position + new Vector3(-realDelta.x, -realDelta.y, 0f);
			MakeGrid2();
		}
	}

	void Awake(){
		Canvas canvas2d = VectorLine.canvas;
		GraphicRaycaster graphicRaycaster = canvas2d.gameObject.AddComponent<GraphicRaycaster>();
		graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.ThreeD;

		Button drawBtn = GameObject.Find("UIRoot/DrawBtn").GetComponent<Button>();

		bool clicked = false;

		drawBtn.onClick.AddListener(() => {
			clicked  = !clicked;
			Debug.Log("On DrawBtn clicked.." + clicked);

			if(clicked){
				if(this.mState != State2D.Draw){
					this.mState = State2D.Draw;
				}
			}
			else{
				if(this.mState == State2D.Draw){
					this.mState = State2D.Idle;
				}
			}
		});

		GameObject background = new GameObject("background", typeof(RectTransform));
		RectTransform rectTrans = background.GetComponent<RectTransform>();
		rectTrans.parent = canvas2d.transform;
		rectTrans.anchoredPosition = new Vector2(0.5f, 0.5f);
		rectTrans.anchorMin = new Vector2(0f, 0f);
		rectTrans.anchorMax = new Vector2(1f, 1f);
		GraphicTest test = background.AddComponent<GraphicTest>();
//		test.onDrag = (eventData) => {
//			Debug.Log("onDrag..");
//			if(mState == State2D.Idle){
//				OnDrag(eventData.delta);
//			}
//		};

		test.color = new Color(0.95f, 0.95f, 0.95f, 1.0f);

		EventTrigger trigger = background.AddComponent<EventTrigger>();

		//OnClick;
		EventTrigger.Entry entry = new EventTrigger.Entry();
		trigger.triggers.Add(entry);
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener((eventData) => {
//			PointerEventData data = eventData as PointerEventData;
//			if(mState == State2D.Idle){
//				//Debug.Log("onClick..");
//				Vector2 wordPos = DrawHelper.PixelToReal(camera2d, data.position);
//				Wall2D wall;
//				if(segmentFill.Select(wordPos, out wall)){
//					wall.Color = Color.green;
//				}
//			}
		});

		Wall2D selected = null;

		//OnPress Down;
		entry = new EventTrigger.Entry();
		trigger.triggers.Add(entry);
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener((eventData) => {
			PointerEventData data = eventData as PointerEventData;
			if(mState == State2D.Idle){
				Vector2 wordPos = DrawHelper.PixelToReal(camera2d, data.position);
				if(segmentFill.Select(wordPos, out selected)){
					//Debug.Log(string.Format("change green..{0}, {1}", selected.StartPos, selected.EndPos ));
					selected.Color = Color.green;
				}
			}
		});

		//OnPress Up;
		entry = new EventTrigger.Entry();
		trigger.triggers.Add(entry);
		entry.eventID = EventTriggerType.PointerUp;
		entry.callback.AddListener((eventData) => {
			PointerEventData data = eventData as PointerEventData;
			if(mState == State2D.Idle){
				if(selected != null){
					selected.Color = Color.red;
					selected = null;
				}
			}
		});

		//OnDrag;
		entry = new EventTrigger.Entry();
		trigger.triggers.Add(entry);
		entry.eventID = EventTriggerType.Drag;
		entry.callback.AddListener((eventData) => {
			//Debug.Log("onDrag..");
			PointerEventData data = eventData as PointerEventData;
			Vector2 realDelta = data.delta * DrawHelper.Scale;

			if(mState == State2D.Idle){
				if(selected == null){
					camera2d.transform.position = camera2d.transform.position + new Vector3(-realDelta.x, -realDelta.y, 0f);
					MakeGrid2();
				}
				else{
					Wall2D wall = selected;
					Vector2 startT = wall.StartPos + realDelta;
					Vector2 endT = wall.EndPos + realDelta;

					wall.StartPos = startT;
					if(wall.WallAtStart != null){
						wall.WallAtStart.EndPos = startT;
					}

					wall.EndPos = endT;
					if(wall.WallAtEnd != null){
						wall.WallAtEnd.StartPos = endT;
					}
				}
			}
		});

	}

	void DrawWall(Vector3[] room){
		bool isClose = false;
		if(room[0] == room[room.Length-1]){
			isClose = true;
		}

		if(isClose){
			List<Vector3> outter;
			RoomQuad.GetPoint(room, true, out outter);
			List<Vector3> inner;
			RoomQuad.GetPoint(room, false, out inner);

			WallFill wallFill = new WallFill(VectorLine.canvas3D, "wallFill");
			wallFill.Add(outter, inner);
			wallFill.Draw();
			
			if(inner.Count > 0){
				VectorLine roomInner = new VectorLine("RoomInner", inner, null, 1.0f, LineType.Continuous, Joins.Weld);
				roomInner.SetColor(Color.black);
				roomInner.Draw3D();
			}
			
			if(outter.Count > 0){
				VectorLine roomOutter = new VectorLine("RoomOutter", outter, null, 1.0f, LineType.Continuous, Joins.Weld);
				roomOutter.SetColor(Color.black);
				roomOutter.Draw3D();
			}
		}
		else {

			Parallel parallel = new Parallel();
			List<Vector3> outter = parallel.Execute(room, false);
			List<Vector3> inner = parallel.Execute(room, true);

			WallFill wallFill = new WallFill(VectorLine.canvas3D, "wallFill");
			wallFill.Add(outter, inner);
			wallFill.Draw();
			
//			if(inner.Count > 0){
//				VectorLine roomInner = new VectorLine("RoomInner", inner, null, 1.0f, LineType.Continuous, Joins.None);
//				roomInner.SetColor(Color.black);
//				roomInner.Draw3D();
//			}
//			
//			if(outter.Count > 0){
//				VectorLine roomOutter = new VectorLine("RoomOutter", outter, null, 1.0f, LineType.Continuous, Joins.None);
//				roomOutter.SetColor(Color.black);
//				roomOutter.Draw3D();
//			}

//			List<Vector3> outter;
//			bool counterClockwise = RoomQuad.GetPoint(room, isClose, true, out outter);
//
//			int length = outter.Count-1;
//
//			int startA = -1;
//			int startB = -1;
//			int endC = -1;
//			int endD = -1;
//			for(int i=0; i < length; i++){
//				Vector2 dist = outter[i] - room[0];
//				float wallThick = (float)RoomQuad.WallThick;
//				if(Mathf.Abs(dist.sqrMagnitude - wallThick/2*wallThick/2) < 0.001f){
//					if(startA == -1){
//						startA = i;
//					}
//					else{
//						startB = i;
//					}
//				}
//
//				dist = outter[i] - room[room.Length-1];
//				if(Mathf.Abs(dist.sqrMagnitude - wallThick/2*wallThick/2) < 0.001f){
//					if(endC == -1){
//						endC = i;
//					}
//					else{
//						endD = i;
//					}
//				}
//			}
//
//			Debug.Log(string.Format("start index: ({0}, {1}), ", startA, startB));
//
//			Debug.Log("orientation: " + counterClockwise);
//
//			List<Vector3> a = new List<Vector3>();
//			List<Vector3> b = new List<Vector3>();
//
//			if(counterClockwise){
//
//				Vector3[] tmp = new Vector3[length+1];
//				outter.CopyTo(0, tmp, 0, length);
//
////				if(length%2 == 1){
////					tmp[length] = tmp[length-1];
////					length++;
////				}
//
//				for(int i = startA; i > startA - length/2; i--){
//					b.Add(tmp[(i+length)%length]);
//				}
//
//				for(int i = startB; i < startB + length/2; i++){
//					a.Add(tmp[i%length]);
//				}
//
//				WallFill wallFill = new WallFill(VectorLine.canvas3D, "wallFill");
//				wallFill.Draw(a, b);
//			}

//			if(outter.Count > 0){
//				VectorLine roomInner = new VectorLine("RoomOutter", outter, null, 1.0f, LineType.Continuous, Joins.Weld);
//				roomInner.SetColor(Color.blue);
//				roomInner.Draw3D();
//			}
		}

	}

	void Start ()
	{
		camera2d = GameObject.Find ("Camera2d").GetComponent<Camera> ();
		VectorLine.SetCanvasCamera (camera2d);
		VectorLine.canvas3D.sortingLayerName = "World";

		gridLine = new VectorLine ("Grid", new Vector2[0], null, 1.0f);
		drawLine = new VectorLine("DrawLine", new Vector3[0], null, 1.0f, isContinue? LineType.Continuous : LineType.Discrete, isContinue ? Joins.None : Joins.None);
		//drawLine.rectTransform.gameObject.AddComponent<Outline>();
		rulerLine = new VectorLine("Ruler", new Vector3[0], null, 1.0f, LineType.Discrete);
		segmentOutline = new VectorLine("SegmentOutline", new Vector3[0], null, 1.0f, LineType.Discrete);
		segmentFill = new WallFill(VectorLine.canvas3D, "SegmentFill");

		Vector3[] room = RoomQuad.GetVertex();
		//DrawWall(room);
		List<Wall2D> roomWalls = DrawHelper.ContinueToDiscrete(room);
		Home.Get().AddWallList(roomWalls);

//		VectorLine testLine = new VectorLine("test", new Vector3[] { Vector3.zero, new Vector3(1.0f, 0f, 0f), Vector3.one }, null, 2.0f, LineType.Continuous);
//		testLine.Draw3D();

		// Align 1-pixel lines on the pixel grid, so they don't potentially get messed up by anti-aliasing
		//gridLine.rectTransform.anchoredPosition = new Vector2 (.5f, .5f);
		gridLine.rectTransform.anchoredPosition = new Vector2(0f, 0f);
		//MakeGrid ();
		
		SetScale(unitPixels);
		
		mState = State2D.Idle;
	}

	void SetScale(int unitPixels){
		DrawHelper.Scale = 1f / unitPixels;
		camera2d.orthographicSize = DrawHelper.Scale * camera2d.pixelHeight / 2;
		
		MakeGrid2();
	}

	void OnGUI ()
	{
		GUI.Label (new Rect (10, 10, 30, 20), unitPixels.ToString ());
		unitPixels = (int)GUI.HorizontalSlider (new Rect (40f, 15f, 590f, 20f), (float)unitPixels, 50f, 200f);
		if (GUI.changed) {
			//camera2d.orthographicSize = gridPixels;
			//MakeGrid ();
			
			SetScale(unitPixels);
		}
	}

	private int gridUnit = 1;
	
	void MakeGrid2(){

		List<Vector2> points2 = new List<Vector2>();
		
		Rect realRect = DrawHelper.GetRealRect(camera2d);
		
		float xMin = realRect.xMin;
		float xMax = realRect.xMax;
		
		List<float> widths = new List<float>();
		
		int startX = (int)xMin % gridUnit == 0? (int)xMin : ((int)xMin / gridUnit) * gridUnit;
		for(float x = startX; x <= xMax; x = x+gridUnit){
			
			float pixelX = (x-xMin) / DrawHelper.Scale;
			points2.Add(new Vector2(pixelX, 0));
			points2.Add(new Vector2(pixelX, camera2d.pixelHeight - 1));
			
			if(Mathf.Approximately(x, 0)){
				widths.Add(4.0f);
			}
			else{
				widths.Add(1.0f);
			}
		}
		
		float yMin = realRect.yMin;
		float yMax = realRect.yMax;
		
		int startY = (int)yMin % gridUnit == 0? (int)yMin : ((int)yMin / gridUnit) * gridUnit;
		for(float y = startY; y <= yMax; y = y+gridUnit){
			float pixelY = (y-yMin) / DrawHelper.Scale;
			points2.Add(new Vector2(0, pixelY));
			points2.Add(new Vector2(camera2d.pixelWidth - 1, pixelY));
			
			if(Mathf.Approximately(y, 0)){
				widths.Add(4.0f);
			}
			else{
				widths.Add(1.0f);
			}
		}
		
		gridLine.Resize(points2.Count);
		for(int i=0; i < points2.Count; i++){
			gridLine.points2[i] = points2[i];
		}
		
		gridLine.SetWidths(widths);
		gridLine.SetColor(Color.gray);
		gridLine.Draw();
	}

	//	#region implement interface
	//
	//	public void OnBeginDrag (PointerEventData eventData)
	//	{
	//		Debug.Log("OnBeginDrag..");
	//	}
	//
	//	public void OnPointerClick (PointerEventData eventData)
	//	{
	//		Debug.Log("on click");
	//	}
	//
	//	#endregion

	void MakeGrid ()
	{
		var numberOfGridPoints = ((Screen.width / unitPixels + 1) + (Screen.height / unitPixels + 1)) * 2;
		gridLine.Resize ((int)numberOfGridPoints);
	
		var index = 0;
		for (var x = 0; x < Screen.width; x += unitPixels) {
			gridLine.points2 [index++] = new Vector2 (x, 0);
			gridLine.points2 [index++] = new Vector2 (x, Screen.height - 1);
		}
		for (var y = 0; y < Screen.height; y += unitPixels) {

			gridLine.points2 [index++] = new Vector2 (0, y);
			gridLine.points2 [index++] = new Vector2 (Screen.width - 1, y);
		}
		
		gridLine.Draw ();
	}

}