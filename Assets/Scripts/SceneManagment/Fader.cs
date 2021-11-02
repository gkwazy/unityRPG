using UnityEngine;
using System.Collections;
using System;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentFade;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        public IEnumerator FadeOut(float time)
        {
           if (currentFade != null){
               StopCoroutine(currentFade);
           }
           currentFade = StartCoroutine(FadeoutRoutine(time));
           yield return currentFade;
        }

        private IEnumerator FadeoutRoutine(float time)
        {
            while(canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            if (currentFade != null)
            {
                StopCoroutine(currentFade);
            }
            currentFade = StartCoroutine(FadeInRoutine(time));
            yield return currentFade;


        }

        private IEnumerator FadeInRoutine(float time)
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}