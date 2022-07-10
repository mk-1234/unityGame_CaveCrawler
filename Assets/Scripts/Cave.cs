using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent (typeof(MeshCollider))]
public class Cave : MonoBehaviour
{
    [SerializeField] GameObject[] walls;
    [SerializeField] GameObject caveEnter;
    [SerializeField] int sizeX;
    [SerializeField] int sizeY;
    [SerializeField] int repeat;
    private int[,,] field;
    private string[,] fieldMask;
    private Color wallColor;
    private Color floorColor;
    private Vector3[] vertices;
    private Mesh mesh;
    private NavMeshSurface surface;
    private int nmbOfGroundTiles;
    public GameObject playerPrefab;
    public GameObject monsterPrefab;
    private bool createdEntrance = false;

    void Awake() {

        sizeX = GameManager.instance.CaveSizeX;
        sizeY = GameManager.instance.CaveSizeY;
        repeat = GameManager.instance.Repeat;

        field = new int[sizeX, sizeY, 2];
        fieldMask = new string[sizeX, sizeY];

        wallColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        floorColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);

        print("colors:\nwall: " + wallColor + "\nground: " + floorColor);

        surface = FindObjectOfType<NavMeshSurface>();


        CreateGround();
        CreateWalls();
        CreateWallMask();
        
        RemoveWalls();
        CreateWallMask();

        PlaceWalls();
        //CreateBorder();

        CreateVertices();

        GetComponent<MeshCollider>().sharedMesh = mesh;

        surface.BuildNavMesh();

        Vector3 spawnPos = new Vector3(Mathf.RoundToInt(sizeX / 2) * 2f, 0f, Mathf.RoundToInt(sizeY / 2) * 2f);
        GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        PlaceMonsters(player.transform.position);

    }

    void CreateVertices() {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "GroundMesh";

        int tempCounter = 0;
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                if (field[i, j, 0] == 1 || field[i, j, 0] == 2) {
                    tempCounter++;
                }
            }
        }

        vertices = new Vector3[tempCounter * 4];
        Vector2[] storingTiles = new Vector2[tempCounter];

        int[,,] verticesInfo = new int[sizeX, sizeY, 4];

        int nmb = 0;
        for (int y = 0, i = 0; y < sizeY; y++) {
            for (int x = 0; x < sizeX; x++) {
                if (field[x, y, 0] == 1 || field[x, y, 0] == 2) {

                    string s = string.Empty;

                    if (field[x - 1, y - 1, 0] == 1 || field[x - 1, y - 1, 0] == 2)     s += "1"; else s += "0";
                    if (field[x, y - 1, 0] == 1     || field[x, y - 1, 0] == 2)         s += "1"; else s += "0";
                    if (field[x + 1, y - 1, 0] == 1 || field[x + 1, y - 1, 0] == 2)     s += "1"; else s += "0";
                    if (field[x - 1, y, 0] == 1     || field[x - 1, y, 0] == 2)         s += "1"; else s += "0";

                    if (s == "0000") {
                        vertices[i] = new Vector3(x * 2f - 1f, 0.05f, y * 2f - 1f);
                        verticesInfo[x, y, 0] = i;
                        vertices[i + 1] = new Vector3(x * 2f + 1f, 0.05f, y * 2f - 1f);
                        verticesInfo[x, y, 1] = i + 1;
                        verticesInfo[x + 1, y, 0] = i + 1;
                        vertices[i + 2] = new Vector3(x * 2f - 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 2] = i + 2;
                        verticesInfo[x - 1, y + 1, 1] = i + 2;
                        verticesInfo[x, y + 1, 0] = i + 2;
                        vertices[i + 3] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i + 3;
                        verticesInfo[x + 1, y, 2] = i + 3;
                        verticesInfo[x, y + 1, 1] = i + 3;
                        verticesInfo[x + 1, y + 1, 0] = i + 3;
                        i += 4;
                    }
                    if (s == "0001") {
                        vertices[i] = new Vector3(x * 2f + 1f, 0.05f, y * 2f - 1f);
                        verticesInfo[x, y, 1] = i;
                        verticesInfo[x + 1, y, 0] = i;
                        vertices[i + 1] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i + 1;
                        verticesInfo[x + 1, y, 2] = i + 1;
                        verticesInfo[x, y + 1, 1] = i + 1;
                        verticesInfo[x + 1, y + 1, 0] = i + 1;
                        i += 2;
                    }
                    if (s == "0010") {
                        vertices[i] = new Vector3(x * 2f - 1f, 0.05f, y * 2f - 1f);
                        verticesInfo[x, y, 0] = i;
                        vertices[i + 1] = new Vector3(x * 2f - 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 2] = i + 1;
                        verticesInfo[x - 1, y + 1, 1] = i + 1;
                        verticesInfo[x, y + 1, 0] = i + 1;
                        vertices[i + 2] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i + 2;
                        verticesInfo[x + 1, y, 2] = i + 2;
                        verticesInfo[x, y + 1, 1] = i + 2;
                        verticesInfo[x + 1, y + 1, 0] = i + 2;
                        i += 3;
                    }
                    if (s == "0011") {
                        vertices[i] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i;
                        verticesInfo[x + 1, y, 2] = i;
                        verticesInfo[x, y + 1, 1] = i;
                        verticesInfo[x + 1, y + 1, 0] = i;
                        i++;
                    }
                    if (s == "0100") {
                        vertices[i] = new Vector3(x * 2f - 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 2] = i;
                        verticesInfo[x - 1, y + 1, 1] = i;
                        verticesInfo[x, y + 1, 0] = i;
                        vertices[i + 1] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i + 1;
                        verticesInfo[x + 1, y, 2] = i + 1;
                        verticesInfo[x, y + 1, 1] = i + 1;
                        verticesInfo[x + 1, y + 1, 0] = i + 1;
                        i += 2;
                    }
                    if (s == "0101") {
                        vertices[i] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i;
                        verticesInfo[x + 1, y, 2] = i;
                        verticesInfo[x, y + 1, 1] = i;
                        verticesInfo[x + 1, y + 1, 0] = i;
                        i++;
                    }
                    if (s == "0110") {
                        vertices[i] = new Vector3(x * 2f - 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 2] = i;
                        verticesInfo[x - 1, y + 1, 1] = i;
                        verticesInfo[x, y + 1, 0] = i;
                        vertices[i + 1] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i + 1;
                        verticesInfo[x + 1, y, 2] = i + 1;
                        verticesInfo[x, y + 1, 1] = i + 1;
                        verticesInfo[x + 1, y + 1, 0] = i + 1;
                        i += 2;
                    }
                    if (s == "0111") {
                        vertices[i] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i;
                        verticesInfo[x + 1, y, 2] = i;
                        verticesInfo[x, y + 1, 1] = i;
                        verticesInfo[x + 1, y + 1, 0] = i;
                        i++;
                    }
                    if (s == "1000") {
                        vertices[i] = new Vector3(x * 2f + 1f, 0.05f, y * 2f - 1f);
                        verticesInfo[x, y, 1] = i;
                        verticesInfo[x + 1, y, 0] = i;
                        vertices[i + 1] = new Vector3(x * 2f - 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 2] = i + 1;
                        verticesInfo[x - 1, y + 1, 1] = i + 1;
                        verticesInfo[x, y + 1, 0] = i + 1;
                        vertices[i + 2] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i + 2;
                        verticesInfo[x + 1, y, 2] = i + 2;
                        verticesInfo[x, y + 1, 1] = i + 2;
                        verticesInfo[x + 1, y + 1, 0] = i + 2;
                        i += 3;
                    }
                    if (s == "1001") {
                        vertices[i] = new Vector3(x * 2f + 1f, 0.05f, y * 2f - 1f);
                        verticesInfo[x, y, 1] = i;
                        verticesInfo[x + 1, y, 0] = i;
                        vertices[i + 1] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i + 1;
                        verticesInfo[x + 1, y, 2] = i + 1;
                        verticesInfo[x, y + 1, 1] = i + 1;
                        verticesInfo[x + 1, y + 1, 0] = i + 1;
                        i += 2;
                    }
                    if (s == "1010") {
                        vertices[i] = new Vector3(x * 2f - 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 2] = i;
                        verticesInfo[x - 1, y + 1, 1] = i;
                        verticesInfo[x, y + 1, 0] = i;
                        vertices[i + 1] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i + 1;
                        verticesInfo[x + 1, y, 2] = i + 1;
                        verticesInfo[x, y + 1, 1] = i + 1;
                        verticesInfo[x + 1, y + 1, 0] = i + 1;
                        i += 2;
                    }
                    if (s == "1011") {
                        vertices[i] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i;
                        verticesInfo[x + 1, y, 2] = i;
                        verticesInfo[x, y + 1, 1] = i;
                        verticesInfo[x + 1, y + 1, 0] = i;
                        i++;
                    }
                    if (s == "1100") {
                        vertices[i] = new Vector3(x * 2f - 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 2] = i;
                        verticesInfo[x - 1, y + 1, 1] = i;
                        verticesInfo[x, y + 1, 0] = i;
                        vertices[i + 1] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i + 1;
                        verticesInfo[x + 1, y, 2] = i + 1;
                        verticesInfo[x, y + 1, 1] = i + 1;
                        verticesInfo[x + 1, y + 1, 0] = i + 1;
                        i += 2;
                    }
                    if (s == "1101") {
                        vertices[i] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i;
                        verticesInfo[x + 1, y, 2] = i;
                        verticesInfo[x, y + 1, 1] = i;
                        verticesInfo[x + 1, y + 1, 0] = i;
                        i++;
                    }
                    if (s == "1110") {
                        vertices[i] = new Vector3(x * 2f - 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 2] = i;
                        verticesInfo[x - 1, y + 1, 1] = i;
                        verticesInfo[x, y + 1, 0] = i;
                        vertices[i + 1] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i + 1;
                        verticesInfo[x + 1, y, 2] = i + 1;
                        verticesInfo[x, y + 1, 1] = i + 1;
                        verticesInfo[x + 1, y + 1, 0] = i + 1;
                        i += 2;
                    }
                    if (s == "1111") {
                        vertices[i] = new Vector3(x * 2f + 1f, 0.05f, y * 2f + 1f);
                        verticesInfo[x, y, 3] = i;
                        verticesInfo[x + 1, y, 2] = i;
                        verticesInfo[x, y + 1, 1] = i;
                        verticesInfo[x + 1, y + 1, 0] = i;
                        i++;
                    }
                }
            }
            nmb = i;
        }

        Vector3[] tempVerts = new Vector3[nmb];
        for (int i = 0; i < nmb; i++) {
            tempVerts[i] = vertices[i];
        }
        vertices = new Vector3[nmb];
        for (int i = 0; i < nmb; i++) {
            vertices[i] = tempVerts[i];
        }

        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        for (int i = 0; i < vertices.Length; i++) {
            uv[i] = new Vector2((float)vertices[i].x / (sizeX - 1), (float)vertices[i].z / (sizeY - 1));
            tangents[i] = tangent;
        }
        

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[tempCounter * 6];

        for (int j = 0, count = 0; j < sizeY; j++) {
            for (int i = 0; i < sizeX; i++) {
                if (field[i, j, 0] == 1 || field[i, j, 0] == 2) {
                    triangles[count] = verticesInfo[i, j, 0];
                    triangles[count + 1] = verticesInfo[i, j, 3];
                    triangles[count + 2] = verticesInfo[i, j, 1];
                    triangles[count + 3] = verticesInfo[i, j, 0];
                    triangles[count + 4] = verticesInfo[i, j, 2];
                    triangles[count + 5] = verticesInfo[i, j, 3];
                    count += 6;
                }
            }
        }
        
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshRenderer>().material.color = floorColor;
    }

    void PlaceWalls() {
        for (int j = 0; j < sizeY; j++) {
            for (int i = 0; i < sizeX; i++) {

                string s = fieldMask[i, j];

                // spojeno s jednim, dva ili tri polja na istoj strani

                /*
                      --- --- ---
                     |   |   |   |
                      --- --- ---
                     |   |   |   |
                      --- --- ---
                     | x | x | x |
                      --- --- ---
                */

                if (s == "01000000" || s == "11000000" || s == "01100000" || s == "11100000") {
                    CreateAndPositionWall(i, j, 0, 180f);
                }
                if (s == "00010000" || s == "10010000" || s == "00010100" || s == "10010100") {
                    CreateAndPositionWall(i, j, 0, 270f);
                }
                if (s == "00000010" || s == "00000110" || s == "00000011" || s == "00000111") {
                    CreateAndPositionWall(i, j, 0, 0f);
                }
                if (s == "00001000" || s == "00101000" || s == "00001001" || s == "00101001") {
                    CreateAndPositionWall(i, j, 0, 90f);
                }

                // -------------------------------------------------------------------------

                /*
                      --- --- ---
                     | x |   |   |
                      --- --- ---
                     |   |   |   |
                      --- --- ---
                     | x | x | x |
                      --- --- ---
                */

                if (s == "01000100" || s == "11000100" || s == "01100100" || s == "11100100") {
                    CreateAndPositionWall(i, j, 9, 180f);
                }
                if (s == "00010001" || s == "10010001" || s == "00010101" || s == "10010101") {
                    CreateAndPositionWall(i, j, 9, 270f);
                }
                if (s == "00100010" || s == "00100110" || s == "00100011" || s == "00100111") {
                    CreateAndPositionWall(i, j, 9, 0f);
                }
                if (s == "10001000" || s == "10101000" || s == "10001001" || s == "10101001") {
                    CreateAndPositionWall(i, j, 9, 90f);
                }

                /*
                      --- --- ---
                     |   |   | x |
                      --- --- ---
                     |   |   |   |
                      --- --- ---
                     | x | x | x |
                      --- --- ---
                */

                if (s == "01000001" || s == "11000001" || s == "01100001" || s == "11100001") {
                    CreateAndPositionWall(i, j, 10, 180f);
                }
                if (s == "00110000" || s == "10110000" || s == "00110100" || s == "10110100") {
                    CreateAndPositionWall(i, j, 10, 270f);
                }
                if (s == "10000010" || s == "10000110" || s == "10000011" || s == "10000111") {
                    CreateAndPositionWall(i, j, 10, 0f);
                }
                if (s == "00001100" || s == "00101100" || s == "00001101" || s == "00101101") {
                    CreateAndPositionWall(i, j, 10, 90f);
                }

                /*
                      --- --- ---
                     | x |   | x |
                      --- --- ---
                     |   |   |   |
                      --- --- ---
                     | x | x | x |
                      --- --- ---
                */

                if (s == "01000101" || s == "11000101" || s == "01100101" || s == "11100101") {
                    CreateAndPositionWall(i, j, 11, 180f);
                }
                if (s == "00110001" || s == "10110001" || s == "00110101" || s == "10110101") {
                    CreateAndPositionWall(i, j, 11, 270f);
                }
                if (s == "10100010" || s == "10100110" || s == "10100011" || s == "10100111") {
                    CreateAndPositionWall(i, j, 11, 0f);
                }
                if (s == "10001100" || s == "10101100" || s == "10001101" || s == "10101101") {
                    CreateAndPositionWall(i, j, 11, 90f);
                }

                // spojeno samo na jednom kutu

                /*
                      --- --- ---
                     |   |   |   |
                      --- --- ---
                     |   |   |   |
                      --- --- ---
                     | x |   |   |
                      --- --- ---
                */

                if (s == "10000000") {
                    CreateAndPositionWall(i, j, 2, 270f);
                }
                if (s == "00100000") {
                    CreateAndPositionWall(i, j, 2, 180f);
                }
                if (s == "00000100") {
                    CreateAndPositionWall(i, j, 2, 0f);
                }
                if (s == "00000001") {
                    CreateAndPositionWall(i, j, 2, 90f);
                }

                // spojeno na dva suprotna kuta

                /*
                      --- --- ---
                     |   |   | x |
                      --- --- ---
                     |   |   |   |
                      --- --- ---
                     | x |   |   |
                      --- --- ---
                */

                if (s == "10000001") {
                    CreateAndPositionWall(i, j, 3, 90f);
                }
                if (s == "00100100") {
                    CreateAndPositionWall(i, j, 3, 0f);
                }

                // spojeno na tri, četiri, ili pet polja na kutu

                /*
                      --- --- ---
                     |   |   |   |
                      --- --- ---
                     | x |   |   |
                      --- --- ---
                     | x | x |   |
                      --- --- ---
                */

                /*
                    000
                    x-0
                    xx0
                */

                if (s == "11010000" || s == "11110000" || s == "11010100" || s == "11110100" ||
                    s == "01010000" || s == "01110000" || s == "01010100" || s == "01110100") 
                {
                    CreateAndPositionWall(i, j, 7, 180f);
                }
                if (s == "01101000" || s == "11101000" || s == "01101001" || s == "11101001" ||
                    s == "01001000" || s == "11001000" || s == "01001001" || s == "11001001") 
                {
                    CreateAndPositionWall(i, j, 7, 90f);
                }
                if (s == "00010110" || s == "10010110" || s == "00010111" || s == "10010111" ||
                    s == "00010010" || s == "10010010" || s == "00010011" || s == "10010011") 
                {
                    CreateAndPositionWall(i, j, 7, 270f);
                }
                if (s == "00001011" || s == "00101011" || s == "00001111" || s == "00101111" ||
                    s == "00001010" || s == "00101010" || s == "00001110" || s == "00101110") 
                {
                    CreateAndPositionWall(i, j, 7, 0f);
                }

                // spojeno na tri, četiri, ili pet polja na kutu, i suprotnim kutem

                /*
                      --- --- ---
                     |   |   | x |
                      --- --- ---
                     | x |   |   |
                      --- --- ---
                     | x | x |   |
                      --- --- ---
                */

                if (s == "11010001" || s == "11110001" || s == "11010101" || s == "11110101" ||
                    s == "01010001" || s == "01110001" || s == "01010101" || s == "01110101") 
                {
                    CreateAndPositionWall(i, j, 8, 180f);
                }
                if (s == "01101100" || s == "11101100" || s == "01101101" || s == "11101101" ||
                    s == "01001100" || s == "11001100" || s == "01001101" || s == "11001101") 
                {
                    CreateAndPositionWall(i, j, 8, 90f);
                }
                if (s == "00110110" || s == "10110110" || s == "00110111" || s == "10110111" ||
                    s == "00110010" || s == "10110010" || s == "00110011" || s == "10110011") 
                {
                    CreateAndPositionWall(i, j, 8, 270f);
                }
                if (s == "10001011" || s == "10101011" || s == "10001111" || s == "10101111" ||
                    s == "10001010" || s == "10101010" || s == "10001110" || s == "10101110") 
                {
                    CreateAndPositionWall(i, j, 8, 0f);
                }

                // spojen na svim stranama

                /*
                      --- --- ---
                     | x | x | x |
                      --- --- ---
                     | x |   | x |
                      --- --- ---
                     | x | x | x |
                      --- --- ---
                */

                /*
                    xxx 0x0 0xx xx0   xxx 0x0 0xx xx0   0x0 0xx xx0 0x0
                    x-x x-x x-x x-x   x-x x-x x-x x-x   x-x x-x x-x x-x
                    xxx 0x0 0x0 0x0   0x0 0xx 0xx 0xx   xx0 xx0 xx0 xxx
                */

                if (s == "11111111" || s == "01011010" || s == "01011011" || s == "01011110" ||
                    s == "01011111" || s == "01111010" || s == "01111110" || s == "01111110" ||
                    s == "11011010" || s == "11011011" || s == "11011110" || s == "11111010") 
                {
                    float randAngle = Random.Range(0, 4) * 90f;
                    CreateAndPositionWall(i, j, 13, randAngle);
                }

                /*
                      --- --- ---
                     | x |   | x |
                      --- --- ---
                     |   |   |   |
                      --- --- ---
                     | x |   | x |
                      --- --- ---
                */

                if (s == "10100101") {
                    CreateAndPositionWall(i, j, 6, 0f);
                }

                // spojen s dva kuta na istoj strani, ali bez sredine

                //  --- --- ---
                // |   |   |   |
                //  --- --- ---
                // |   |   |   |
                //  --- --- ---
                // | x |   | x |
                //  --- --- ---

                // 000
                // 0-0
                // x0x

                if (s == "10100000") {
                    CreateAndPositionWall(i, j, 4, 180f);
                }

                // x00
                // 0-0
                // x00

                if (s == "10000100") {
                    CreateAndPositionWall(i, j, 4, 270f);
                }

                // x0x
                // 0-0
                // 000

                if (s == "00000101") {
                    CreateAndPositionWall(i, j, 4, 0f);
                }

                // 00x
                // 0-0
                // 00x

                if (s == "00100001") {
                    CreateAndPositionWall(i, j, 4, 90f);
                }

                /*
                      --- --- ---
                     | x |   |   |
                      --- --- ---
                     |   |   |   |
                      --- --- ---
                     | x |   | x |
                      --- --- ---
                */

                if (s == "10100100") {
                    CreateAndPositionWall(i, j, 5, 270f);
                }

                if (s == "10000101") {
                    CreateAndPositionWall(i, j, 5, 0f);
                }

                if (s == "00100101") {
                    CreateAndPositionWall(i, j, 5, 90f);
                }

                if (s == "10100001") {
                    CreateAndPositionWall(i, j, 5, 180f);
                }

                // -------------------------------------------------------------------------

                // spojen s pet, šest, ili sedam polja, zid na jednoj strani

                // 000 x00 00x x0x   000 x00 00x x0x   000 x00 00x x0x   000 x00 00x x0x
                // x-x x-x x-x x-x   x-x x-x x-x x-x   x-x x-x x-x x-x   x-x x-x x-x x-x
                // 0x0 0x0 0x0 0x0   xx0 xx0 xx0 xx0   0xx 0xx 0xx 0xx   xxx xxx xxx xxx

                if (s == "01011000" || s == "01011100" || s == "01011001" || s == "01011101" ||
                    s == "11011000" || s == "11011100" || s == "11011001" || s == "11011101" ||
                    s == "01111000" || s == "01111100" || s == "01111001" || s == "01111101" ||
                    s == "11111000" || s == "11111100" || s == "11111001" || s == "11111101") 
                {
                    CreateAndPositionWall(i, j, 12, 180f);
                }

                /*
                    0x0 0x0 0xx 0xx   xx0 xx0 xxx xxx   0x0 0x0 0xx 0xx   xx0 xx0 xxx xxx
                    x-0 x-0 x-0 x-0   x-0 x-0 x-0 x-0   x-0 x-0 x-0 x-0   x-0 x-0 x-0 x-0
                    0x0 0xx 0x0 0xx   0x0 0xx 0x0 0xx   xx0 xxx xx0 xxx   xx0 xxx xx0 xxx
                */

                if (s == "01010010" || s == "01110010" || s == "01010011" || s == "01110011" ||
                    s == "01010110" || s == "01110110" || s == "01010111" || s == "01110111" ||
                    s == "11010010" || s == "11110010" || s == "11010011" || s == "11110011" ||
                    s == "11010110" || s == "11110110" || s == "11010111" || s == "11110111") 
                {
                    CreateAndPositionWall(i, j, 12, 270f);
                }

                /*
                    0x0 0x0 0x0 0x0   xx0 xx0 xx0 xx0   0xx 0xx 0xx 0xx   xxx xxx xxx xxx
                    x-x x-x x-x x-x   x-x x-x x-x x-x   x-x x-x x-x x-x   x-x x-x x-x x-x
                    000 x00 00x x0x   000 x00 00x x0x   000 x00 00x x0x   000 x00 00x x0x
                */

                if (s == "00011010" || s == "10011010" || s == "00111010" || s == "10111010" ||
                    s == "00011110" || s == "10011110" || s == "00111110" || s == "10111110" ||
                    s == "00011011" || s == "10011011" || s == "00111011" || s == "10111011" ||
                    s == "00011111" || s == "10011111" || s == "00111111" || s == "10111111") 
                {
                    CreateAndPositionWall(i, j, 12, 0f);
                }

                /*
                    0x0 0x0 xx0 xx0   0xx 0xx xxx xxx   0x0 0x0 xx0 xx0   0xx 0xx xxx xxx
                    0-x 0-x 0-x 0-x   0-x 0-x 0-x 0-x   0-x 0-x 0-x 0-x   0-x 0-x 0-x 0-x
                    0x0 xx0 0x0 xx0   0x0 xx0 0x0 xx0   0xx xxx 0xx xxx   0xx xxx 0xx xxx
                */

                if (s == "01001010" || s == "11001010" || s == "01001110" || s == "11001110" ||
                    s == "01001011" || s == "11001011" || s == "01001111" || s == "11001111" ||
                    s == "01101010" || s == "11101010" || s == "01101110" || s == "11101110" ||
                    s == "01101011" || s == "11101011" || s == "01101111" || s == "11101111") 
                {
                    CreateAndPositionWall(i, j, 12, 90f);
                }

                 // ---------------------------------------------------------------------------

                /*
                    0x0 0x0 0x0 0x0   xx0 xx0 xx0 xx0   0xx 0xx 0xx 0xx   xxx xxx xxx xxx
                    0-0 0-0 0-0 0-0   0-0 0-0 0-0 0-0   0-0 0-0 0-0 0-0   0-0 0-0 0-0 0-0
                    0x0 xx0 0xx xxx   0x0 xx0 0xx xxx   0x0 xx0 0xx xxx   0x0 xx0 0xx xxx
                */

                if (s == "01000010" || s == "11000010" || s == "01100010" || s == "11100010" ||
                    s == "01000110" || s == "11000110" || s == "01100110" || s == "11100110" ||
                    s == "01000011" || s == "11000011" || s == "01100011" || s == "11100011" ||
                    s == "01000111" || s == "11000111" || s == "01100111" || s == "11100111") 
                {
                    CreateAndPositionWall(i, j, 1, 0f);
                }

                /*
                    000 x00 000 x00   00x x0x 00x x0x   000 x00 000 x00   00x x0x 00x x0x
                    x-x x-x x-x x-x   x-x x-x x-x x-x   x-x x-x x-x x-x   x-x x-x x-x x-x
                    000 000 x00 x00   000 000 x00 x00   00x 00x x0x x0x   00x 00x x0x x0x
                */

                if (s == "00011000" || s == "00011100" || s == "10011000" || s == "10011100" ||
                    s == "00011001" || s == "00011101" || s == "10011001" || s == "10011101" ||
                    s == "00111000" || s == "00111100" || s == "10111000" || s == "10111100" ||
                    s == "00111001" || s == "00111101" || s == "10111001" || s == "10111101") 
                {
                    CreateAndPositionWall(i, j, 1, 90f);
                }
            }
        }
    }

    void CreateAndPositionWall(int i, int j, int wallNmb, float angle) {
        Vector3 wallPos = new Vector3(i * 2f, 0f, j * 2f);
        if (!createdEntrance && wallNmb == 0 && (angle == 180f || angle == 270f)) {
            GameObject wall = Instantiate(caveEnter, wallPos, Quaternion.Euler(0f, angle, 0f));
            wall.GetComponent<MeshRenderer>().material.color = wallColor;
            wall.transform.SetParent(gameObject.transform);
            createdEntrance = true;
        } else {
            GameObject wall = Instantiate(walls[wallNmb], wallPos, Quaternion.Euler(0f, angle, 0f));
            wall.GetComponent<MeshRenderer>().material.color = wallColor;
            wall.transform.SetParent(gameObject.transform);
        }
    }

    void CreateGround() {
        int curX = Mathf.RoundToInt(sizeX / 2);
        int curY = Mathf.RoundToInt(sizeY / 2);
        field[curX, curY, 0] = 1;
        nmbOfGroundTiles = 1;
        
        for (int i = 0; i < repeat; i++) {
            int directionX = 0;
            int directionY = 0;

            if (Random.Range(0, 2) == 0) {
                directionX = Random.Range(0, 2);
                if (directionX == 0) directionX = -1;
            } else {
                directionY = Random.Range(0, 2);
                if (directionY == 0) directionY = -1;
            }
            
            curX += directionX;
            curY += directionY;

            if (curX < 2) curX = 2;
            if (curX >= sizeX - 2) curX = sizeX - 3;
            if (curY < 2) curY = 2;
            if (curY >= sizeY - 2) curY = sizeY - 3;

            if (field[curX, curY, 0] != 1) {
                field[curX, curY, 0] = 1;
                nmbOfGroundTiles++;
            }
        }
    }

    void CreateWalls() {
        for (int j = 0; j < sizeY; j++) {
            for (int i = 0; i < sizeX; i++) {
                if (field[i, j, 0] == 1) {

                    if (field[i - 1, j - 1, 0] == 0)    field[i - 1, j - 1, 0] = 2;
                    if (field[i, j - 1, 0] == 0)        field[i, j - 1, 0] = 2;
                    if (field[i + 1, j - 1, 0] == 0)    field[i + 1, j - 1, 0] = 2;
                    if (field[i - 1, j, 0] == 0)        field[i - 1, j, 0] = 2;
                    if (field[i + 1, j, 0] == 0)        field[i + 1, j, 0] = 2;
                    if (field[i - 1, j + 1, 0] == 0)    field[i - 1, j + 1, 0] = 2;
                    if (field[i, j + 1, 0] == 0)        field[i, j + 1, 0] = 2;
                    if (field[i + 1, j + 1, 0] == 0)    field[i + 1, j + 1, 0] = 2;
                }
            }
        }
    }

    void CreateWallMask() {
        for (int j = 0; j < sizeY; j++) {
            for (int i = 0; i < sizeX; i++) {
                if (field[i, j, 0] == 2) {

                    string mask = string.Empty;

                    if (field[i - 1, j - 1, 0] == 1)    mask += '1'; else mask += '0';
                    if (field[i, j - 1, 0] == 1)        mask += '1'; else mask += '0';
                    if (field[i + 1, j - 1, 0] == 1)    mask += '1'; else mask += '0';
                    if (field[i - 1, j, 0] == 1)        mask += '1'; else mask += '0';
                    if (field[i + 1, j, 0] == 1)        mask += '1'; else mask += '0';
                    if (field[i - 1, j + 1, 0] == 1)    mask += '1'; else mask += '0';
                    if (field[i, j + 1, 0] == 1)        mask += '1'; else mask += '0';
                    if (field[i + 1, j + 1, 0] == 1)    mask += '1'; else mask += '0';

                    fieldMask[i, j] = mask;
                }
            }
        }
    }

    void RemoveWalls() {
        for (int j = 0; j < sizeY; j++) {
            for (int i = 0; i < sizeX; i++) {
                string s = fieldMask[i, j];

                if (s == "11111111") {
                    int rng = Random.Range(0, 100);
                    if (rng < 95) {
                        field[i, j, 0] = 1;
                        fieldMask[i, j] = string.Empty;
                        nmbOfGroundTiles++;
                    }
                }
                if (s == "01111111" || s == "11011111" || s == "11111011" || s == "11111110") {
                    field[i, j, 0] = 1;
                    fieldMask[i, j] = string.Empty;
                    nmbOfGroundTiles++;
                }
            }
        }
    }

    void CreateBorder() {
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                if (j == 0 || j == sizeY - 1 || i == 0 || i == sizeX - 1) {
                    if (i == 0 && j == 0) {
                        Vector3 wallPos = new Vector3(i * 2f, 0f, j * 2f);
                        GameObject wall = Instantiate(walls[2], wallPos, Quaternion.Euler(0f, 90f, 0f)) as GameObject;
                        wall.GetComponent<MeshRenderer>().material.color = Color.green;
                        wall.transform.SetParent(this.gameObject.transform);
                    }
                    if (i == 0 && j == sizeY - 1) {
                        Vector3 wallPos = new Vector3(i * 2f, 0f, j * 2f);
                        GameObject wall = Instantiate(walls[2], wallPos, Quaternion.Euler(0f, 180f, 0f)) as GameObject;
                        wall.GetComponent<MeshRenderer>().material.color = Color.green;
                        wall.transform.SetParent(this.gameObject.transform);
                    }
                    if (i == sizeX - 1 && j == 0) {
                        Vector3 wallPos = new Vector3(i * 2f, 0f, j * 2f);
                        GameObject wall = Instantiate(walls[2], wallPos, Quaternion.identity) as GameObject;
                        wall.GetComponent<MeshRenderer>().material.color = Color.green;
                        wall.transform.SetParent(this.gameObject.transform);
                    }
                    if (i == sizeX - 1 && j == sizeY - 1) {
                        Vector3 wallPos = new Vector3(i * 2f, 0f, j * 2f);
                        GameObject wall = Instantiate(walls[2], wallPos, Quaternion.Euler(0f, 270f, 0f)) as GameObject;
                        wall.GetComponent<MeshRenderer>().material.color = Color.green;
                        wall.transform.SetParent(this.gameObject.transform);
                    }
                    if (i == 0 && (j != 0 && j != sizeY - 1)) {
                        Vector3 wallPos = new Vector3(i * 2f, 0f, j * 2f);
                        GameObject wall = Instantiate(walls[0], wallPos, Quaternion.Euler(0f, 90f, 0f)) as GameObject;
                        wall.GetComponent<MeshRenderer>().material.color = Color.green;
                        wall.transform.SetParent(this.gameObject.transform);
                    }
                    if (i == sizeX - 1 && (j != 0 && j != sizeY - 1)) {
                        Vector3 wallPos = new Vector3(i * 2f, 0f, j * 2f);
                        GameObject wall = Instantiate(walls[0], wallPos, Quaternion.Euler(0f, 270f, 0f)) as GameObject;
                        wall.GetComponent<MeshRenderer>().material.color = Color.green;
                        wall.transform.SetParent(this.gameObject.transform);
                    }
                    if (j == 0 && (i != 0 && i != sizeX - 1)) {
                        Vector3 wallPos = new Vector3(i * 2f, 0f, j * 2f);
                        GameObject wall = Instantiate(walls[0], wallPos, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
                        wall.GetComponent<MeshRenderer>().material.color = Color.green;
                        wall.transform.SetParent(this.gameObject.transform);
                    }
                    if (j == sizeY - 1 && (i != 0 && i != sizeX - 1)) {
                        Vector3 wallPos = new Vector3(i * 2f, 0f, j * 2f);
                        GameObject wall = Instantiate(walls[0], wallPos, Quaternion.Euler(0f, 180f, 0f)) as GameObject;
                        wall.GetComponent<MeshRenderer>().material.color = Color.green;
                        wall.transform.SetParent(this.gameObject.transform);
                    }
                }
            }
        }
    }

    void PlaceMonsters(Vector3 playerPosition) {
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                if (field[i, j, 0] == 1) {
                    int randomNmb = Random.Range(0, 100);
                    if (randomNmb <= Mathf.Min((5 + Mathf.FloorToInt(GameManager.instance.difficulty / 2)), 95)) {
                        Vector3 spawnPos = new Vector3(i * 2f, 0f, j * 2f);
                        float distance = Vector3.Distance(spawnPos, playerPosition);
                        if (distance > 3) {
                            Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
}
