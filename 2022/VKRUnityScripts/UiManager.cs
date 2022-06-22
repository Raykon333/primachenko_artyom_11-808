using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] Text timeText;
    [SerializeField] Text fpsText;
    [SerializeField] InputField simulationSpeedInput;
    [SerializeField] Text PopulationCount;
    [SerializeField] GameManager gameManager;
    public bool started = false;

    float fpsTimer = 0f;
    float fpsInterval = 1f;

    // Update is called once per frame
    void Update()
    {
        if (!started)
            return;
        var time = gameManager.EntityManager.GetComponentData<TimeUniqueComponent>(gameManager.StaticEntity);

        var day = time.Day;
        var dayOfWeek = time.DayOfWeek;
        var hour = (int)(time.Seconds * 24 / TimeSystem.secondsInDay);
        var minute = (int)(time.Seconds * 24 % TimeSystem.secondsInDay / TimeSystem.secondsInDay * 60);

        var hourString = hour.ToString();
        if (hourString.Length == 1)
            hourString = "0" + hourString;

        var minuteString = minute.ToString();
        if (minuteString.Length == 1)
            minuteString = "0" + minuteString;

        timeText.text = $"Day {day}, {dayOfWeek}, {hourString}:{minuteString}";

        fpsTimer += Time.deltaTime;
        if (fpsTimer > fpsInterval)
        {
            fpsTimer -= fpsInterval;
            fpsText.text = ((int)(1f / Time.unscaledDeltaTime)).ToString() + " FPS";
        }

        var text = simulationSpeedInput.text;
        int speed = 0;
        if (text != "")
            speed = int.Parse(simulationSpeedInput.text);
        if (speed < 0)
            speed = 0;
        if (speed > 10000)
            speed = 10000;
        simulationSpeedInput.text = speed.ToString();

        var staticComponent = gameManager.EntityManager.GetComponentData<StaticComponent>(gameManager.StaticEntity);
        staticComponent.SimulationSpeed = speed;
        gameManager.EntityManager.SetComponentData(gameManager.StaticEntity, staticComponent);

        PopulationCount.text = "Population Count: " + staticComponent.UnitsCount.ToString();
    }
}
