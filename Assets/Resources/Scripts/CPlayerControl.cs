using UnityEngine;
using System.Collections;

public class CPlayerControl : MonoBehaviour
{
	#region declaration
	
	//GameObject que almacenará el ScriptAux
	GameObject GOData;
	
	//Ficha seleccionada
	GameObject selectObj;
	
	//la situación del jugador se gestiona por los tres siguientes estados y empieza Jugando
	enum estados {Jugando, Ganado, Perdido};
	estados estado = estados.Jugando; 
	
	//recurso de audio
	public AudioSource audioGame;
	
	//clips con los diferentes sonidos del juego
	public AudioClip y;
	public AudioClip n;
	public AudioClip win;
	public AudioClip lose;
	
	//GameObjects que se utilizan para almacenar las fichas
	GameObject GOSelected;
	GameObject lastGOSelected; 
	
	//condición de victoria (número de parejas a realizar)
	int winnerCond;
	
	//Script de selección de GameObjects
	CSelectGameObject selectGO;
	
	//array de las fichas
	GameObject[] clones;
	
	//tiempo para realizar el juego
	public float segundos = 60;
	
	//premio por acertar
	public float suma = 2;
	
	//penalización por fallar
	public float resta = 1;
	
	//variable que indica cuanto tiempo se muestran las fichas antes de voltearse (no son segundos)
	public float tiempoVuelta = 0.3f;
	
	//GuiTexts del HUD
	public GUIText textTime;
	public GUIText turnos;
	
	//contador de turnos
	int turno = 0;
	
	//curva de Bézier que modula el giro de las fichas
	public AnimationCurve transitionVoltear;
	
	//variable que afecta al tiempo de giro
	public float timeTransition = 0.1f;
	
	//variable que impide jugar si la cámara está haciendo zoom
	public bool zoom =  true;
	
	#endregion
	
	
	#region functions
	
	// Use this for initialization
	void Start ()
	{
		getData();
		
		//valores de inicio de los guitext
		turnos.text = ("Turno: " + turno);
		textTime.text = ("Time: " + (int)segundos);
	}

	
	// Update is called once per frame
	void Update ()
	{	
		if(!zoom)
		{
			if (estado == estados.Jugando)
			{
				gui();
				
				//solo refresco cuandoa aprieto con el ratón
				if(Input.GetMouseButtonDown(0))
				{
					//Actualiza el GameObject de la ficha seleccionada
					selectObj = selectGO.selectGO();
					
					//condición para acceder al refresco
					if(checkData(GOData)==true)
					{
						
						refreshData();
						
						//Si el usuario está pulsando sobre una ficha para verla, se voltea para que se muestre su anverso.
						if(	GOSelected && GOSelected.transform.rotation.z == 0f && GOSelected.tag =="card")
							StartCoroutine (Girar(GOSelected)); 
					}
				}
				
			}
		}
	}
	
	/// <summary>
    /// busca el objecto ScriptAux que contiene los scripts del juego y lo guarda en GOData
    /// Asigna a instanciar el CInstanciarCasillas del ScriptAux.
    /// Recoge el array de fichas.
    /// Establece cuántas parejas hay que realizar para ganar (winnerCond)
    /// </summary>
	void getData()
	{
		GOData = GameObject.Find("ScriptAux");
		CInstanciarCasillas casillas = GOData.GetComponent<CInstanciarCasillas>(); 
		selectGO = GOData.GetComponent<CSelectGameObject>();
		clones = casillas.clones;
		winnerCond = casillas.filas*casillas.columnas/2;
	}
	
	/// <summary>
	/// Función que se encarga de actualizar la GUI (excepto el turno).
	/// Actualiza el contador de segundos.
	/// Actualiza a derrota o a victoria según la condición de éstas.
	/// </summary>
	void gui()
	{
		segundos -= Time.deltaTime;
		textTime.text = ("Time: " + (int)segundos);
		
		if(segundos < 0)
		{
			sonido(lose);
			estado = estados.Perdido;
			textTime.text = ("Has perdido.");
			Invoke ("showBoard",1f);
		}
		
		if(winnerCond == 0)
		{
			sonido(win);
			estado = estados.Ganado;
			textTime.text = ("Has ganado."); 
		}
	}
	
	/// <summary>
	/// Función que se activa si se pierde el juego.
	/// La función recorre todas las fichas y la que no está girada (su tag es "card") se pone de cara.
	/// Con esta función el usuario puede ver al final del juego todas las fichas giradas y así puede buscar las fichas que no encontraba.
	/// </summary>
	void showBoard()
	{
	 	foreach(GameObject clone in clones)
		{
		 	if(clone.tag =="card" )
				StartCoroutine (Girar(clone));
		}
	}
	
	/// <summary>
	/// Mira si la pareja de fichas es igual: si lo son las pone en tag=matched,si no las voltea, ocultándolas otra vez.
	/// Si son pareja se suma tiempo, si no, se resta.
	/// </summary>
	void mirarPareja()
	{
		//si se llaman igual es que son pareja
		if(lastGOSelected.name == GOSelected.name)
		{
			lastGOSelected.tag = "matched";
			GOSelected.tag = "matched";
			
			//se suma la recompensa de segundos por acierto
			segundos += suma;
			
			//la winnerCond baja porque falta una pareja menos por encontrar
			winnerCond--;
			
			//se lanza el sonido de acierto
			sonido(y);
			
			//vaciar GameObjects
			liberarGO();
		}
		
		//fallas y se voltean ambas otra vez
		else
		{
			segundos -= resta;
			
			//se lanza el sonido de fallo
			sonido(n);
			
			//se invoca, con un tiempo prudencial para que el jugador vea las fichas, la función que lanzará el volteo
			Invoke("resetCards",tiempoVuelta);
		 }
		
		//se suma un turno
		masTurno();
	}
	
	/// <summary>
	/// Lanza la corrutina que gira las fichas.
	/// </summary>
	void resetCards()
	{
		StartCoroutine (Girar(GOSelected)); 
		StartCoroutine (Girar(lastGOSelected)); 
	}
	
	/// <summary>
	/// Recibe un clip de sonio y lo reproduce.
	/// </summary>
	void sonido(AudioClip clip)
	{
		audioGame.clip = clip;
		audioGame.Play();
	}
	
	/// <summary>
	/// Función que actualiza el guitext de turnos.
	/// Suma un turno y muestra el cambio en el guitext.
	/// </summary>
	void masTurno()
	{
		turno++;
		turnos.text = ("Turno: " + turno);
	}
 
	/// <summary>
	/// Iguala a null los GameObjects que se utilizan en la comparación.
	/// Esta función se ejecuta una vez comprobado si ambas fichas son pareja.
	/// Al liberar los GameObjects, el script se prepara para recibir una nueva pareja.
	/// </summary>
	void liberarGO()
	{
		this.lastGOSelected = null;
		this.GOSelected = null;
	}
	
	/// <summary>
	/// Corrutina que realiza el giro de las fichas.
	/// El giro se modula con una curva de Bézier.
	/// </summary>
	IEnumerator Girar(GameObject go)
	{
		//establece el ángulo de partida y el objetivo
	 	int angleToReach;
		float angleToStart;
		
	 	angleToStart =  go.transform.rotation.eulerAngles.z;
		
		if(go.transform.rotation.eulerAngles.z == 0)
		 angleToReach = 180; 
		else
		 angleToReach = 0; 
		 
		
		//Declaración de variables necesarias para aplicar la curva de Bézier.
		#region CurveVariables
		
		//Variable que acelerará o decelerará el volteo y que varia según el momento al seguir la curva.
		float currentQuantity;
		
		//Tiempo actual de evaluación de la curva. Oscila de cero a uno
		float currentTimeEvaluate;
		
		//Tiempo de transición actual. Oscila de cero a timeTransition. Se incrementa en cada bucle con Time.deltaTime
		float currentTransitionTime = 0; 
		
		#endregion
		
			
			while (true)
			{
				//Calculo el lugar donde debo evaluar la curva, determinado por el tiempo (eje x)
				currentTimeEvaluate = currentTransitionTime / timeTransition;
				
				//La cantidad la devuelve la evaluación de la curva, determinada por la diferencia entre los dos valores extremos (eje y)
				currentQuantity = transitionVoltear.Evaluate (currentTimeEvaluate);
			 	
				//se ejecuta un "tic" de la rotación influenciado por currentQuantity
				go.transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 180, angleToStart), Quaternion.Euler(0, 180, 	angleToReach), currentQuantity );
	   			
				currentTransitionTime += Time.deltaTime;
				
				//Si llega al final de la curva es que ha realizado el giro, por tanto su tag se cambia en conveniencia. Se sale del bucle.
	  			if(currentQuantity >= 1  )
				{
				    if(angleToReach == 180)
						go.tag="Volteado";
					else
						go.tag="card";
					
					break;
					  
				}
				
				yield return 0;
		 	
			}
			
			//Si están amgos GameObjects y además están volteados, se mira si son pareja.
			if((GOSelected && lastGOSelected) && (GOSelected.tag == "Volteado" && lastGOSelected.tag == "Volteado") && estado != estados.Perdido )
		 		mirarPareja();
	 }	
	
	/// <summary>
	/// Comprueba que no haya fichas volviendo a taparse (girando) y que el usuario esté pulsando sobre una ficha.
	/// Si se cumple que no pasa lo anterior se da paso devolviendo true.
	/// </summary>
	bool checkData(GameObject GObject)
	{
		//no deja voltear si hay fichas volviendo a taparse
		if (GOSelected && lastGOSelected && GOSelected.transform.rotation.z != 0f && lastGOSelected.transform.rotation.z != 0f)
		{
		 	return false;		
		}
		
		//mira si es una ficha
		if(selectObj && selectObj.tag == "card" )
		return true;
		else
		return false;
	}
	
	/// <summary> 
	/// Fución que cambia los dos GameObjects que usamos para comparar las parejas
	/// Si no es la misma ficha que la pulsada la vez anterior la almacena.
	/// Si existe un GOSelected, éste pasa a ser lastGOSelected y el nuevo es GOSelected.
	/// Así se puede completar parejas de cartas que se comprobarán.
	/// </summary>
	void refreshData()
	{
		if(selectObj !=  GOSelected)
		{
			//si no es el primer turno
			if(GOSelected)
			this.lastGOSelected = this.GOSelected;
			
			this.GOSelected = selectObj;
		 }
	 } 	
	
	#endregion
}