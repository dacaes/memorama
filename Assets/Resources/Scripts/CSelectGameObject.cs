using UnityEngine;
using System.Collections;
using System;

public class CSelectGameObject : MonoBehaviour 
{
	#region declaration
	
	//distancia que tiene el rayo que busca GameObjects
	public float distance = 100;
	 
	#endregion
	
	
	#region functions
	
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	/// <summary>
	/// selectGo lanza un rayo y si alcanza algún GameObject, lo devuelve.
	/// </summary>
	public GameObject selectGO()
	{
		//se crea el rayo desde donde aprieto el botón en el marco de la pantalla en dirección paralela a una de las aristas de profunidad del frustum
	 	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	 	RaycastHit hit;

	    //Lanza un rayo hacia adelante la distancia especificada
	    if (Physics.Raycast(ray, out hit, distance))
	    {
			//Devuelvo en GOSelected el gameobject que he tocado.
			return hit.collider.transform.parent.gameObject;
	    }
		else
			return null;
	}
	
	#endregion
}
