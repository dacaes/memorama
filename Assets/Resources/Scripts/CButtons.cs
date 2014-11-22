using UnityEngine;
using System.Collections;

public class CButtons : MonoBehaviour 
{
	#region declaration
	
	//porcentaje que se utiliza para reubicar los botones dentro de la pantalla
	public float porcentaje = 10;
	
	//fuente para el texto de los botones
	public Font font;
	
	//material del que tomarán el color del texto
	public Material mat;
	
	//elemento para personalizar los parámetros de los botones
	GUIStyle guiStyle = new GUIStyle();
	
	#endregion
	
	
	#region functions
	
	// Use this for initialization
	void Start () 
	{
		setGUIStyle();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	/// <summary>
	/// Da los parámetros al guiStyle.
	/// El tamaño depende de la pantalla.
	/// </summary>
	void setGUIStyle()
	{
		guiStyle.font = font;
		guiStyle.normal.textColor = new Color(0.65F, 0.2F, 0.15F, 1.0F);
		guiStyle.fontSize = (int)Camera.mainCamera.pixelWidth*25/800;
	}
	
	/// <summary>
	/// Situa botones en pantalla.
	/// Actúa como un update esperando el evento de pulsar los botones.
	/// Si se pulsan se cumple el if y realiza lo que hay dentro.
	/// </summary>
	void OnGUI() 
	{	
		//Botón con el texto "Exit" que cierra la aplicación al ser pulsado.
		if (GUI.Button(new Rect(Screen.width/(porcentaje*4), Screen.height - Screen.height/(porcentaje), (int)Camera.mainCamera.pixelWidth*60/662, (int)Camera.mainCamera.pixelWidth*30/662), "Exit" , guiStyle))
		{
         	Application.Quit();
		}
		
		//Botón con el texto "Restart" que carga la escena 1 al ser pulsado.
        if (GUI.Button(new Rect(Screen.width - Screen.width/(porcentaje/1.15f), Screen.height - Screen.height/(porcentaje), (int)Camera.mainCamera.pixelWidth*60/662, (int)Camera.mainCamera.pixelWidth*30/662), "Restart" , guiStyle))
		{
         	Application.LoadLevel(1);
		}
    }

	#endregion
}
