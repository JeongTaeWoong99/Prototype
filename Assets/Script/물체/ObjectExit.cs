using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectExit : MonoBehaviour
{
    public string     seenName;					                // 새로운 씬 이름
    public GameObject message;                                  // 안내 메세지 
    public float      waitToLoad;                               // 씬 전환 시간
    
    private bool      inZoon;                                   // 범위 들어왔는지 여부

	private void Update()
	{
        if (inZoon)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(LevelEnd());
            }
        }
    }
    
    private IEnumerator LevelEnd()
    {
        UIController.instance.fadeToBlack = true;      // 화면 어둠게

        yield return new WaitForSeconds(waitToLoad);    // 씬 전환 딜레이
        SceneManager.LoadScene(seenName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            message.SetActive(true);
            inZoon = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            message.SetActive(false);
            inZoon = false;
        }
    }
}