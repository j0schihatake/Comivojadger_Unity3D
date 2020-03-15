using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
	
	//Ссылка на скрипт камеры:
	public CameraScript camera_script = null;

	//Ссылка на реализацию поиска:
	public CPath patch = null;
	public CClient player_client = null;

	private bool pause = false;

	//Ссылка на стартовую позицию игрока;
	public GameObject start_Point_Object = null;
	Point start_Point = null;
	public Transform center_Transform = null;
	public GameObject enemy_parent_prefab = null;
	//Это старый уровень:
	public GameObject lost_level = null;

	//Ссылка на префаб обьекта игрока:
	//public GameObject player_prefab = null;
	public GameObject player = null;

	//переключатель таймера:
	public bool timer_bool = false;
	public bool movement_timer_bool = false;
	public float move_times = 0f;
	public float times = 0f;
	//Рассчитываем период для new WaitForSeconds:
	public float time_to_one_object = 0f;

	//Пройденный путь:
	public float lenght_patch = 0f;

	//Список доступных обьектов противников(случайные обьекты из тестового задания)
	public List<GameObject> enemy_prefab_List = new List<GameObject> ();
	public List<GameObject> level_enemy_list = new List<GameObject> ();

	//Список поинтов:
	public List<Point> mapPointList = new List<Point> ();
		
	//Задаваемый диапазон:
	public int diapason_x = 1;
	public int diapason_z = 1;
	//Диапазон Y с целью ускорения разработки будет захватывать лишь положительные значения
	public int diapason_Y = 1;

	//возможность задать число противников:
	public int enemy_count = 0;

	//---------------UI
	public Text x_diapason_text = null;
	public Text y_diapason_text = null;
	public Text z_diapason_text = null;
	public Text enemy_count_text = null;
	public Text level_timer = null;
	//Статистика:
	public Text timer_text = null;
	public Text lenght_text = null;
	public Text record_text = null;

	public GameObject main_menu = null;
	public GameObject go_Panel = null;

	void Start ()
	{
		//Отображаем прошлый рекорд:
		start_Point = start_Point_Object.GetComponent<Point> ();
		updateGUI (diapason_x, diapason_Y, diapason_z, enemy_count);
		main_menu.SetActive (true);
		go_Panel.SetActive (false);
	}

	//Этот метод будет учитывая диапазон выдавать случайное местоположение.
	Vector3 get_random_position (int x, int y, int z)
	{
		//Для скорости разработки не замарачиваюсь с подгонкой рандома а использую стандартный юнитевский
		Vector3 return_Vector = Vector3.zero;

		return_Vector = new Vector3 ((Random.Range (0, x)), (Random.Range (0, y)), (Random.Range (0, z)));

		return return_Vector;
	}

	//Отслеживаем таймер
	void Update ()
	{
		//Таймер глобалного времени:
		if (timer_bool) {
			if (times > 0) {
				times -= Time.deltaTime;
				//Ну и обновляем значение на UI:
				level_timer.text = times.ToString ();
			}
			if (times < 0) {
				times = 0f;
				level_timer.text = times.ToString ();
				//достчитали отключаем.
				timer_bool = false;
			}
		}
		//Таймер рассчитывающий время перемещения:
		if (movement_timer_bool) {
			move_times += Time.deltaTime;
			//Ну  отображаем на UI:
			timer_text.text = move_times.ToString ();
		}
	}

	//----------------------------------------------------UI_Методы---------------------------------------------------------
	public void On_Button_Start ()
	{
		main_menu.SetActive (false);
		go_Panel.SetActive (true);
	}

	public void On_Pause_Button ()
	{
		if (pause) {
			//Раз уже на паузе, разлочиваем:
			Time.timeScale = 1.0f;
		} else {
			Time.timeScale = 0.2f;
		}
	}

	//Нажатие кнопки Go, запускает выполнение алгоритма описанного в тестовом задании.
	public void On_Button_Go ()
	{
		//Помещаем игрока в стартовую позицию(0,0,0)
		player.transform.position = start_Point_Object.transform.position;
		//Первым делом запускаем таймер:
		//Теперь расставляем и через несколько секунд скрываем "случайные обьекты"
		StartCoroutine ("create_object");
	}

	//Метод расставляет и скрывает через секунды обьекты:
	IEnumerator create_object ()
	{
		//Уничтожаем старые обьекты, сделаю так попробую удалить родительский обьект;
		if (lost_level != null) {
			DestroyObject (lost_level);
			lost_level = null;
		}
		//Создаем новые обьекты:
		lost_level = (GameObject)Instantiate (enemy_parent_prefab, enemy_parent_prefab.transform.position, Quaternion.identity);
		player_client.targetPosition = null;
		player_client.mapPointList.Clear ();
		mapPointList.Clear ();

		//Так как весь цикл должен быть уложен в 30с, разделяем время на обьекты:
		time_to_one_object = 30f / (float)enemy_count;

		//Сбрасываем таймер времени перемещения:
		move_times = 0f;
		movement_timer_bool = false;

		//запускаем глобальный таймер:
		times = 30f;
		level_timer.text = times.ToString ();
		timer_bool = true;

		//Теперь расставляем обьекты:
		for (int i = 0; i < enemy_count; i++) {
			int random_Object = (int)Random.Range (0, enemy_prefab_List.Count);
			//Создаю обьекты:
			GameObject rand_object = enemy_prefab_List [random_Object];
			GameObject clone = (GameObject)Instantiate (rand_object, (Vector3)get_random_position (diapason_x, diapason_Y, diapason_z), rand_object.transform.rotation);
			clone.transform.SetParent (lost_level.transform);
			//добавляем поинты в список:
			mapPointList.Add (clone.GetComponent<Point> ());
			level_enemy_list.Add (rand_object);
			yield return new WaitForSeconds (time_to_one_object);
			//Скрываем обьект через секунды(но решил по другому после создания, все обьекты разом исчезают;).
			//clone.GetComponent<Point>().hide_object.SetActive(false);
		}
		//надо проверить убрали ди утечку памяти в foreach
		foreach (Point point in mapPointList) {
			point.hide_object.SetActive (false);
		}
		//отдаем клиенту список созданных поинтов:
		player_client.mapPointList = this.mapPointList;
		//Теперь отправляем нашего клента проходить поинты(закидываем ему чтонибудь, я для красоты сделал так):
		player_client.targetPosition = start_Point.gameObject.transform;
		movement_timer_bool = true;
		yield return null;
	}

	//Методы UI Диапазон по оси x:
	public void add_Count_Diapason_X ()
	{
		diapason_x += 1;
		updateGUI (diapason_x, diapason_Y, diapason_z, enemy_count);
	}

	public void remove_Count_Diapason_X ()
	{
		if ((diapason_x - 1) >= 0) {
			diapason_x -= 1;
			updateGUI (diapason_x, diapason_Y, diapason_z, enemy_count);
		}
	}

	//Методы UI Диапазон по оси y:
	public void add_Count_Diapason_Y ()
	{
		diapason_Y += 1;
		updateGUI (diapason_x, diapason_Y, diapason_z, enemy_count);
	}

	public void remove_Count_Diapason_Y ()
	{
		if ((diapason_Y - 1) >= 0) {
			diapason_Y -= 1;
			updateGUI (diapason_x, diapason_Y, diapason_z, enemy_count);
		}
	}

	//Методы UI Диапазон по оси Z:
	public void add_Count_Diapason_Z ()
	{
		diapason_z += 1;
		updateGUI (diapason_x, diapason_Y, diapason_z, enemy_count);
	}

	public void remove_Count_Diapason_Z ()
	{
		if ((diapason_z - 1) >= 0) {
			diapason_z -= 1;
			updateGUI (diapason_x, diapason_Y, diapason_z, enemy_count);
		}
	}

	//Методы по изменению количества врагов:
	public void add_Enemy_Count ()
	{
		enemy_count += 1;
		updateGUI (diapason_x, diapason_Y, diapason_z, enemy_count);
	}

	public void remove_Enemy_Count ()
	{
		if (enemy_count - 1 > 4) {
			enemy_count -= 1;
			updateGUI (diapason_x, diapason_Y, diapason_z, enemy_count);
		}
	}

	//центрируем камеру на игроке:
	public void center_To_Player ()
	{
		if (camera_script.cameraTypes == CameraScript.TypesCamera.orbit) {
			camera_script.FolowObject = player.transform;
			camera_script.cameraTypes = CameraScript.TypesCamera.followtounit;
		} else {
			center_Transform.position = new Vector3 (diapason_x / 2, diapason_Y / 2, diapason_z / 2);
			camera_script.FolowObject = center_Transform;
			camera_script.cameraTypes = CameraScript.TypesCamera.orbit;
		}
	}
	//Обновляем отображаемые параметры диапазона:
	private void updateGUI (int x, int y, int z, int enemy)
	{
		x_diapason_text.text = x.ToString ();
		y_diapason_text.text = y.ToString ();
		z_diapason_text.text = z.ToString ();
		enemy_count_text.text = enemy_count.ToString ();
	}

	//Метод отработает в конце пути:
	public void on_end_Move ()
	{
		movement_timer_bool = false;
		//И считываемпродолжительность пути:
		lenght_text.text = player_client.patch_lenght.ToString ();
	}

	//Выход из приложения:
	public void on_Button_Quit ()
	{
		Application.Quit ();
	}
}
