using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth 
{
   //Creating this interface because in the future, additional health mechanics can be added to the specific world canvas for example... 
   public void DecreaseHealth();
}
