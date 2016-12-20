using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TilePathFinder : MonoBehaviour {
	public static List<Tile> FindPath(Tile originTile, Tile destinationTile) {
		return FindPath(originTile, destinationTile, new Vector2[0]);
	}
	public static List<Tile> FindPath(Tile originTile, Tile destinationTile, Vector2[] occupied) //occupied有人已經使用這個tilePosition
	{
		List<Tile> closed = new List<Tile>();
		List<TilePath> open = new List<TilePath>();
		
		TilePath originPath = new TilePath();
		originPath.addTile(originTile);//把起始點加到路徑中
		
		open.Add(originPath);
		TilePath current = new TilePath() ;
		
		while (open.Count > 0)
		{
			current = open[0];//openList的第1個路徑指定給current
			open.Remove(open[0]);
			
			if (closed.Contains(current.lastTile))
			{
				continue;
			} 
			if (current.lastTile == destinationTile)
			{
				current.listOfTiles.Distinct();
				current.listOfTiles.Remove(originTile);
				return current.listOfTiles;//找到了
			}
			
			closed.Add(current.lastTile);
			
			foreach (Tile t in current.lastTile.neighbors) //對每個路徑上的tile的鄰居執行
			{
				if (t.impassible || occupied.Contains(t.gridPosition)) continue;//如果是不可走的或是已經被占據就執行迴圈的下個tile
				TilePath newTilePath = new TilePath(current);
				newTilePath.addTile(t);
				open.Add(newTilePath);
			}
			
			
		}
		return closed ;
	}
}
