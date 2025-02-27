using UnityEngine
using System

public class PlayerController : MonoBehaviour
{
	[SerializeField] private speed = 10f;
	
	private float inputX;
	private float inputY;
	void Update
	{
		float inputX = Input.GetAxis("Horizontal");
		float inputY = Input.GetAxis("Vertical");
		
		Vector3 direction = new Vector3(inputX,inputY,0);
		
		transform.translate(direction * (speed * Time.deltaTime));
	}
}
