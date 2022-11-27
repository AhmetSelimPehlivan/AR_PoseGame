using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point {
   public ObjectType type;
   public GameObject prefab ;
   [Range (0f, 100f)]public float Chance = 100f ;
   [HideInInspector] public double _weight ;
}
public enum ObjectType{
   Point,
   Bomb,
   Time
}

public class CircleControler : MonoBehaviour {
   [SerializeField] private Canvas _screen;
   [SerializeField] private Point[] points ;

   private double accumulatedWeights ;
   private System.Random rand = new System.Random () ;


   private void Awake () {
      CalculateWeights () ;
   }

   private void Start () {
      InvokeRepeating(nameof(Spawn), 3.0f, 3.0f);
   }
   
    void Spawn(){
        Point currentPoint = points [ GetRandomEnemyIndex () ] ;
        Instantiate(currentPoint.prefab, RandomCircle(_screen.transform.localPosition), _screen.transform.rotation, _screen.transform);
    }

   private int GetRandomEnemyIndex () {
      double r = rand.NextDouble () * accumulatedWeights ;

      for (int i = 0; i < points.Length; i++)
         if (points [ i ]._weight >= r)
            return i ;
      return 0 ;
   }

   private void CalculateWeights () {
      accumulatedWeights = 0f ;
      foreach (Point point in points) {
         accumulatedWeights += point.Chance ;
         point._weight = accumulatedWeights ;
      }
   }

   private Vector3 RandomCircle ( Vector3 center ){
      Debug.Log("center.z "+center.z);  
      return new Vector3(center.x + Random.Range(-12.0f, 10.0f), center.y + Random.Range(-5.0f, 2.0f), center.z);
   }
}