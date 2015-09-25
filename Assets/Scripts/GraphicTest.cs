using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class GraphicTest : Graphic, IPointerClickHandler, IDragHandler {

	public Action<PointerEventData> onDrag;

	public void OnPointerClick (PointerEventData eventData)
	{
		//Debug.Log("onClick..");
	}

	public void OnDrag (PointerEventData eventData)
	{
		//Debug.Log("GraphicTest..OnDrag: " + eventData.delta);
		if(onDrag != null){
			onDrag(eventData);
		}
	}
}
