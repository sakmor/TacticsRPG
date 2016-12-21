using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TilePath //尋路的路徑
{
	public List<Tile> listOfTiles = new List<Tile>();//路徑上全部的tile

	public int costOfPath = 0;	//這條路徑的花費
	
	public Tile lastTile;//路徑上最後一個tile
	
	public TilePath() {}
	
	public TilePath(TilePath tp)//丟進一個TilePath然後存取這條路徑的資料
	{
		listOfTiles = tp.listOfTiles.ToList();
		costOfPath = tp.costOfPath;
		lastTile = tp.lastTile;
	}
	
	public void addTile(Tile t) //加一個tile到這個路徑，並加上他的花費，指定他為最後一個tile
	{
		costOfPath += t.movementCost;
		listOfTiles.Add(t);
		lastTile = t;
	}

	public void addStaticTile(Tile t) 
	{
		costOfPath += 1;
		listOfTiles.Add(t);
		lastTile = t;
	}
}