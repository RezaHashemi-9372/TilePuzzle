using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMode : MonoBehaviour
{
    [SerializeField]
    private Tile tilePrefab;
    [SerializeField]
    private GameObject originPlace;
    [SerializeField, Range(0, 10)]
    private int row = 0;
    [SerializeField, Range(0, 10)]
    private int column = 0;
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject losePanel;
    [SerializeField]
    private GameObject winPanel;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text recordText;
    [SerializeField]
    private Text highScore;
    [SerializeField]
    private Text chosenClick;
    [SerializeField]
    private List<Texture2D> textures = new List<Texture2D>();

    private List<Tile> tiles = new List<Tile>();
    private int count = 0;
    private float gap;
    public static int Score { get; private set; }
    public static int LeftTile { get; private set; }
    public static int ClickedTime { get; private set; }
    public static int Record { get; private set; }
    private List<Tile> selectedTiles = new List<Tile>();
    private Tile helperTile = null;

    private void Awake()
    {
        GenerateGrid();
        Record = PlayerPrefs.GetInt("Record");
        count = 0;
        pausePanel.SetActive(false);
        losePanel.SetActive(false);
        winPanel.SetActive(false);
        highScore.enabled = false;
        Score = 0;
        
    }


    void Update()
    {
        scoreText.text = string.Format("Score: {0}" , Score);
        recordText.text = string.Format("Record: {0}" , Record);
        chosenClick.text = string.Format("{0}", ClickedTime);
    }

    public void SetupGrid(int row, int column)
    {
        Debug.Log("Setup Grid Called!");
        this.row = row;
        this.column = column;
        if (column % 2 == 1 && row % 2 == 1)
        {
            column -= 1;
        }
        else
        {
            count = (column * row) / 2;
        }
        this.count = column * row;
        GenerateGrid();
    }

    public void AddScore(int score)
    {
        Score += score;
        LeftTile -= 1;
        if (LeftTile <= 0)
        {
            winPanel.SetActive(true);
        }
        if (Score > Record)
        {
            Record = Score;
            highScore.enabled = true;
            PlayerPrefs.SetInt("Record", Record);
        }
    }


    public void Pause()
    {
        Time.timeScale = 0.0f;
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        pausePanel.SetActive(false);
    }
    
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("Scene", scene);
        SceneManager.LoadScene(0);
    }

    private void TileCount(int count)
    {
        //Debug.Log("Row is: " + row);
        //Debug.Log("Column is: " + column);
    }
    private void GenerateGrid()
    {
        BoxCollider boxCollider = tilePrefab.GetComponent<BoxCollider>();
        float width = boxCollider.size.x * tilePrefab.transform.localScale.x;
        float height = boxCollider.size.y * tilePrefab.transform.localScale.y;
        Vector3 pos = originPlace.transform.position;
        gap = 1.0f + (1.0f / (column * row));

        for (int i = 0; i < column; i++)
        {
            pos.x = i * width * gap;
            for (int j = 0; j < row; j++)
            {
                pos.y = j * height * gap;
                tiles.Add(Instantiate(tilePrefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity));
            }
        }
        count = row * column / 2;
        ClickedTime = SceneManager.GetActiveScene().buildIndex < 5 ?
            (row * column) + 25 : (row * column) + 35;
        LeftTile = row * column;
        List<Texture2D> selectedTextures = new List<Texture2D>();
        Debug.Log("Selected textures count is: " + selectedTextures.Count);
        Debug.Log("Count is: " + count);
        int index = -1;
        //Filling selected textures list with for loop and the list counts depends on Column and Row
        for (int i = 0; i < count; i++)
        {
            Debug.Log("Its second loop");
            index = Random.Range(0, textures.Count);
            if (!selectedTextures.Contains(textures[index]))
            {
                selectedTextures.Add(textures[index]);
                textures.RemoveAt(index);
            }
        }

        List<int> counter = new List<int>();
        //counter.Clear();

        //loop for filling a list we have created counter list
        for (int i = 0; i < column * row; i++) 
        {
            counter.Add(i);
        }

        ///for just test Random list
        List<int> temp = new List<int>();
        for (int i = 0; i < tiles.Count; i++)
        {
            temp.Add(i);
        }

        //putting texture from selectedTexture List to each Tiles 
        for (int i = 0; i < selectedTextures.Count; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int n = counter[Random.Range(0, temp.Count)];
                Debug.Log("Random N is: " + n);
                tiles[n].GetComponent<Tile>().SetTexture(selectedTextures[i]);
                tiles.RemoveAt(n);
                temp.RemoveAt(n);
            }
        }
    }

    public void ClearGrid()
    {
        if (tiles != null)
        {
            for (int i = 0; i < tiles.Count; i++)
            {

                GameObject obj = tiles[i].gameObject;
                tiles.RemoveAt(i);
                Destroy(obj);
            }
        }
    }

    public void CheckSelectedTile(Tile tile)
    {
        ClickedTime -= 1;
        if (ClickedTime <= 0)
        {
            losePanel.SetActive(true);
        }
        selectedTiles.Add(tile);
        if (selectedTiles.Count >= 2)
        {
            if (selectedTiles[0].Texture == selectedTiles[1].Texture && helperTile == null)
            {
                selectedTiles[0].Matched();
                selectedTiles[1].Matched();
                selectedTiles.Clear();
            }
            else if (selectedTiles[0].Texture != selectedTiles[1].Texture && selectedTiles.Count == 3 && helperTile == null)
            {
                selectedTiles[0].Close();
                selectedTiles[1].Close();
                helperTile = selectedTiles[2];
                selectedTiles.Clear();
                if (selectedTiles.Count == 0 && helperTile != null)
                {
                    selectedTiles.Add(helperTile);
                }
            }
                helperTile = null;
            
            //helperTile = null;
            //Debug.Log("Selected tiles: " + selectedTiles.Count);
             //   selectedTiles.Clear();
        }
    }
}