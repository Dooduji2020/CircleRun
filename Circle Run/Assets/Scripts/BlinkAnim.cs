using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BlinkAnim : MonoBehaviour {

    
	TextMeshProUGUI flashingText;

	// Use this for initialization
	private void Start () {
		flashingText = GetComponent<TextMeshProUGUI> ();
		StartCoroutine (BlinkText());
	}

	private IEnumerator BlinkText(){
		while (true) {
			flashingText.text = "";
			yield return new WaitForSeconds (.5f);
			flashingText.text = "TAP TO PLAY";
			yield return new WaitForSeconds (.5f);
		}
	}
  /* public TMP_Text textComponent;

   private void Update() {

    textComponent.ForceMeshUpdate();
    var textInfo = textComponent.textInfo;

    for (int i = 0; i < textInfo.characterCount; ++i) {

        var charInfo = textInfo.characterInfo[i];

        if(!charInfo.isVisible) {

            continue;

        }

        var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

        for (int j = 0; j < 4; ++j) {

            var orig = verts[charInfo.vertexIndex + j];
            verts[charInfo.vertexIndex +j] = orig + new Vector3(0, Mathf.Sin(Time.time*2f + orig.x*0.01f) * 10f, 0);
        }
    }
    
    for (int i = 0; i < textInfo.meshInfo.Length; ++i) {

        var meshInfo = textInfo.meshInfo[i];
        meshInfo.mesh.vertices = meshInfo.vertices;
        textComponent.UpdateGeometry(meshInfo.mesh, i);
    }
   }*/
}
