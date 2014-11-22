using UnityEngine;
using System.Collections;

public class CSetText : MonoBehaviour 
{
	#region declaration
	
	//GuiTexts del HUD
	public GUIText textTime;
	public GUIText turnos;

	#endregion
	
	
	#region functions
	
	// Use this for initialization
	void Start () 
	{
		setText();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	/// <summary>
	/// Escala la fuente del texto según el tamaño de la pantalla.
	/// </summary>
	void setText()
	{
		textTime.fontSize = (int)camera.pixelWidth*25/800;
		turnos.fontSize = (int)camera.pixelWidth*25/800;
	}
	
	#endregion
}
