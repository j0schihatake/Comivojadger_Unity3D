using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//А этот класс нам и не нужен вовсе...
public class CCities : MonoBehaviour {
	//массив коррдинат городов
	public Point[] Coordinate;

	public CCities(int N, int maxValue) //maxValue - размер элемента pictureBox на форме
	{
		System.Random random = new System.Random();

		Coordinate = new Point[N];

		//создаем более узкие границы, чем сам pictureBox, чтобы города не лежали с самого краю
		//так просто визуально приятнее выглядит
		int minBorder = (int)(maxValue * 0.05);
		int maxBorder = (int)(maxValue * 0.95);

		for (int i = 0; i < N; i++)
		{
			Coordinate[i] = new Point(random.Next(minBorder, maxBorder),
				random.Next(minBorder, maxBorder));
		}
	}
}
