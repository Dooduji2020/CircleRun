using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Character {
    Green, Dark_Green, Orange, Blue
}
public class DataManager : MonoBehaviour {
  
  public static DataManager Instance { get; private set; }

  private void Awake() {

    DontDestroyOnLoad(gameObject);
    
  }

  public Character CurrentCharacter;

  }


