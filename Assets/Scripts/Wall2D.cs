using UnityEngine;
using System.Collections;

public class Wall2D {
	private float startX;
	private float startY;
	private float endX;
	private float endY;
	private Wall2D wallAtStart;
	private Wall2D wallAtEnd;
	private Color color = Color.red;

	public Wall2D(Vector2 start, Vector2 end){
		this.startX = start.x;
		this.startY = start.y;
		this.endX = end.x;
		this.endY = end.y;
	}

	public Vector2 StartPos {
		get {
			return new Vector2(this.startX, this.startY);
		}
		set {
			this.startX = value.x;
			this.startY = value.y;
		}
	}

	public Vector2 EndPos {
		get {
			return new Vector2(this.endX, this.endY);
		}
		set {
			this.endX = value.x;
			this.endY = value.y;
		}
	}

	float StartX {
		get {
			return this.startX;
		}
		set {
			startX = value;
		}
	}

	float StartY {
		get {
			return this.startY;
		}
		set {
			startY = value;
		}
	}

	float EndX {
		get {
			return this.endX;
		}
		set {
			endX = value;
		}
	}

	float EndY {
		get {
			return this.endY;
		}
		set {
			endY = value;
		}
	}

	public Wall2D WallAtStart {
		get {
			return this.wallAtStart;
		}
		set {
			this.wallAtStart = value;
			value.wallAtEnd = this;
		}
	}

	public Wall2D WallAtEnd {
		get {
			return this.wallAtEnd;
		}
		set {
			this.wallAtEnd = value;
			value.wallAtStart = this;
		}
	}

	public Color Color {
		get {
			return this.color;
		}
		set {
			color = value;
		}
	}
	
}
