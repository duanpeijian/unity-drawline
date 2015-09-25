using UnityEngine;
using System.Collections.Generic;

public class Home : PlainSingleton<Home> {

	private List<Wall2D> walls = new List<Wall2D>();

	public void AddWall(Wall2D wall){
		this.walls.Add(wall);
	}

	public IList<Wall2D> Walls {
		get {
			return walls;
		}
	}
}
