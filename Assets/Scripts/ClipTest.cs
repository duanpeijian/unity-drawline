using UnityEngine;
using System.Collections.Generic;
using ClipperLib;

namespace ClipperTest {
	using Path = List<IntPoint>; 
	using Paths = List<List<IntPoint>>;
	
	public class ClipTest : MonoBehaviour {
		
		// Use this for initialization
		void Start () {
			Clipper clipper = new Clipper();
			//clipper.AddPath();
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
