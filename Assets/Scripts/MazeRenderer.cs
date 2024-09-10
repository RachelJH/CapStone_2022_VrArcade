using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField]
    [Range (1,50)]
    private int width = 5;
    //미로의 가로 넓이 세팅
    [SerializeField]
    [Range(1, 50)]
    private int height = 5;
    //미로의 세로넓이 세팅

    [SerializeField]
    private float size = 1f;

    [SerializeField]
    private Transform wallPrefab = null;
    // Start is called before the first frame update
    [SerializeField]
    private Transform floorPrefab = null;
    void Start()
    {
        var maze = MazeGenerator.Generate(width, height);
        Draw(maze);
    }

    private void Draw(WallState[,] maze) // width x height 넓이의 칸막이 생성
    {
        var floor = Instantiate(floorPrefab,transform);
        floor.localScale = new Vector3(width,0,height);
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, 0, -height / 2 + j);

                if (cell.HasFlag(WallState.UP)) 
                {
                    var topwall = Instantiate(wallPrefab, transform) as Transform;
                    topwall.position = position + new Vector3(0,0,size/2);
                    topwall.localScale = new Vector3(size,topwall.localScale.y,topwall.localScale.z);
                }
                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform) as Transform;
                    leftWall.position = position + new Vector3(-size/2, 0, 0);
                    leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0,90,0);
                }
                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform) as Transform;
                        rightWall.position = position + new Vector3(size / 2, 0, 0);
                        rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }
                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, transform) as Transform;
                        bottomWall.position = position + new Vector3(0,0,-size/2);
                        bottomWall.localScale = new Vector3(size, bottomWall.localScale.y, bottomWall.localScale.z);
                        
                    }
                }
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
