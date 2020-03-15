using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {

	public float X;
	public float Y;
	public float Z;

	//Ссылка дочерний обьект с мшем:
	public GameObject hide_object = null;

	//В моем варианте просто можно иметь Vrector3
	public Vector3 position = Vector3.zero;

	//Конструктор
	public Point(float X, float Y){}

	//Так мне то необходима еще одна ось!
	public Point(float X, float Y, float Z){}
}
