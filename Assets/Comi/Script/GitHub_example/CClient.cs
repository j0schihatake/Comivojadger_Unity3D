using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CClient : MonoBehaviour {
	//Итак это реализация тестового обьекта конкретно из тестового задания. 
	public bool moveRandom = true;

	//Ссылка на скрипт лине рендера
	public LineRenderer liner = null;
	public int liner_index = -1;

	public float patch_lenght = 0f;

	public CPath patcher = null;
	//появилась необходимость отметить последний путь:
	public bool end_point = false;

	public Main main = null;

	//Следующая позиция:
	public Vector3 nextPosition = Vector3.zero;
	public Transform targetPosition = null;
	public Vector3 lostPointVector3D = Vector3.zero;
	public Point lostPoints = null;

	public List<Point> mapPointList = null;
	public List<Point> patch = new List<Point> ();            //итоговый список поинтов пути
	public int countPoint = 0;

	//для предотвращения постоянного создания векторов:
	public Vector3 returned = Vector3.zero;
	public Vector3 testVector = Vector3.zero;
	public Vector3 lostPoint = Vector3.zero;

	//ссылка на риджидбоди:
	private Rigidbody myRigidbody = null;

	public float stopDistance = 0.1f;  //большее значение этого параметра предотвратит "проскакивание"

	public bool IsMoving = false;

	public float speed = 10f;

	void Awake(){
		myRigidbody = this.gameObject.GetComponent<Rigidbody>();
		lostPointVector3D = myRigidbody.transform.position;
		//lostPoint.y += 0.5f;
		nextPosition = myRigidbody.transform.position;
	}

	void Update()
	{
		//Если я закину какую то позицию:
		if (targetPosition != null)
		{
			if (patch.Count == 0) {
				//запускаем поиск пути и получаем список поинтов:
				patch = patcher.CPath_GO (mapPointList, this);
				lostPoint = this.gameObject.transform.position;
				end_point = false;
				if(liner == null){
					//Настраем Linr_renderer:
					patch [0].gameObject.AddComponent<LineRenderer> ();
					liner = patch [0].gameObject.GetComponent<LineRenderer> ();
					liner.startWidth = 0.03f;
					liner.endWidth = 0.03f;
					//добавляем первую позицию для стрта отсчета:
					liner.positionCount = patch.Count;
					for(int i = 0; i < patch.Count; i++){
						liner.SetPosition (i,patch[i].transform.position);
					}
					//Временно отключаем LineRenderer:
					liner.enabled = false;
					//liner_index = 0;
					//update_patch (this.gameObject.transform.position);
				}
			}
		}
		//Боту нужно следовать от точки к точке:
		if (patch.Count > 0) {
			if (!IsMoving)
			{
				if (patch.Count > 0) {
					countPoint = patch.Count - 1;
					//Ищем последнюю точку:
					lostPoint = nextPosition;
					nextPosition = patch[countPoint].gameObject.transform.position;
					if(countPoint == 0){end_point = true;}
				}
			}
		}
		move(nextPosition);
	}

	//Выполнение перемещения к точке:
	void move(Vector3 nextPosition)
	{
		if (myRigidbody.transform.position != nextPosition)
		{
			IsMoving = true;
			myRigidbody.transform.position = Vector3.MoveTowards(myRigidbody.transform.position, nextPosition, speed * Time.deltaTime);
		}
		else
		{
			if (patch.Count > 0)
			{
				//Удаляем последний в списке поинт:
				patch.RemoveAt(patch.Count - 1);
			}
			if (targetPosition != null) {
				if (lostPointVector3D == targetPosition.position) {
					targetPosition = null;
				}
			}
			//Теперь ее и помечаем пройденный поинт:
			//Ну и всякие завершающие подчищения:
			if(patch.Count == 0 & end_point){
				targetPosition = null;
				//Отмечаем пройденный путь:
				if(liner != null){
				liner.enabled = true;
				}
				liner = null;
				main.on_end_Move ();
			}
			nextPosition = myRigidbody.transform.position;
			lostPointVector3D = nextPosition;
			lostPoint = Vector3.zero;
			IsMoving = false;
		}
	}

	void update_patch(Vector3 add_position){
		liner.positionCount += 1;
		liner_index += 1;
		liner.SetPosition(liner_index, add_position);

	}

}
