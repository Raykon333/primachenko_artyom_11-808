using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class StatCollector : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [SerializeField] Text StatTexts;
    [SerializeField] Text StatValues;

    // Update is called once per frame
    void Update()
    {
        var entityManager = gameManager.EntityManager;
        List<string> statTexts = new List<string>();
        List<float> statValues = new List<float>();
        var statTrackers = gameManager.StatTrackers;
        foreach (var statTracker in statTrackers)
        {
            string prefix = "";
            if (entityManager.HasComponent<StatTrackingComponent>(statTracker))
            {
                var statComponent = entityManager.GetComponentData<StatTrackingComponent>(statTracker);
                prefix = statComponent.Name + " " + statComponent.TrackedResource + " ";
            }
            if (entityManager.HasComponent<SatietyTrackingComponent>(statTracker))
            {
                var satietyComponent = entityManager.GetComponentData<SatietyTrackingComponent>(statTracker);
                prefix = satietyComponent.Name + " ";
            }
            if (entityManager.HasComponent<ResourceMinComponent>(statTracker))
            {
                var resourceMin = entityManager.GetComponentData<ResourceMinComponent>(statTracker);
                statTexts.Add(prefix + "min:");
                statValues.Add(resourceMin.Value);
            }
            if (entityManager.HasComponent<ResourceAvgComponent>(statTracker))
            {
                var resourceAvg = entityManager.GetComponentData<ResourceAvgComponent>(statTracker);
                statTexts.Add(prefix + "avg:");
                statValues.Add(resourceAvg.Value);
            }
            if (entityManager.HasComponent<ResourceMaxComponent>(statTracker))
            {
                var resourceMax = entityManager.GetComponentData<ResourceMaxComponent>(statTracker);
                statTexts.Add(prefix + "max:");
                statValues.Add(resourceMax.Value);
            }
        }

        StatTexts.text = "";
        foreach(var text in statTexts)
        {
            StatTexts.text += text + "\n";
        }
        StatValues.text = "";
        foreach(var value in statValues)
        {
            StatValues.text += value.ToString("0.00") + "\n";
        }
    }
}
