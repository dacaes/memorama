using UnityEngine;
using System.Collections;

public class CInstanciarCasillas : MonoBehaviour 
{
	#region declaration
	
	//filas de fichas que tiene el juego
	public int filas = 4;
	
	//columnas de fichas que tiene el juego
	public int columnas = 5;
	
	//espacio entre centro de cada ficha (como als fichas miden 10 de lado, al poner 12 de valor de borde queda una separación de 2 entre ellas)
	public int borde = 12;
	
	//array que contendrá las fichas (se utiliza más adelante para mostrar las tapadas en caso de perder la partida)
	[HideInInspector]
	public GameObject[] clones;
	
	 //GameObject que almacenará el ScriptAux
	GameObject GOData;
	
	//Indicador del número de texturas diferentes que hay en la hoja de texturas
	int numeroImagenesHojaTexturas;
	
	//Array de Texture2D donde se guardan las texturas troceadas del textureSheet
	Texture2D[] textureCollection;
	
	/*shader que se le aplica a los planos de las fichas para que no necesiten la luz.
	 * ecesario por el bug de la luz comprobado en clase por el profesor
	 */
	Shader shader;
	
	#endregion
	
	
	#region functions
	
	// Use this for initialization
	void Start ()
	{	
		//inicializa el array clones con el número de fichas que habrá
	  	clones = new GameObject[filas*columnas];
		
		//mete en shader el shader autolimunidado adecuado
		shader = Shader.Find ("Self-Illumin/Diffuse");
		
		getData();
		instantiate(positions(),textures());
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	/// <summary>
    /// busca el objecto ScriptAux que contiene los scripts del juego y lo guarda en GOData.
    /// Coge el textureCollection de CTrocearTexturas y lo guarda en su propio textureCollection.
    /// Coge el numeroImagenesHojaTexturas de CTrocearTexturas y lo guarda en su propio numeroImagenesHojaTexturas.
    /// </summary>
	void getData()
	{		
		GOData = GameObject.Find("ScriptAux");
		CTrocearTexturas texturas = GOData.GetComponent<CTrocearTexturas>();
		this.textureCollection = texturas.textureCollection;
		this.numeroImagenesHojaTexturas = texturas.numeroImagenesHojaTexturas;
	}
	
	/// <summary>
	/// Lleno un array de Vector3 con las posiciones que deberán tomar las casillas.
	/// La 'z' hace de 'y'.
	/// Devuelve el array desordenada por la función correspondiente.
	/// </summary>
	Vector3[] positions()
	{
		//lleno el array position con las posiciones
		Vector3[] position = new Vector3[filas*columnas];
		int posArray = 0;
		
		for (int y = 0; y < filas; y++)
		{
            for (int x = 0; x < columnas; x++)
			{
				position[posArray] = new Vector3(x*borde,0,y*borde);
				posArray++;
            }
        }
		
		//mando el array a randomizar(int[] input) para desordenar las posiciones
		return randomizar(position);
		
	}
	
	/// <summary>
	/// Crea un array de enteros que devuelve randomizados por la función correspondiente.
	/// El array son las texturas de las fichas.
	/// </summary>
	int[] textures()
	{
		int[] array = new int[numeroImagenesHojaTexturas];
		
		for(int i = 0 ; i < numeroImagenesHojaTexturas ; i++)
		{
			array[i] = i;
		}
		return randomizar(array);
	}
	
	/// <summary>
	/// Instancia las fichas para jugar.
	/// Recibe dos arrays.
	/// position son las posiciones desordenadas.
	/// arrayInt es un array desordenado de enteros que son las posiciones del textureCollection de las fichas (interrogante no)
	/// </summary>
	void instantiate(Vector3[] position, int[] arrayInt)
	{
		int num = 1;
		for(int i = 0 ; i < filas*columnas ; i++)
		{
			//instancio el prefab
			GameObject clone = (GameObject) Instantiate(Resources.Load("Prefabs/Casilla"));
			
			//lo meto en el array de clones
		 	clones[i]=clone;
			
			//le doy posición y rotación. La posición
			clone.transform.position = position[i];
			clone.transform.rotation = Quaternion.Euler(0, 180, 0);
			
			//lo nombro con la coletilla num (num augmenta cada dos instantiate, así que tendré parejas de elementos)
			clone.name += ("_" + num);
			
			//le doy textura al reverso de la ficha. Siempre es el interrogante. Le aplico el shader.
			clone.transform.GetChild(1).renderer.material.mainTexture = textureCollection[0];
			clone.transform.GetChild(1).renderer.material.shader = shader;
			
			/*le doy textura al anverso de la ficha que es la posición de textureCollection aleatorizada por arrayInt.
			 * Como depende de num saldrán parejas.
			 * Le aplico el shader
			 * */
			clone.transform.GetChild(0).renderer.material.mainTexture = textureCollection[arrayInt[num]];
			clone.transform.GetChild(0).renderer.material.shader = shader;

			//si es turno impar, num se incrementa en uno(así suma cada dos turnos)
			if(i%2 != 0)
				num++;

		}
	}
	
	/// <summary>
	/// Randomiza el array de enteros que entra y lo devuelve.
	/// El calculo del número random no admite el valor 0 porque es el símbolo del interrogante y función 
	/// se utiliza para sacar posiciones aleatorias del textureCollecion que luego se instanciarán.
	/// </summary>
	int[] randomizar(int[] input)
	{
	   	int[] arrayDesordenado = new int[input.Length];
	 	int num;
		
		for(int i = 0 ; i < input.Length ; i++)
		{
			//se resta i al rango para que así mientras crece el valor de i se acota el cerco y se evita repetir valores
			num = Random.Range(1,input.Length-1-(i));
			
			//se mete el valor aleatorio en el array desordenado
			arrayDesordenado[i] = input[num];
			
			/*se mete el último valor del actual cotejamiento en la posición que ha salido ahora esto
			 *evita que salga otra vez el mismo ya que el número que ha aparecido se elimina del array
			 *de origen y al disminuir el rango del numero aleatorio con el siguiente i++ se cierra
			 *el cerco evitando que el número final esté dos veces en el cálculo aleatorio
			 **/
			input[num] = input[input.Length-1-(i)];
		}
		
		//devuelve un array de enteros que es el input desordenado
		return arrayDesordenado;
	}
	
 
	/// <summary>
	/// Randomiza el array de Vector3 que entra y lo devuelve.
	/// </summary>
	Vector3[] randomizar(Vector3[] input)
	{
	   	Vector3[] arrayDesordenado = new Vector3[input.Length];
	 	int num;
		
		for(int i = 0 ; i < input.Length ; i++)
		{
			//se resta i al rango para que así mientras crece el valor de i se acota el cerco y se evita repetir valores
			num = Random.Range(0,input.Length-1-(i));
			
			//se mete el valor aleatorio en el array desordenado
			arrayDesordenado[i] = input[num];
			
			/*se mete el último valor del actual cotejamiento en la posición que ha salido ahora esto
			 *evita que salga otra vez el mismo ya que el número que ha aparecido se elimina del array
			 *de origen y al disminuir el rango del numero aleatorio con el siguiente i++ se cierra
			 *el cerco evitando que el número final esté dos veces en el cálculo aleatorio
			 **/
			input[num] = input[input.Length-1-(i)];
		}
		
		//devuelve un array de Vector3 que es el input desordenado
		return arrayDesordenado;
	}
	
	#endregion
}
