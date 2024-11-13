using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallGetInfo : MonoBehaviour
{
   public GolfBallSO golfBallSO;

   public int GetLevel()
   {
      return golfBallSO.level;
   }
}
