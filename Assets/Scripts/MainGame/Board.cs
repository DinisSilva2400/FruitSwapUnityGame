using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Board : MonoBehaviour
{
    [Header("Board Settings")]
    public int width = 4;
    public int height = 8;
    public GameObject[] fruits;
    public float swapSpeed = 0.2f;

    [Header("Effects")]
    public GameObject explosionPrefab; // arrasta aqui o prefab da explosão

    private GameObject[,] allFruits;

    [Header("Audio slide")]
    public AudioSource audioSource;
    public AudioClip selectSound;
    public AudioClip swapSound;
    public AudioClip matchSound;

    [Header("Audio Music")]
    public AudioSource musicSource;
    public AudioClip backgroundMusic;

    [Header("UI")]
    public GameObject timeBonusTMP; // assign your disabled TMP "+10" GameObject here
    public float bonusDisplayDuration = 1f;
    public float bonusMoveDistance = 30f;
    private Coroutine bonusCoroutine;

    public GameObject pointsTMP; // assign your disabled TMP for points display
    public float pointsDisplayDuration = 1f;
    public float pointsMoveDistance = 30f;
    private Coroutine pointsCoroutine;




    // bloqueia input enquanto processa trocas/clears
    private bool isProcessing = false;

    private Fruit firstSelected = null;
    private Fruit secondSelected = null;

    void Start()
    {
        Time.timeScale = 1f;

        allFruits = new GameObject[width, height];
        GenerateBoard();

        // 🎵 música de fundo
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // -----------------------------------------------------
    // GERAR TABULEIRO
    // -----------------------------------------------------
    void GenerateBoard()
    {
        Vector2 offset = new Vector2(width / 2f - 0.5f, height / 2f - 0.5f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = Random.Range(0, fruits.Length);

                // evita criar matches imediatos ao gerar
                while ((x >= 2 &&
                        allFruits[x - 1, y] != null &&
                        allFruits[x - 2, y] != null &&
                        allFruits[x - 1, y].GetComponent<Fruit>().type == index &&
                        allFruits[x - 2, y].GetComponent<Fruit>().type == index)
                       ||
                       (y >= 2 &&
                        allFruits[x, y - 1] != null &&
                        allFruits[x, y - 2] != null &&
                        allFruits[x, y - 1].GetComponent<Fruit>().type == index &&
                        allFruits[x, y - 2].GetComponent<Fruit>().type == index))
                {
                    index = Random.Range(0, fruits.Length);
                }

                Vector3 pos = new Vector3(x - offset.x, y - offset.y, 0);

                GameObject fruit = Instantiate(fruits[index], pos, Quaternion.identity);
                fruit.transform.parent = this.transform;

                Fruit fs = fruit.GetComponent<Fruit>();
                fs.x = x;
                fs.y = y;
                fs.type = index; // define o tipo da fruta
                fs.board = this;

                allFruits[x, y] = fruit;
            }
        }
    }

    // -----------------------------------------------------
    // SELEÇÃO / TROCA
    // -----------------------------------------------------
    public void SelectFruit(Fruit fruit)
    {
        
        if (isProcessing) return;

        if (firstSelected == null)
        {
            firstSelected = fruit;

            // 🔊 som de seleção imediata
            if (audioSource != null && selectSound != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(selectSound);
            }

            return;
        }


        if (secondSelected == null)
        {
            int dx = Mathf.Abs(fruit.x - firstSelected.x);
            int dy = Mathf.Abs(fruit.y - firstSelected.y);

            // permitir apenas vizinho ortogonal (sem diagonal)
            if ((dx == 1 && dy == 0) || (dx == 0 && dy == 1))
            {
                secondSelected = fruit;
                StartCoroutine(SwapAndCheck(firstSelected, secondSelected));
                firstSelected = null;
                secondSelected = null;
            }
            else
            {
                // não é vizinho ortogonal → torna esta a nova seleção
                firstSelected = fruit;
            }
        }
    }


    IEnumerator SwapAndCheck(Fruit a, Fruit b)
    {
        
        // bloquear novas ações enquanto processa
        isProcessing = true;

        // realiza a troca
        yield return StartCoroutine(SwapFruits(a, b));

        // se houver match processa limpeza e cascata; se não, mantém a troca (não desfaz)
        if (CheckMatches())
        {
            yield return StartCoroutine(ClearMatches());
            yield return StartCoroutine(CollapseBoard());
        }
        else
        {
            // sem match → mantém a troca (nenhuma ação adicional)
            yield return null;
        }

        // esperar 0.5s antes de permitir nova interação
        yield return new WaitForSeconds(0.1f);
        isProcessing = false;
    }


    IEnumerator SwapFruits(Fruit a, Fruit b)
    {
        Vector3 posA = a.transform.position;
        Vector3 posB = b.transform.position;

        float t = 0;
        while (t < swapSpeed)
        {
            a.transform.position = Vector3.Lerp(posA, posB, t / swapSpeed);
            b.transform.position = Vector3.Lerp(posB, posA, t / swapSpeed);
            t += Time.deltaTime;
            yield return null;
        }

        a.transform.position = posB;
        b.transform.position = posA;

        // 🔊 som da troca
        if (audioSource != null && swapSound != null)
        {
            audioSource.PlayOneShot(swapSound);
        }


        // atualizar matriz
        allFruits[a.x, a.y] = b.gameObject;
        allFruits[b.x, b.y] = a.gameObject;

        int tx = a.x;
        int ty = a.y;

        a.x = b.x;
        a.y = b.y;

        b.x = tx;
        b.y = ty;
    }

    // -----------------------------------------------------
    // MATCHES
    // -----------------------------------------------------
    bool CheckMatches()
    {
        bool matchFound = false;

        // horizontal
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                if (allFruits[x, y] != null &&
                    allFruits[x + 1, y] != null &&
                    allFruits[x + 2, y] != null)
                {
                    int t0 = allFruits[x, y].GetComponent<Fruit>().type;
                    int t1 = allFruits[x + 1, y].GetComponent<Fruit>().type;
                    int t2 = allFruits[x + 2, y].GetComponent<Fruit>().type;

                    if (t0 == t1 && t0 == t2)
                        matchFound = true;
                }
            }
        }

        // vertical
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                if (allFruits[x, y] != null &&
                    allFruits[x, y + 1] != null &&
                    allFruits[x, y + 2] != null)
                {
                    int t0 = allFruits[x, y].GetComponent<Fruit>().type;
                    int t1 = allFruits[x, y + 1].GetComponent<Fruit>().type;
                    int t2 = allFruits[x, y + 2].GetComponent<Fruit>().type;

                    if (t0 == t1 && t0 == t2)
                        matchFound = true;
                }
            }
        }

        return matchFound;
    }

    IEnumerator ClearMatches()
    {
        HashSet<GameObject> matched = new HashSet<GameObject>();

        // horizontal
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                if (allFruits[x, y] != null &&
                    allFruits[x + 1, y] != null &&
                    allFruits[x + 2, y] != null)
                {
                    int t0 = allFruits[x, y].GetComponent<Fruit>().type;
                    int t1 = allFruits[x + 1, y].GetComponent<Fruit>().type;
                    int t2 = allFruits[x + 2, y].GetComponent<Fruit>().type;

                    if (t0 == t1 && t0 == t2)
                    {
                        matched.Add(allFruits[x, y]);
                        matched.Add(allFruits[x + 1, y]);
                        matched.Add(allFruits[x + 2, y]);
                    }
                }
            }
        }

        // vertical
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                if (allFruits[x, y] != null &&
                    allFruits[x, y + 1] != null &&
                    allFruits[x, y + 2] != null)
                {
                    int t0 = allFruits[x, y].GetComponent<Fruit>().type;
                    int t1 = allFruits[x, y + 1].GetComponent<Fruit>().type;
                    int t2 = allFruits[x, y + 2].GetComponent<Fruit>().type;

                    if (t0 == t1 && t0 == t2)
                    {
                        matched.Add(allFruits[x, y]);
                        matched.Add(allFruits[x, y + 1]);
                        matched.Add(allFruits[x, y + 2]);
                    }
                }
            }
        }

        // Verificar se há linhas de 4+ frutas e adicionar tempo
        CheckAndAwardTimeBonus(matched);

        int pointsPerFruit = 10;
        int totalMatched = matched.Count;

        // 🔊 som da combinação (igual ao select)
        if (totalMatched > 0 && audioSource != null && matchSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(matchSound);
        }

        int earnedPoints = totalMatched * pointsPerFruit;
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(earnedPoints);
        }

        // Mostrar pontos da combinação
        if (totalMatched > 0 && pointsTMP != null)
        {
            if (pointsCoroutine != null) StopCoroutine(pointsCoroutine);
            pointsCoroutine = StartCoroutine(ShowPointsBonus(earnedPoints));
        }

        
        // destruir
        foreach (GameObject f in matched)
        {
            if (f == null) continue;

            Fruit fs = f.GetComponent<Fruit>();
            allFruits[fs.x, fs.y] = null;

            // ---- explosão colocada aqui ----
            if (explosionPrefab != null)
            {
                GameObject exp = Instantiate(explosionPrefab, f.transform.position, Quaternion.identity);
                Destroy(exp,  1.5f); // limpa as partículas após 1.5s
            }
            // --------------------------------

            Destroy(f);
        }



        yield return null;
    }

    // -----------------------------------------------------
    // CASCATA
    // -----------------------------------------------------
    IEnumerator CollapseBoard()
    {
        // puxar frutas para baixo
        for (int x = 0; x < width; x++)
        {
            int nextY = 0;

            for (int y = 0; y < height; y++)
            {
                if (allFruits[x, y] != null)
                {
                    if (y != nextY)
                    {
                        GameObject fruit = allFruits[x, y];
                        allFruits[x, y] = null;
                        allFruits[x, nextY] = fruit;

                        Fruit fs = fruit.GetComponent<Fruit>();
                        fs.x = x;
                        fs.y = nextY;

                        StartCoroutine(MoveFruitTo(fruit, x, nextY));
                    }

                    nextY++;
                }
            }
        }

        yield return new WaitForSeconds(0.2f);

        // preencher espaços vazios
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allFruits[x, y] == null)
                {
                    int index = Random.Range(0, fruits.Length);

                    Vector3 spawnPos = new Vector3(
                        x - width / 2f + 0.5f,
                        height + 2,
                        0
                    );

                    GameObject fruit = Instantiate(fruits[index], spawnPos, Quaternion.identity);
                    fruit.transform.parent = this.transform;

                    Fruit fs = fruit.GetComponent<Fruit>();
                    fs.x = x;
                    fs.y = y;
                    fs.type = index; // define tipo ao nascer
                    fs.board = this;

                    allFruits[x, y] = fruit;

                    StartCoroutine(MoveFruitTo(fruit, x, y));
                }
            }
        }

        yield return new WaitForSeconds(0.25f);

        // continuar combos
        if (CheckMatches())
        {
            yield return StartCoroutine(ClearMatches());
            yield return StartCoroutine(CollapseBoard());
        }
    }

    IEnumerator MoveFruitTo(GameObject fruit, int x, int y)
    {
        Vector3 start = fruit.transform.position;
        Vector3 end = new Vector3(
            x - width / 2f + 0.5f,
            y - height / 2f + 0.5f,
            0
        );

        float t = 0;
        float speed = 0.15f;

        while (t < 1)
        {
            t += Time.deltaTime / speed;
            fruit.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        fruit.transform.position = end;
    }

    void CheckAndAwardTimeBonus(HashSet<GameObject> matched)
    {
        // Verificar linhas horizontais de 4+ frutas
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (allFruits[x, y] != null && matched.Contains(allFruits[x, y]))
                {
                    int fruitType = allFruits[x, y].GetComponent<Fruit>().type;
                    int consecutive = 1;

                    // Contar para a direita
                    for (int nextX = x + 1; nextX < width; nextX++)
                    {
                        if (allFruits[nextX, y] != null && 
                            allFruits[nextX, y].GetComponent<Fruit>().type == fruitType &&
                            matched.Contains(allFruits[nextX, y]))
                        {
                            consecutive++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Se tem 4 ou mais em linha, adiciona tempo
                    if (consecutive >= 4)
                    {
                        if (RadialTimer.Instance != null)
                        {
                            RadialTimer.Instance.AddTime(10f);
                            if (timeBonusTMP != null)
                            {
                                if (bonusCoroutine != null) StopCoroutine(bonusCoroutine);
                                bonusCoroutine = StartCoroutine(ShowTimeBonus());
                            }
                        }
                    }
                }
            }
        }

        // Verificar linhas verticais de 4+ frutas
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allFruits[x, y] != null && matched.Contains(allFruits[x, y]))
                {
                    int fruitType = allFruits[x, y].GetComponent<Fruit>().type;
                    int consecutive = 1;

                    // Contar para cima
                    for (int nextY = y + 1; nextY < height; nextY++)
                    {
                        if (allFruits[x, nextY] != null && 
                            allFruits[x, nextY].GetComponent<Fruit>().type == fruitType &&
                            matched.Contains(allFruits[x, nextY]))
                        {
                            consecutive++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Se tem 4 ou mais em linha, adiciona tempo
                    if (consecutive >= 4)
                    {
                        if (RadialTimer.Instance != null)
                        {
                            RadialTimer.Instance.AddTime(10f);
                            if (timeBonusTMP != null)
                            {
                                if (bonusCoroutine != null) StopCoroutine(bonusCoroutine);
                                bonusCoroutine = StartCoroutine(ShowTimeBonus());
                            }
                            Debug.Log("Bónus! 4+ frutas em linha vertical! +10 segundos!");
                        }
                    }
                }
            }
        }
    }

    IEnumerator ShowTimeBonus()
    {
        if (timeBonusTMP == null)
            yield break;

        // try to get TMP and RectTransform
        var tmp = timeBonusTMP.GetComponent<TextMeshProUGUI>();
        var rt = timeBonusTMP.GetComponent<RectTransform>();

        timeBonusTMP.SetActive(true);

        Vector2 startPos = Vector2.zero;
        if (rt != null) startPos = rt.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * bonusMoveDistance;

        if (tmp != null)
        {
            Color c = tmp.color;
            c.a = 1f;
            tmp.color = c;
        }

        float elapsed = 0f;
        while (elapsed < bonusDisplayDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / bonusDisplayDuration);

            if (rt != null)
                rt.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            if (tmp != null)
            {
                Color c = tmp.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                tmp.color = c;
            }

            yield return null;
        }

        if (rt != null)
            rt.anchoredPosition = startPos;

        if (tmp != null)
        {
            Color c = tmp.color;
            c.a = 0f;
            tmp.color = c;
        }

        timeBonusTMP.SetActive(false);
        bonusCoroutine = null;
    }

    IEnumerator ShowPointsBonus(int points)
    {
        if (pointsTMP == null)
            yield break;

        // try to get TMP and RectTransform
        var tmp = pointsTMP.GetComponent<TextMeshProUGUI>();
        var rt = pointsTMP.GetComponent<RectTransform>();

        pointsTMP.SetActive(true);

        // Set the points text
        if (tmp != null)
        {
            tmp.text = "+" + points.ToString();
        }

        Vector2 startPos = Vector2.zero;
        if (rt != null) startPos = rt.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * pointsMoveDistance;

        if (tmp != null)
        {
            Color c = tmp.color;
            c.a = 1f;
            tmp.color = c;
        }

        float elapsed = 0f;
        while (elapsed < pointsDisplayDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / pointsDisplayDuration);

            if (rt != null)
                rt.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            if (tmp != null)
            {
                Color c = tmp.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                tmp.color = c;
            }

            yield return null;
        }

        if (rt != null)
            rt.anchoredPosition = startPos;

        if (tmp != null)
        {
            Color c = tmp.color;
            c.a = 0f;
            tmp.color = c;
        }

        pointsTMP.SetActive(false);
        pointsCoroutine = null;
    }

    // ======================================================
    // SWAP COM VIZINHO (para swipe / arrastar)
    // ======================================================
    public void SwapWithNeighbor(Fruit fruit, Vector2 dir)
    {
        
        if (isProcessing) return;

        // garantir valores inteiros e bloquear diagonais
        int dirX = Mathf.RoundToInt(dir.x);
        int dirY = Mathf.RoundToInt(dir.y);

        // se houver componente em x e y ao mesmo tempo, ignora (sem diagonal)
        if (dirX != 0 && dirY != 0) return;

        int targetX = fruit.x + dirX;
        int targetY = fruit.y + dirY;

        // checar limites
        if (targetX < 0 || targetX >= width || targetY < 0 || targetY >= height)
            return;

        GameObject targetObj = allFruits[targetX, targetY];
        if (targetObj == null) return;

        Fruit neighbor = targetObj.GetComponent<Fruit>();
        if (neighbor == null) return;

        // inicia troca + verificação
        StartCoroutine(SwapAndCheck(fruit, neighbor));
    }

}

