using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using YG;
public class Manager : MonoBehaviour
{
    public static Manager Instance { get; private set; }

    public List<GameObject> Checkpoints = new();
    int TargetCheckpoints = 0;

    public TMP_Text MoneyText;
    public TMP_Text ScoreText;

    public List<GameObject> CoinsList = new();
    private bool[] CoinIsRaised;

    public AudioClip[] Sounds;
    public AudioSource CastSound;

    [HideInInspector] public int Money
    {
        get
        {
            return _money;
        }
        set
        {
            _money = value;
            MoneyText.text = value.ToString();
        }
    }
    int _money;
    [HideInInspector] public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            value = Mathf.Clamp(value, 0, int.MaxValue);
            _score = value;
            ScoreText.text = value.ToString();
            if (value > BestScore)
            {
                BestScore = value;
                Menu.Instance.ScoreLb.NewScore(value);
            }
        }
    }
    int _score;
    int BestScore;

    public GameObject Player;
    public Transform Camera;

    private Rigidbody2D PlayerRb;

    public float Force;

    private Vector2 OldMousePos;
    public SpriteRenderer line;

    private bool IsScope = false;
    private void Awake()
    {
        Instance = this;
        Debug.Log(YandexGame.savesData.CoinIsRaised);
        if (YandexGame.savesData.CoinIsRaised == null)
        {
            CoinIsRaised = new bool[CoinsList.Count];
            YandexGame.savesData.CoinIsRaised = CoinIsRaised;
        }
        else
        {
            CoinIsRaised = new bool[CoinsList.Count];
            YandexGame.savesData.CoinIsRaised.CopyTo(CoinIsRaised, 0);
            YandexGame.savesData.CoinIsRaised = CoinIsRaised;
        }
        TargetCheckpoints = YandexGame.savesData.TargetCheckpoints;
        int _money = 0;
        for (int i = 0; i < CoinIsRaised.Length; i++)
        {
            if (CoinIsRaised[i])
            {
                _money++;
            }
        }
        Money = _money;

        DisableCheckpoints();
        DisableMoney();

        PlayerRb = Player.GetComponent<Rigidbody2D>();

        Player.transform.position = Checkpoints[TargetCheckpoints].transform.position;

        line.enabled = false;
    }
    void Update()
    {
        Camera.position = Player.transform.position;
        if (PlayerRb.velocity.magnitude < .1f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OldMousePos = Input.mousePosition;
                line.enabled = true;
                IsScope = true;
            }
            if (IsScope)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector2 NowPos = Input.mousePosition;
                    float LineDistance = Vector2.Distance(OldMousePos, NowPos);
                    LineDistance = LineDistance / ((Screen.height + Screen.width) / 2) * 1500;
                    if (LineDistance > 1000)
                        LineDistance = 1000;
                    float angle = Angle(NowPos, OldMousePos);
                    line.transform.rotation = Quaternion.Euler(0, 0, angle + 180);
                    line.transform.position = Player.transform.position;
                    line.size = new Vector2(LineDistance / 25, 1);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    CastSound.Play();
                    Vector2 NowPos = Input.mousePosition;
                    line.enabled = false;
                    float LineDistance = Vector2.Distance(OldMousePos, NowPos);
                    LineDistance = LineDistance / ((Screen.height + Screen.width) / 2) * 1500;
                    if (LineDistance > 1000)
                        LineDistance = 1000;
                    PlayerRb.AddForce(new Vector3(OldMousePos.x - NowPos.x, OldMousePos.y - NowPos.y).normalized * LineDistance * Force);
                    int x = -1;
                    if (OldMousePos.x - NowPos.x > 0)
                        x = 1;
                    PlayerRb.AddTorque(-LineDistance / 15 * x);
                    IsScope = false;
                }
            }
        }
    }
    public void Kill()
    {
        StartCoroutine(KillCorutine());
    }
    public IEnumerator KillCorutine()
    {
        SpriteRenderer PlayerSkin = Player.GetComponent<SpriteRenderer>();
        TrailRenderer trailRenderer = Player.GetComponent<TrailRenderer>();
        for (int i = 0; i < 30; i++)
        {
            PlayerSkin.color = new Color(1,1,1, 1 - ((i + 1f) / 30f));
            trailRenderer.startColor = new Color(1, 1, 1, 1 - ((i + 1f) / 30f));
            yield return new WaitForSeconds(.03f);
        }
        PlayerSkin.color = Color.white;
        trailRenderer.startColor = Color.white;
        Player.transform.position = Checkpoints[TargetCheckpoints].transform.position;
        Player.transform.rotation = Quaternion.identity;
        PlayerRb.velocity = Vector2.zero;
        trailRenderer.Clear();
    }
    public void NextCheckpoint(GameObject point)
    {
        TargetCheckpoints = Checkpoints.FindIndex(x => x.Equals(point));
        YandexGame.savesData.TargetCheckpoints = TargetCheckpoints;
        YandexGame.SaveProgress();
        DisableCheckpoints();
    }
    private void DisableCheckpoints()
    {
        for (int i = 0; i <= TargetCheckpoints; i++)
        {
            Checkpoints[i].SetActive(false);
        }
    }
    public void AddCoin(GameObject coin)
    {
        int i = CoinsList.FindIndex(x => x.Equals(coin));
        CoinsList[i].SetActive(false);
        CoinIsRaised[i] = true;
        Money++;
        Menu.Instance.MoneyeLb.NewScore(Money);
        YandexGame.savesData.CoinIsRaised = CoinIsRaised;
        YandexGame.savesData.Money = Money;
        YandexGame.SaveProgress();
        StartCoroutine(PlaySound());
    }
    private void DisableMoney()
    {
        for (int i = 0; i < CoinsList.Count; i++)
        {
            if (CoinIsRaised[i])
            {
                CoinsList[i].SetActive(false);
            }
        }
    }
    public IEnumerator PlaySound()
    {
        AudioSource MoneySound = Player.AddComponent<AudioSource>();
        MoneySound.clip = Sounds[Random.Range(0, Sounds.Length)];
        MoneySound.Play();
        yield return new WaitForSeconds(1.5f);
        Destroy(MoneySound);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    private float Angle(Vector2 F, Vector2 S)
    {
        Vector2 correct = F - S;
        correct = correct.normalized;
        float Angle = Mathf.Acos(correct.x) * Mathf.Rad2Deg;
        if (correct.y < 0)
            Angle = -Angle;
        return Angle;
    }
}