using UnityEngine;

public class Background : MonoBehaviour
{
	private float backX;
	public  float speed;

	private float beforPosCheckTime = 0.1f;
	private float checkTimeCount;
	private float afterMove;
	private float beforMove;
	
	void Update()
	{
		// 움직임 체크
		afterMove = PlayerController.instance.transform.position.x;  
		checkTimeCount += Time.deltaTime;
		if (checkTimeCount > beforPosCheckTime)
		{
		    beforMove = PlayerController.instance.transform.position.x;                                     // 이동 후 값
		    checkTimeCount = 0f;
		}

		// 배경이동 + beforPosCheckTime 이전 위치에서 0.1f보다 이동하지 않았다면, 이동하지 않음.
		// transform 이동이라, timescale이 0.0f여도 움직임
		if (PlayerController.instance.gameObject.activeInHierarchy != false &&
			Vector2.Distance(new Vector2(afterMove,0), new Vector2(beforMove,0)) > 0.1f &&
			UIEvent.instance.eventState == true)
		{
			backX = transform.position.x;
			backX += (speed * PlayerController.instance.inputX) * 0.01f;
			transform.position = new Vector3(backX, transform.position.y, transform.position.z);
		}

		// 멀어지면, 다시 위치 조정
		if (PlayerController.instance.transform.position.x - transform.position.x <= -11f)
		{
			transform.position = new Vector3(PlayerController.instance.transform.position.x - 7.5f, transform.position.y, transform.position.z);
		}
		else if (transform.position.x - PlayerController.instance.transform.position.x <= -11f)
		{
			transform.position = new Vector3(PlayerController.instance.transform.position.x + 7.5f, transform.position.y, transform.position.z);
		}
	
	}

}