
using UnityEngine;

namespace Gameplay.Levels
{
	public static class ExtensionMethods
	{
		public static T RandomElement<T>( this T[] array )
		{
			return array[UnityEngine.Random.Range( 0, array.Length )];
		}
		
		public static Vector3 With( this Vector3 vector, float? x = null, float? y = null, float? z = null )
		{
			return new Vector3( x ?? vector.x, y ?? vector.y, z ?? vector.z );
		}
	}
}
