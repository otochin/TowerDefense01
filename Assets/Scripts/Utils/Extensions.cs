using UnityEngine;

/// <summary>
/// ユーティリティ拡張メソッド
/// </summary>
public static class Extensions
{
    /// <summary>
    /// GameObjectが指定されたタグを持っているかチェック
    /// </summary>
    public static bool HasTag(this GameObject obj, string tag)
    {
        return obj.CompareTag(tag);
    }
    
    /// <summary>
    /// Transformから指定されたタグの子オブジェクトを検索
    /// </summary>
    public static Transform FindChildWithTag(this Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
                return child;
            
            Transform result = child.FindChildWithTag(tag);
            if (result != null)
                return result;
        }
        
        return null;
    }
    
    /// <summary>
    /// Vector3のX座標のみ変更
    /// </summary>
    public static Vector3 WithX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }
    
    /// <summary>
    /// Vector3のY座標のみ変更
    /// </summary>
    public static Vector3 WithY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }
    
    /// <summary>
    /// Vector3のZ座標のみ変更
    /// </summary>
    public static Vector3 WithZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }
    
    /// <summary>
    /// 2つのVector3の距離を計算（2D用、Z座標を無視）
    /// </summary>
    public static float Distance2D(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x, b.y));
    }
}
