using UnityEngine;
using System.Collections;

public class CCameraController : MonoBehaviour 
{
    #region declaration
    
    //Variable que almacenará el centro del tablero de juego.
    Vector3 vecCentro;
    
    //doy paso a hacer zoom
    bool zoom = true;
    
    //borde que hay entre las fichas
    int correcBorde;
    
    //GameObject que almacenará el ScriptAux
    GameObject GOData;
    
    //Distancia a la que "parte" la cámara (es el ortographicSize)
    public float distance = 400;
    
    //multiplicador que acentua (si es >1) o disminuye (<1) la velocidad de "acercamiento" de la cámara (ortographicSize)
    public float speed = 100;
    
	//margen que se deja para poner los botones en horizontal
	public float margin = 12;
    #endregion
    
    
    #region functions
    
    // Use this for initialization
    void Start () 
    {
        getData();
        posicionar();
    }
    
    // Update is called once per frame
    void Update () 
    {
        if(zoom)
        {
            setSize();
            frustumCondition();
        }        
    }
    
    /// <summary>
    /// busca el objecto ScriptAux que contiene los scripts del juego y lo guarda en GOData
    /// Asigna a instanciar el CInstanciarCasillas del ScriptAux.
    /// Halla el centro del tablero de juego (conjunto de fichas) y lo guarda en vecCentro
    /// Halla el espacio que hay entre fichas y lo guarda en correcBorde
    /// </summary>
    void getData()
    {        
        GOData = GameObject.Find("ScriptAux");
        CInstanciarCasillas instanciar = GOData.GetComponent<CInstanciarCasillas>();
        this.vecCentro = new Vector3 (instanciar.borde*(instanciar.columnas-1)/2,0,instanciar.borde*(instanciar.filas-1)/2);
        this.correcBorde = instanciar.borde -10;
    }
	
	/// <summary>
	/// Posiciona la cámara en el centro del tablero de juego y a cierta distancia de éste.
	/// La distancia es 10, aunque podría ser otro número dentro de unos límites razonables de la escala del mundo.
	/// Solamente es necesario que el valor de distancia sea mayor que una distancia mínima. A partir de 0.5f no da problemas.
	/// </summary>
	void posicionar()
    {
        this.transform.position = new Vector3 (vecCentro.x,10,vecCentro.z);
        this.transform.gameObject.camera.orthographicSize = distance;
    }
	
	/// <summary>
	/// Función que reduce el ortographicSize.
	/// La función se estabiliza con el deltaTime (para no depender de velocidad de procesamiento).
	/// La variable speed es un acelerador o decelerador de la velocidad a la que se reduce el ortographicSize.
	/// </summary>
	void setSize()
    {
        this.camera.orthographicSize-= Time.deltaTime*speed;
    }
	
    /// <summary>
    /// Miro si el marco de la cámara ha llegado a su posición,
    /// si lo ha hecho pongo 'zoom' a false para evitar que en el siguiente update se reduzca el ortographicSize mediante setSize()
    /// La primera ficha (esquina inferior izquierda) siempre se instancia en el (0,0,0).
    /// El lado de las fichas tiene un tamaño de 10 (inamovible), por tanto al situarse la primera en (0,0,0) 
    /// sabemos que la esquina inferior izquierda de ésta está en el (-5,0-5) (trabajamos sobre el plano de la vista TOP (x,z))
    /// Para no comernos las fichas y que se vean todas se ha de comprobar que el marco de la cámara, por la izquierda y por debajo no se pase de -5.
    /// Esa es la condición que se contempla en ésta función.
    /// Además se introduce el modificador correcBorde, que hace que el marco deje de estrecharse soble el tablero un poco antes.
    /// </summary>
    void frustumCondition()
    {    
        //no hago el cambio de y por z al encontrar el ViewportToWorldPoint porque toma su posición localmente en la cámara. La rotación de ésta en relación al mundo es otro tema.
        if(this.camera.ViewportToWorldPoint(new Vector3(0.5f,0,this.camera.nearClipPlane)).z > (-5 - correcBorde) || this.camera.ViewportToWorldPoint(new Vector3(0,0.5f,this.camera.nearClipPlane)).x > (-5 - correcBorde - margin))
		{
			zoom = false;
			
        	/*CPlayerControl también tiene 'zoom' y aquí se posiciona en falso
        	 * El 'zoom' de CPlayerControl sirve para evitar que el jugador juegue mientras se situa la cámara
        	 */
       		CPlayerControl player = GOData.GetComponent<CPlayerControl>();
        	player.zoom = false;
		}
    }
	
    #endregion
}