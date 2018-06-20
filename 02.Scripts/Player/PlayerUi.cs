using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour {
	[SerializeField]
	private GameObject Energy;

    [SerializeField]
    private GameObject Health;
    
    public GameObject backCamMask;

	public GameObject miniCamMask;

    public GameObject aimCircle;

    public GameObject[] invenBG;

    [SerializeField]
    private PlayerStats playerStats;

    Image EnergyImage;
    Image HealthImage;

    private void Start()
	{
		EnergyImage = Energy.GetComponent<Image>();
        HealthImage = Health.GetComponent<Image>();
    }

	public void SetEnergy(float e)
    {
        EnergyImage.fillAmount = e / playerStats.EnergyMax;
    }

    public void SetHealth(float h)
    {
        HealthImage.fillAmount = h / playerStats.HealthMax;        
    }

    public void SetEnergyBar(float e)
    {
        EnergyImage.fillAmount = e / playerStats.EnergyMax;
    }

    public void SetHealthBar(float h)
    {
        HealthImage.fillAmount = h / playerStats.HealthMax;
    }
}
