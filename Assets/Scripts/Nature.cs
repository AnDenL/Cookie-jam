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
    [SerializeField] private ParticleSystem snow;
    [SerializeField] private ParticleSystem fireflys;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource nightSounds;
    [SerializeField] private AudioSource lightRainSounds;
    [SerializeField] private AudioSource heavyRainSounds;

    private float targetLightRain;
    private float targetHeavyRain;
    private float lastLightRain;
    private float lastHeavyRain;

    private float hourProgress;
    private float precipitationAmount;
    private float rainAmoutPreviousHour;
    private bool isNight;

    private ParticleSystem.EmissionModule rainEmission;
    private ParticleSystem.EmissionModule snowEmission;
    private ParticleSystem.EmissionModule ffEmission;
    
    public event Action<int> NextHour;
    public event Action<int> NewDay;

    private void Start()
    {
        rainEmission = rain.emission;
        snowEmission = snow.emission;
        ffEmission = fireflys.emission;
        StartCoroutine(HourTimer());
    }

    private void Update()
    {
        hourProgress += Time.deltaTime;
        float t = hourProgress / secondsInHour;
        float totalHours = hour + t;
        float normalizedTime = totalHours / 24f;

        Light.color = lightColorOverDay.Evaluate(normalizedTime);
        precipitationAmount = Mathf.Lerp(rainAmoutPreviousHour, rainDuringHour, t);

        if (Generation.CurrentBiome == Biome.Forest && (hour > 19 || hour < 3))
        {
            ffEmission.rateOverTime = 10;
        }
        else
            ffEmission.rateOverTime = 0;

        if (Generation.CurrentBiome == Biome.Field || Generation.CurrentBiome == Biome.Forest) 
        {  
            rainEmission.rateOverTime = precipitationAmount;
            snowEmission.rateOverTime = 0;
            lightRainSounds.volume = Mathf.Lerp(lastLightRain, targetLightRain, t);
            heavyRainSounds.volume = Mathf.Lerp(lastHeavyRain, targetHeavyRain, t);
        }
        else if (Generation.CurrentBiome == Biome.Snow)
        {
            rainEmission.rateOverTime = 0;
            snowEmission.rateOverTime = precipitationAmount / 3;
            lightRainSounds.volume = Mathf.Lerp(lightRainSounds.volume, 0, Time.deltaTime * 2);
            heavyRainSounds.volume = Mathf.Lerp(heavyRainSounds.volume, 0, Time.deltaTime * 2);
        }
        else
        {
            rainEmission.rateOverTime = 0;
            snowEmission.rateOverTime = 0;
            lightRainSounds.volume = Mathf.Lerp(lightRainSounds.volume, 0, Time.deltaTime * 2);
            heavyRainSounds.volume = Mathf.Lerp(heavyRainSounds.volume, 0, Time.deltaTime * 2);
        }
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

        bool Weather = rainDuringHour > 0 || precipitationAmount > 0;
        rain.gameObject.SetActive(Weather);
        snow.gameObject.SetActive(Weather);

        targetLightRain = rainDuringHour != 0 ? Mathf.Lerp(0.5f, 1f, Mathf.Min(400f, rainDuringHour) / 400f) : 0f;
        targetHeavyRain = rainDuringHour > 400f ? Mathf.Lerp(0.3f, 1f, (rainDuringHour - 400f) / 500f) : 0f;
    }
}