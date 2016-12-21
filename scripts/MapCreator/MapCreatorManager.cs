using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapCreatorManager : MonoBehaviour {
	public static MapCreatorManager instance;

	public int mapSize;
	public List <List<Tile>> map = new List<List<Tile>>();

	public TileType palletSelection = TileType.Normal1;

	Transform mapTransform;

	void Awake () {
		instance = this;
		mapTransform = transform.FindChild("Map");
		generateBlankMap(25);
	}

	void generateBlankMap(int mSize) {
		mapSize = mSize;

		//initially remove all children
		for(int i = 0; i < mapTransform.childCount; i++) {
			Destroy (mapTransform.GetChild(i).gameObject);
		}

		map = new List<List<Tile>>();
		for (int i = 0; i < mapSize; i++) {
			List <Tile> row = new List<Tile>();
			for (int j = 0; j < mapSize; j++) {
				Tile tile = ((GameObject)Instantiate(PrefabHolder.instance.BASE_TILE_PREFAB, new Vector3(i - Mathf.Floor(mapSize/2),0, -j + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
				tile.transform.parent = mapTransform;
				tile.gridPosition = new Vector2(i, j);
				tile.setType(TileType.Normal1);
				row.Add (tile);
			}
			map.Add(row);
		}
	}

	void loadMapFromXml() {
		MapXmlContainer container = MapSaveLoad.Load("map.xml");

		mapSize = container.size;
		
		//initially remove all children
		for(int i = 0; i < mapTransform.childCount; i++) {
			Destroy (mapTransform.GetChild(i).gameObject);
		}

		map = new List<List<Tile>>();
		for (int i = 0; i < mapSize; i++) {
			List <Tile> row = new List<Tile>();
			for (int j = 0; j < mapSize; j++) {
				Tile tile = ((GameObject)Instantiate(PrefabHolder.instance.BASE_TILE_PREFAB, new Vector3(i - Mathf.Floor(mapSize/2),0, -j + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
				tile.transform.parent = mapTransform;
				tile.gridPosition = new Vector2(i, j);
				tile.setType((TileType)container.tiles.Where(x => x.locX == i && x.locY == j).First().id);
				row.Add (tile);
			}
			map.Add(row);
		}
	}

	void saveMapToXml() {
		MapSaveLoad.Save(MapSaveLoad.CreateMapContainer(map), "map.xml");
	}

	void OnGUI() {

		//pallet

		Rect rect = new Rect(10, Screen.height - 80, 50, 30);

		if (GUI.Button(rect, "N1")) {
			palletSelection = TileType.Normal1;
		}

		rect = new Rect(10 + (50 + 10) * 1, Screen.height - 80, 50, 30);
		
		if (GUI.Button(rect, "N2")) {
			palletSelection = TileType.Normal2;
		}

		rect = new Rect(10 + (50 + 10) * 2, Screen.height - 80, 50, 30);
		
		if (GUI.Button(rect, "N3")) {
			palletSelection = TileType.Normal3;
		}

		rect = new Rect(10 + (50 + 10) * 3, Screen.height - 80, 50, 30);
		
		if (GUI.Button(rect, "N4")) {
			palletSelection = TileType.Normal4;
		}
		rect = new Rect(10 + (50 + 10) * 4, Screen.height - 80, 50, 30);

		if (GUI.Button(rect, "Bridge")) {
			palletSelection = TileType.Bridge;
		}


	

		//-------------------以下第二排----------------//
		rect = new Rect(10, Screen.height - 160, 50, 30);

		if (GUI.Button(rect, "FakeNl")) {
			palletSelection = TileType.Impassible_FakeNomal;
		}
		rect = new Rect(10 + (50 + 10) * 1, Screen.height - 160, 50, 30);

		if (GUI.Button(rect, "Empty")) {
			palletSelection = TileType.Impassible_Empty;
		}

		rect = new Rect(10 + (50 + 10) * 2, Screen.height - 160, 50, 30);

		if (GUI.Button(rect, "Pilar")) {
			palletSelection = TileType.Impassible_pillar;
		}

		rect = new Rect(10 + (50 + 10) * 3, Screen.height - 160, 50, 30);

		if (GUI.Button(rect, "Mw_c")) {
			palletSelection = TileType.Impassible_matelWall_col;
		}
		rect = new Rect(10 + (50 + 10) * 4, Screen.height - 160, 50, 30);

		if (GUI.Button(rect, "Mw_r")) {
			palletSelection = TileType.Impassible_matelWall_row;
		}
		rect = new Rect(10 + (50 + 10) * 5, Screen.height - 160, 50, 30);
		if (GUI.Button(rect, "Rw_c")) {
			palletSelection = TileType.Impassible_RockWall_col;
		}
		rect = new Rect(10 + (50 + 10) * 6, Screen.height - 160, 50, 30);

		if (GUI.Button(rect, "Rw_r")) {
			palletSelection = TileType.Impassible_RockWall_row;
		}

		//------------以下第三排-------------//
		rect = new Rect(10, Screen.height - 240, 50, 30);

		if (GUI.Button(rect, "Lw_r")) {
			palletSelection = TileType.Impassible_RockWall_row_light;
		}
		rect = new Rect(10 + (50 + 10) * 1, Screen.height - 240, 50, 30);

		if (GUI.Button(rect, "Lw_c")) {
			palletSelection = TileType.Impassible_RockWall_col_light;
		}

		rect = new Rect(10 + (50 + 10) * 2, Screen.height - 240, 50, 30);

		if (GUI.Button(rect, "D_c")) {
			palletSelection = TileType.Impassible_Door_col;
		}

		rect = new Rect(10 + (50 + 10) * 3, Screen.height - 240, 50, 30);

		if (GUI.Button(rect, "D_r")) {
			palletSelection = TileType.Impassible_Door_row;
		}
	






		//

		//IO 
		rect = new Rect(Screen.width - (10 + (100 + 10) * 3), Screen.height - 80, 100, 60);
		
		if (GUI.Button(rect, "Clear Map")) {
			generateBlankMap(mapSize);
		}

		rect = new Rect(Screen.width - (10 + (100 + 10) * 2), Screen.height - 80, 100, 60);
		
		if (GUI.Button(rect, "Load Map")) {
			loadMapFromXml();
		}

		rect = new Rect(Screen.width - (10 + (100 + 10) * 1), Screen.height - 80, 100, 60);
		
		if (GUI.Button(rect, "Save Map")) {
			saveMapToXml();
		}
		//

	}
}
