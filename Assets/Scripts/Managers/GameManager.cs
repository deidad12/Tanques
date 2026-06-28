using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public CameraControl m_CameraControl;

    public UnityEngine.UI.Text m_MessageText;

    public GameObject m_TankPrefab;
    public TankManager[] m_Tanks;

    public int maxLosses = 5;

    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;

    private int player1Losses = 0;
    private int player2Losses = 0;

    private float gameStartTime;
    private bool gameEnded = false;

    private void Start()
    {
        gameStartTime = Time.time;

        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (!gameEnded)
        {
            yield return StartCoroutine(RoundStarting());
            yield return StartCoroutine(RoundPlaying());
            yield return StartCoroutine(RoundEnding());
        }
    }

    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance = Instantiate(
                m_TankPrefab,
                m_Tanks[i].m_SpawnPoint.position,
                m_Tanks[i].m_SpawnPoint.rotation
            );

            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
            targets[i] = m_Tanks[i].m_Instance.transform;

        m_CameraControl.m_Targets = targets;
    }

    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }

    // ✅ MODIFICADO — AHORA VERIFICA TIEMPO
    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        m_MessageText.text = "";

        while (!OneTankLeft())
        {
            // ✅ SI SE ACABA EL TIEMPO → DERROTA DOBLE
            if (MatchTimer.Instance.IsTimeOver())
            {
                EndGameByTime();
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner == m_Tanks[0])
            player2Losses++;
        else if (m_RoundWinner == m_Tanks[1])
            player1Losses++;

        if (player1Losses >= maxLosses)
        {
            EndGame("JUGADOR 2 GANA EL JUEGO!");
            yield break;
        }

        if (player2Losses >= maxLosses)
        {
            EndGame("JUGADOR 1 GANA EL JUEGO!");
            yield break;
        }

        m_MessageText.text = EndMessage();

        yield return m_EndWait;
    }

    // ✅ NUEVO — DERROTA POR TIEMPO
    private void EndGameByTime()
    {
        gameEnded = true;

        m_MessageText.text =
            "TIEMPO AGOTADO!\nAMBOS JUGADORES PIERDEN";

        StartCoroutine(RestartGame());
    }

    private void EndGame(string winnerText)
    {
        gameEnded = true;

        float totalTime = Time.time - gameStartTime;

        m_MessageText.text =
            winnerText +
            "\n\nTIEMPO TOTAL: " + totalTime.ToString("F1") + " segundos" +
            "\nJugador 1 Derrotas: " + player1Losses +
            "\nJugador 2 Derrotas: " + player2Losses;

        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(0);
    }

    private bool OneTankLeft()
    {
        int active = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
            if (m_Tanks[i].m_Instance.activeSelf)
                active++;

        return active <= 1;
    }

    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];

        return null;
    }

    private string EndMessage()
    {
        string msg = "EMPATE!";

        if (m_RoundWinner != null)
            msg = m_RoundWinner.m_ColoredPlayerText + " GANA LA RONDA!";

        msg += "\n\nJugador 1 Derrotas: " + player1Losses;
        msg += "\nJugador 2 Derrotas: " + player2Losses;

        return msg;
    }

    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
            m_Tanks[i].Reset();
    }

    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
            m_Tanks[i].EnableControl();
    }

    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
            m_Tanks[i].DisableControl();
    }
}