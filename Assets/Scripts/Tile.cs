using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField, Range(0.0f, 100.0f)]
    private float rotateSpeed = 20.0f;
    [SerializeField, Range(0.0f, 50.0f)]
    private float vanishSpeed = 1.0f;
    [SerializeField, Range(0, 100)]
    private int score = 10;
    [SerializeField]
    private GameObject imgPlane;
    [SerializeField]
    private AudioClip clickSound;

    private AudioSource audioSource;
    private Quaternion nextposition;
    private float rotateAngle = 180.0f;
    private float closeAngle = -180.0f;
    private GameMode gameMode;
    private bool isMatched = false;
    private Vector3 worldPosition;
    public enum Direction : short
    {
        negative = -1,
        neutral,
        positive,
    }
    public Texture2D Texture
    {
        get
        {
            return imgPlane.GetComponent<MeshRenderer>()
                .material.GetTexture("_MainTex") as Texture2D;
        }

        set
        {
            imgPlane.GetComponent<MeshRenderer>()
                .material.SetTexture("_MainTex", value);
        }
    }

    private void Awake()
    {
        gameMode = FindObjectOfType<GameMode>();
        imgPlane = this.transform.Find("Image").gameObject;
        audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = clickSound;
    }
    void Update()
    {
        this.transform.rotation = Quaternion.Slerp(
            this.transform.rotation, nextposition, rotateSpeed * Time.deltaTime);
        if (isMatched)
        {
            this.transform.localScale = Vector3.Slerp(
                this.transform.localScale, new Vector3(0, 0, 0), vanishSpeed * Time.deltaTime);

            if (this.transform.localScale.y < .1f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public Tile SetTexture(Texture2D texture)
    {
        this.Texture = texture;

        return this;
    }

    public void Matched()
    {
        isMatched = true;
        gameMode.AddScore(score);
    }

    public void Open(Direction dir)
    {
        nextposition = Quaternion.Euler(0.0f, (short)dir * rotateAngle  , 0.0f);
    }

    public void Close()
    {
        nextposition = Quaternion.Euler(0.0f, this.transform.rotation.y + closeAngle * Time.deltaTime, 0.0f);
    }

    private void OnMouseDown()
    {
        Vector3 v3 = Input.mousePosition;
        v3.z = -Camera.main.transform.position.z; 
        //v3 = Camera.main.ViewportToWorldPoint(v3);
        worldPosition = Camera.main.ScreenToWorldPoint(v3);
        if (worldPosition.x > this.transform.position.x)
        {
            Open(Direction.positive);
        }
        else
        {
            Open(Direction.negative);
        }
        audioSource.Play();
        gameMode.CheckSelectedTile(this);
    }
}
