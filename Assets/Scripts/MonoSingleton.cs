using UnityEngine;
using System.Collections;
using System;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>{
	
	private static T _instance;
	
	void Awake(){
		_instance = (T)this;
		this.Init();
	}
	
	public static T Get() {
		if(_instance == null){
			Debug.LogWarning(string.Format("type: {0} has not initialized", typeof(T)));
			return null;
		}
		return _instance;
	}
	
	protected virtual void Init() {
		
	}
}

public class PlainSingleton<T> : System.Object where T : PlainSingleton<T>{

	private static T _instance;

	public static T Get() {
		if(_instance == null){
			_instance = Activator.CreateInstance<T>();
			_instance.Init();
		}

		return _instance;
	}

	protected virtual void Init() {
	}
}
