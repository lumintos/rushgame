using UnityEngine;
using System.Collections;

public class Size{

	public Size(int w,int h)
	{
		width = w;
		height = h;
	}

	public Size()
	{
		width = 0;
		height = 0;
	}

	int width;

	public int Width {
		get {
			return width;
		}
		set {
			width = value;
		}
	}

	int height;

	public int Height {
		get {
			return height;
		}
		set {
			height = value;
		}
	}
}
