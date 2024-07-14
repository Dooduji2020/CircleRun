using System.Collections;
using UnityEngine;

public class ScoreStar : MonoBehaviour {
   
   private void Start() {

    ColorChanged(GameManager.Instance.CurrentColor);

   }

   private void OnEnable() {

    GameManager.Instance.GameEnded += OnGameEnded;
    GameManager.Instance.ColorChanged += ColorChanged;
   }

   public void OnGameEnded() {

    GetComponent<Collider2D>().enabled = false;
   }

   private void ColorChanged(Color col) {

    GetComponent<SpriteRenderer>().color = col;
   }
}
