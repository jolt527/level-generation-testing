using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGeneration : MonoBehaviour {

    public Vector3 worldLoadPoint;
    public GameObject[] levelPrefabs;

    private const float PIECE_LENGTH = 10f;
    private const string NEW_SCENE_NAME = "new scene";
    private const string RELATIVE_LEVEL_FILE_PATH = "/test.level";

    void Start() {
        int[,] wallData = {
            { 1, 1, 1, 1, 0, 0, 1, 1, 1 },
            { 0, 0, 0, 1, 0, 0, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 1, 1, 1 },
            { 0, 0, 0, 1, 1, 0, 1, 0, 1 },
            { 1, 1, 1, 1, 0, 0, 0, 0, 0 },
            { 1, 0, 1, 1, 1, 0, 1, 1, 1 }
        };
        LevelData levelData = createLevelDataFromWallData(wallData, 2, 1);

        string levelFilePath = Application.persistentDataPath + RELATIVE_LEVEL_FILE_PATH;
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        Debug.Log("outputFilePath = " + levelFilePath);
        FileStream outputFileStream = new FileStream(levelFilePath, FileMode.Create);
        binaryFormatter.Serialize(outputFileStream, levelData);
        outputFileStream.Close();

        createSceneFromFile();
    }

    void Update() {
        if (!Input.GetKeyDown(KeyCode.Space)) {
            return;
        }

        if (SceneManager.GetSceneByName(NEW_SCENE_NAME).isLoaded) {
            SceneManager.UnloadSceneAsync(NEW_SCENE_NAME);
        } else {
            createSceneFromFile();
        }
    }

    private LevelData createLevelDataFromWallData(int[,] wallData, int anchorRow, int anchorColumn) {
        int rows = wallData.GetLength(0);
        int columns = wallData.GetLength(1);
        int[,] data = new int[rows,columns];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                if (wallData[i,j] == 1) {
                    data[i,j] = -1;
                    continue;
                }

                bool hasLeftWall = j == 0 || wallData[i, j - 1] == 1;
                bool hasRightWall = j == columns - 1 || wallData[i, j + 1] == 1;
                bool hasUpWall = i == 0 || wallData[i - 1, j] == 1;
                bool hasDownWall = i == rows - 1 || wallData[i + 1, j] == 1;

                // shitty prefab index determination
                int prefabIndex = 0;
                if (!hasLeftWall && !hasRightWall && !hasUpWall && hasDownWall) {
                    prefabIndex = 1;
                } else if (hasLeftWall && !hasRightWall && !hasUpWall && !hasDownWall) {
                    prefabIndex = 2;
                } else if (!hasLeftWall && !hasRightWall && hasUpWall && !hasDownWall) {
                    prefabIndex = 3;
                } else if (!hasLeftWall && hasRightWall && !hasUpWall && !hasDownWall) {
                    prefabIndex = 4;
                } else if (hasLeftWall && !hasRightWall && !hasUpWall && hasDownWall) {
                    prefabIndex = 5;
                } else if (hasLeftWall && !hasRightWall && hasUpWall && !hasDownWall) {
                    prefabIndex = 6;
                } else if (!hasLeftWall && hasRightWall && hasUpWall && !hasDownWall) {
                    prefabIndex = 7;
                } else if (!hasLeftWall && hasRightWall && !hasUpWall && hasDownWall) {
                    prefabIndex = 8;
                } else if (!hasLeftWall && !hasRightWall && hasUpWall && hasDownWall) {
                    prefabIndex = 9;
                } else if (hasLeftWall && hasRightWall && !hasUpWall && !hasDownWall) {
                    prefabIndex = 10;
                } else if (hasLeftWall && hasRightWall && hasUpWall && !hasDownWall) {
                    prefabIndex = 11;
                } else if (!hasLeftWall && hasRightWall && hasUpWall && hasDownWall) {
                    prefabIndex = 12;
                } else if (hasLeftWall && hasRightWall && !hasUpWall && hasDownWall) {
                    prefabIndex = 13;
                } else if (hasLeftWall && !hasRightWall && hasUpWall && hasDownWall) {
                    prefabIndex = 14;
                } else if (hasLeftWall && hasRightWall && hasUpWall && hasDownWall) {
                    prefabIndex = 15;
                }

                data[i,j] = prefabIndex;
            }
        }

        LevelData levelData = new LevelData();
        levelData.data = data;
        levelData.anchorRow = anchorRow;
        levelData.anchorColumn = anchorColumn;

        return levelData;
    }

    private void createSceneFromFile() {
        float startTime = Time.time;

        string levelFilePath = Application.persistentDataPath + RELATIVE_LEVEL_FILE_PATH;
        LevelData loadedLevelData;
        if (File.Exists(levelFilePath)) {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream inputFileStream = new FileStream(levelFilePath, FileMode.Open);
            loadedLevelData = binaryFormatter.Deserialize(inputFileStream) as LevelData;
            inputFileStream.Close();
        } else {
            Debug.LogError("File does not exist at: " + levelFilePath);
            return;
        }

        int[,] data = loadedLevelData.data;
        int rows = data.GetLength(0);
        int columns = data.GetLength(1);

        Scene newScene = SceneManager.CreateScene(NEW_SCENE_NAME);
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                int levelPrefabIndex = data[i,j];
                if (levelPrefabIndex < 0) {
                    continue;
                }

                float offsetColumn = loadedLevelData.anchorColumn;
                float offsetRow = loadedLevelData.anchorRow;
                Vector3 pieceWorldPosition = new Vector3(
                    (j - offsetColumn) * PIECE_LENGTH + worldLoadPoint.x,
                    0f + worldLoadPoint.y,
                    -(i - offsetRow) * PIECE_LENGTH + worldLoadPoint.z
                );
                GameObject piece = Instantiate(levelPrefabs[levelPrefabIndex], pieceWorldPosition, Quaternion.identity);
                SceneManager.MoveGameObjectToScene(piece, newScene);
            }
        }

        Debug.Log("Total time to load scene in seconds: " + (Time.time - startTime));
    }
}
