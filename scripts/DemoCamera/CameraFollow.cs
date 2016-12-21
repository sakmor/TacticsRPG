//===========================
// Camera Follows the Player
// 攝影機跟隨玩家
//===========================
using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public AudioClip[] au ;
	public AudioClip[] vocal ;
	Vector2 range = new Vector2(-3f,174f) ;
	public Transform target ;


	void Start ()
	{
		GameObject Player;
		Player=GameObject.FindGameObjectWithTag ("Player");
		target = Player.transform;
	}

	void Update(){
		Vector3 pos = transform.position;
		pos.x = target.position.x ;
		pos.x = Mathf.Clamp(pos.x, range.x, range.y) ;
		transform.position = pos ;
	}
	//====================
	// 播放音效 Play sound
	//====================
	public void PlaySound(int id){
		if(GetComponent<AudioSource>().isPlaying){return;}

		GetComponent<AudioSource>().clip = au[id] ;
		GetComponent<AudioSource>().Play() ;
	}
	//====================
	// 播放語音 Play voice
	//====================
	public void VocalPlay(int id){
		if(GetComponent<AudioSource>().isPlaying){return;}

		GetComponent<AudioSource>().clip = vocal[id] ;
		GetComponent<AudioSource>().Play() ;
	}
}
