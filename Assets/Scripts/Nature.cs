using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class Nature : MonoBehaviour
{
    [Header("Day/Night Cycle")]
    [SerializeField] private int hour;
    [SerializeField] private int day;
    [SerializeField] private float secondsInHour;
    [SerializeField] private Light2D Light;
    [SerializeField] private Gradient lightColorOverDay;

    [Header("Weather Settings")]
    [SerializeField] private float rainDuringHour;
    [SerializeField] private float minRainAmountPerHour;
    [SerializeField] private float maxRainAmountPerHour;
    [SerializeField] private float rainReductionPerHour;
    [SerializeField] private float rainChance;
    [SerializeField] private ParticleSystem rain;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource nightSounds;
    [SerializeField] private AudioSource lightRainSounds;
    [SerializeField] private AudioSource heavyRainSounds;

    private float targetLightRain;
    private float targetHeavyRain;
    private float lastLightRain;
    private float lastHeavyRain;

    private float hourProgress;
    private float rainAmout;
    private float rainAmoutPreviousHour;
    private bool isNight;

    private ParticleSystem.EmissionModule rainEmission;

    public event Action<int> NextHour;
    public event Action<int> NewDay;

    private void Start()
    {
        rainEmission = rain.emission;
        StartCoroutine(HourTimer());
    }

    private void Update()
    {
        hourProgress += Time.deltaTime;
        float t = hourProgress / secondsInHour;
        float totalHours = hour + t;
        float normalizedTime = totalHours / 24f;

        Light.color = lightColorOverDay.Evaluate(normalizedTime);
        rainAmout = Mathf.Lerp(rainAmoutPreviousHour, rainDuringHour, t);
        rainEmission.rateOverTime = rainAmout;

        lightRainSounds.volume = Mathf.Lerp(lastLightRain, targetLightRain, t);
        heavyRainSounds.volume = Mathf.Lerp(lastHeavyRain, targetHeavyRain, t);
    }

    private IEnumerator HourTimer()
    {
        while (true)
        {
            hourProgress = 0;

            rainAmoutPreviousHour = rainDuringHour;
            lastLightRain = targetLightRain;
            lastHeavyRain = targetHeavyRain;

            WeatherChange();

            yield return new WaitForSeconds(secondsInHour);
        }
    }

    private IEnumerator FastVolumeChange(AudioSource source, float target, float time)
    {
        float start = source.volume;
        if (Mathf.Approximately(start, target)) yield break;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / time;
            source.volume = Mathf.Lerp(start, target, t);
            yield return null;
        }

        source.volume = target;
    }

    private void WeatherChange()
    {
        hour++;
        NextHour?.Invoke(hour);

        isNight = hour < 5 || hour > 20;
        StartCoroutine(FastVolumeChange(nightSounds, isNight ? 1f : 0f, secondsInHour));

        if (hour > 23)
        {
            day++;
            hour = 0;
            NewDay?.Invoke(day);
        }

        rainDuringHour -= rainReductionPerHour;

        if (Random.Range(0f, 100f) < rainChance)
        {
            rainDuringHour += Random.Range(minRainAmountPerHour, maxRainAmountPerHour);
        }

        rainDuringHour = Mathf.Clamp(rainDuringHour, 0f, 900f);

        rain.gameObject.SetActive(rainDuringHour > 0 || rainAmout > 0);

        targetLightRain = rainDuringHour != 0 ? Mathf.Lerp(0.5f, 1f, Mathf.Min(400f, rainDuringHour) / 400f) : 0f;
        targetHeavyRain = rainDuringHour > 400f ? Mathf.Lerp(0.3f, 1f, (rainDuringHour - 400f) / 500f) : 0f;
    }
}