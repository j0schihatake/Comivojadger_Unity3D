using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPath : MonoBehaviour {
	
//--------------------------Дабы не запутаться сохраню исходный код комментированным.-----------------------------------

	//расстояния между городами
	//double[,] distance;
	//Матрица..ммм.., матрица x и y.... а я хочу еще добавить высоту....ммм...
	double[,] distance;
	//индексы городов(в моем случае "случайные обьекты - враги") формируют искомый путь
	public int[] Path;

	//Превращаем конструктор в метод возвращающий нам список с поинтами попорядку...
	public List<Point> CPath_GO(List<Point> list_object, CClient client)
	{
		//на вход передаем уже созданные города
		//так map.Coordinate.Lenght... ммм... это у нас коунт листа поинтов...
		distance = new double[list_object.Count, list_object.Count];
		//формируем матрицу расстояний, работать в дальнейшем будем именно с ней
		//Так что тут у нас? Корень квадрата разницы...
		for (int j = 0; j < list_object.Count; j++)
		{
			distance[j, j] = 0;

			for (int i = 0; i < list_object.Count; i++)
			{
				//double value = Math.Sqrt(Math.Pow(map.Coordinate[i].X - map.Coordinate[j].X, 2) +
				//	Math.Pow(map.Coordinate[i].Y - map.Coordinate[j].Y, 2));
				//кароче, запихаю сюда просто дистанцию между точками, и будь что будет...
				double value = Vector3.Distance(list_object[i].position, list_object[j].position);
				distance[i, j] = distance[j, i] = value;
			}
		}

		//создаем начальный путь
		//массив на 1 больше кол-ва городов, а первый и последний индексы равны 0 - это сделано для того чтобы "замкнуть" путь
		Path = new int[list_object.Count];
		for (int i = 0; i < list_object.Count; i++)
		{
			Path[i] = i;
		}
		//Path[list_object.Count-1] = 0;

		//Матрицу сформировали пора и поиск знать:
		FindBestPath();

		//Тут-же возвращаем значение продолжительности пути:
		//client.patch_lenght = (float)PathLength();

		//Возвращаем результат:
		return massiv_to_list(list_object, Path, client);

	}

	//метод, реулизующий алгоритм поиска оптимального пути
	//Теперь значит надо найти где тут формируется список поинтов и дело в шляпе), а вот он же Path
	public void FindBestPath()
	{
		System.Random random = new System.Random();

		for (int fails = 0, F = Path.Length * Path.Length; fails < F; )
		{
			//выбираем два случайных города
			//первый и последний индексы не трогаем
			int p1 = 0, p2 = 0;
			while (p1 == p2)
			{
				p1 = random.Next(1, Path.Length - 1);
				p2 = random.Next(1, Path.Length - 1);
			}

			//проверка расстояний
			double sum1 = distance[Path[p1 - 1], Path[p1]] + distance[Path[p1], Path[p1 + 1]] +
				distance[Path[p2 - 1], Path[p2]] + distance[Path[p2], Path[p2 + 1]];
			double sum2 = distance[Path[p1 - 1], Path[p2]] + distance[Path[p2], Path[p1 + 1]] +
				distance[Path[p2 - 1], Path[p1]] + distance[Path[p1], Path[p2 + 1]];

			if (sum2 < sum1)
			{
				int temp = Path[p1];
				Path[p1] = Path[p2];
				Path[p2] = temp;
			}
			else
			{
				fails++;
			}
		}
	}

	//Ну и мой костыль потому что лениво...)
	List<Point> massiv_to_list(List<Point> empty_list, int[] massiv, CClient client){
		
		List<Point> return_list = new List<Point> ();
		//Перебираем массив:
		//foreach(int element in massiv){
		for(int i = 0; i < massiv.Length; i++){
			return_list.Add (empty_list [massiv[i]]);
		}
			//Debug.Log (element);
			//return_list.Add (empty_list [element]);
		//}

			//так-же прокидываем рассчитанную продолжительность пути:
			client.patch_lenght = get_patch_lenght (return_list);
		return return_list;
	}

	//На этапе расчета distance[] с нулями), поэтому еще один мой костыль
	public float get_patch_lenght(List<Point> patch_List){
		float distance = 0;
		for(int i = 0; i < patch_List.Count; i++){
			if (i <= (patch_List.Count - 2)) {
				distance += Vector3.Distance (patch_List [i].gameObject.transform.position, patch_List [i + 1].gameObject.transform.position);
			}
			if(i == (patch_List.Count - 1)){
				distance += Vector3.Distance (patch_List [i].gameObject.transform.position, patch_List [i - 1].gameObject.transform.position);
			}
		}
		return distance;
	}

	//возвращает длину пути
	public double PathLength()
	{
		double pathSum = 0;
		for (int i = 0; i < Path.Length - 1; i++)
		{
			pathSum += distance[Path[i], Path[i + 1]];
		}
		return pathSum;
	}
}
