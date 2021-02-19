using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BattleView : MonoBehaviour,IDragHandler
{
    [SerializeField] private CircleView _ballPreff;
    [SerializeField] private Transform _content;
    [SerializeField] private Slider _speedSlider;
    [SerializeField] private Image _blueImage;
    [SerializeField] private Image _battleOverPanel;
    [SerializeField] private Text _resultText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private GameObject fireworkToPool;
    List<GameObject> pooledFireworks;


    private List<CircleView> _balls;
    public event System.Action<float> SliderValueChanged;
    public event System.Action RestartButtonPressed;
    public event System.Action<float> ScreenMoved;
    public event System.Action ObjectsCreated;
    public void Init()
    {
        _speedSlider.onValueChanged.AddListener((s) => SliderValueChanged?.Invoke(s));
        _restartButton.onClick.AddListener(()=>RestartButtonPressed?.Invoke());

        pooledFireworks = new List<GameObject>();
        GameObject temp;
        for (int i = 0; i < 10; i++)
        {
            temp = Instantiate(fireworkToPool);
            temp.gameObject.SetActive(false);
            pooledFireworks.Add(temp);
        }
    }

    public GameObject GetPooledFirework()
    {
        for (int i = 0; i < pooledFireworks.Count; i++)
        {
            if (!pooledFireworks[i].activeInHierarchy)
            {
                return pooledFireworks[i];
            }
        }
        GameObject temp = Instantiate(fireworkToPool);
        temp.gameObject.SetActive(false);
        pooledFireworks.Add(temp);
        return temp;
    }


    public void UpdateCircles()
    {
        foreach (var ball in _balls)
        {
            ball.UpdateData();
        }
    }

    public void ShowRatio(float value)
    {
        _blueImage.fillAmount = value;
    }

    private void HideCircle(CircleView ball)
    {
        _balls.Remove(ball);
        ball.gameObject.SetActive(false);

        var explosion = GetPooledFirework();
        explosion.transform.position = ball.transform.position;
        explosion.SetActive(true);
        StartCoroutine(DeactivateExplosion(explosion));
    }

    IEnumerator DeactivateExplosion(GameObject explosion)
    {
        yield return new WaitForSeconds(3);
        explosion.SetActive(false);
    }

    public void ShowBattleOverPanel(float time, string winner)
    {
        _battleOverPanel.gameObject.SetActive(true);
        _resultText.text = $"Симуляция закончилась за {time} секунд\n Победивший цвет: {winner}.";
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            ScreenMoved.Invoke(eventData.delta.x);
        }
    }

    public void CreateObjects(Circle[] circles, float spawnDelay)
    {
        StartCoroutine(CreateObjectsCourutine(circles, spawnDelay));
    }
    private IEnumerator CreateObjectsCourutine(Circle[] circles, float spawnDelay)
    {
        var wait = new WaitForSeconds(spawnDelay);
        _balls = new List<CircleView>();

        Shuffle(circles);
        foreach (var circle in circles)
        {
            CircleView ball = Instantiate(_ballPreff, _content);
            ball.name = "Ball_" + circle.Color.ToString();
            _balls.Add(ball);
            ball.Init(circle);
            circle.Hided += () => HideCircle(ball);
            ball.UpdateData();
            yield return wait;
        }
        ObjectsCreated?.Invoke();
        _speedSlider.interactable = true;
    }

    private void Shuffle(Circle[] circles)
    {
        int n = circles.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0,n + 1);
            var value = circles[k];
            circles[k] = circles[n];
            circles[n] = value;
        }
    }

}
