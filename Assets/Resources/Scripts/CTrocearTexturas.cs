using UnityEngine;
using System.Collections;

//script basado y optimizado en hojas de texturas cuadradas con texturas cuadradas
public class CTrocearTexturas : MonoBehaviour 
{
	#region declaration
	
	//hoja de texturas a dividir (se ha de indicar desde el editor)
	public Texture2D textureSheet;
	
	//Indicador del número de texturas diferentes que hay en la hoja de texturas (se ha de indicar en el editor)
	public int numeroImagenesHojaTexturas = 64;

	//tamaño del lado de las texturas, si no fueran cuadradas se tendría que hacer ancho y alto.
	int lado;
	
	int fila;
	int columna;
	int index;
	
	//Array de Texture2D donde se guardan las texturas troceadas del textureSheet
	//[HideInInspector]
	public Texture2D[] textureCollection;
	
	#endregion
	
	
	#region functions
	
	// Use this for initialization
	void Start () 
	{
		//el tamaño del lado se establece dividiendo el ancho del textureSheet entre el número de columnas
		lado = (int)textureSheet.width/(int)Mathf.Sqrt(numeroImagenesHojaTexturas);
		
		trocearTexturas();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	/// <summary>
	/// Función que crea el array de texturas 'textureCollection'y lo llena con los trozos cogidos de la 'textureSheet'
	/// </summary>
	void trocearTexturas()
	{
		//instancia array
		textureCollection = new Texture2D[numeroImagenesHojaTexturas];
		
		for(index=0;index<numeroImagenesHojaTexturas;index++)
		{
			//crea una nueva Texture2D (con el tamaño que deseamos) en la posición del array
			textureCollection[index] = new Texture2D(lado, lado);
			
			//mira en que fila y en que columna está (hallar coordenadas)
			fila = index / (int)Mathf.Sqrt(numeroImagenesHojaTexturas);
			columna = index % (int)Mathf.Sqrt(numeroImagenesHojaTexturas);
			
			//busca el pixel superior izquierdo del trozo que quiere meter en el array y con lado le da un volumen de ancho y alto de pixeles a coger
			textureCollection[index].SetPixels(textureSheet.GetPixels(columna * lado,fila * lado,lado,lado));
			//Aplica los píxeles cogidos en el texture2D que hay en el array
			textureCollection[index].Apply();
		}
	}
	
	#endregion
}
