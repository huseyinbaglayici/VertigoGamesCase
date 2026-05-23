using Runtime.Data.UnityObjects;
using Runtime.Enums;
using Runtime.Interfaces;
using TMPro;
using UnityEngine;
using Zenject;

namespace Runtime.UI
{
    public class ZoneAreaView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _silverText;
        [SerializeField] private TextMeshProUGUI _goldText;

        private const string SilverTextName = "ui_text_area_silver_value";
        private const string GoldTextName = "ui_text_area_gold_value";

        private IZoneManager _zoneManager;
        private SO_GameConfig _gameConfig;
        private string _silverLabel;
        private string _goldLabel;

        private void OnValidate()
        {
            foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (text.gameObject.name == SilverTextName) _silverText = text;
                if (text.gameObject.name == GoldTextName) _goldText = text;
            }
        }

        [Inject]
        public void Construct(IZoneManager zoneManager, SO_GameConfig gameConfig)
        {
            _zoneManager = zoneManager;
            _gameConfig = gameConfig;
            _silverLabel = gameConfig.stageConfig.silverAreaLabel;
            _goldLabel = gameConfig.stageConfig.goldAreaLabel;
        }

        private void Start()
        {
            _zoneManager.OnZoneChanged += UpdateTexts;
            UpdateTexts(_zoneManager.CurrentZone);
        }

        private void OnDestroy()
        {
            if (_zoneManager != null)
                _zoneManager.OnZoneChanged -= UpdateTexts;
        }

        private void UpdateTexts(int zone)
        {
            int nextSilver = GetNextZoneOfType(zone, ZoneType.Silver);
            int nextGold = GetNextZoneOfType(zone, ZoneType.Gold);

            _silverText.text = nextSilver > 0 ? $"{_silverLabel} {nextSilver}" : string.Empty;
            _goldText.text = nextGold > 0 ? $"{_goldLabel} {nextGold}" : string.Empty;
        }

        private int GetNextZoneOfType(int currentZone, ZoneType type)
        {
            for (int z = currentZone + 1; z <= _gameConfig.goldZoneInterval; z++)
                if (_zoneManager.GetZoneType(z) == type)
                    return z;
            return 0;
        }
    }
}