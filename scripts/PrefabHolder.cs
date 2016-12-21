using UnityEngine;
using System.Collections;

public class PrefabHolder : MonoBehaviour {
	public static PrefabHolder instance;

	public GameObject BASE_TILE_PREFAB;//基本外框
	//----------------------------//
	public GameObject Normal1;
	public GameObject Normal2;
	public GameObject Normal3;
	public GameObject Normal4;
	public GameObject Bridge;
	//------------------以下不可走------------------------//
	public GameObject Impassible_Empty;
	public GameObject Impassible_FakeNomal;
	public GameObject Impassible_pillar;
	public GameObject Impassible_matelWall_col;
	public GameObject Impassible_matelWall_row;
	public GameObject Impassible_RockWall_col;
	public GameObject Impassible_RockWall_row;
	public GameObject Impassible_RockWall_row_light;
	public GameObject Impassible_RockWall_col_light;
	public GameObject Impassible_Door_col;
	public GameObject Impassible_Door_row;
	//-------------------------------------------------------//
	public GameObject Impassible;








	void Awake()
	{
		instance = this;
	}
}
