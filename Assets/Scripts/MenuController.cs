using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MenuController : MonoBehaviour
{
    public GameObject[] gates;
    public MenuCharacter menuCharacter;
    private float spawner;
    public GameObject quitButton;

    private void Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            spawnDude();
        }

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            quitButton.SetActive(false);
        }
    }

    private void Update()
    {
        spawner += Time.deltaTime;

        if (spawner >= .5f)
        {
            spawner = 0;
            spawnDude();
        }
    }

    private void spawnDude()
    {
        GameObject gate1 = gates[Random.Range(0, gates.Length)];
        GameObject gate2 = gates[Random.Range(0, gates.Length)];
        if (gate2 == gate1)
        {
            spawnDude();
            return;
        }

        MenuCharacter guy = Instantiate(menuCharacter, gate1.transform.position, Quaternion.identity);
        guy.targetPosition = gate2.transform.position;
    }

    public void startGame()
    {
        SceneManager.LoadScene("PrototypeNick - SpriteShapeTest 1");
    }

    public void options()
    {
    }

    public void quit()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            Application.Quit();
        }
    }
}