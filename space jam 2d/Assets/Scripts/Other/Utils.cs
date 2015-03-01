using UnityEngine;
using System.Collections;

namespace DragsRace.Utils
{
	public static class Utils {

		public static Vector2 ToVector2(Vector3 vector)
		{
			return new Vector2(vector.x, vector.y);
		}

		public static Vector2 ToVector3(Vector2 vector)
		{
			return new Vector3(vector.x, vector.y);
		}
	}
}