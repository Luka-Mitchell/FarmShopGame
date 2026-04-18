using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// obsolete, replaced by BuildingData, buildingdatamanager and BuildingStateManager, but keeping for future reference

[System.Serializable]
public class BuildType
{
    public string buildName;
    public int maxQuantity;
    public int currentQuantity;
    public int buildCost;
    //public int buildXP;
    public Sprite buildSprite;
    public bool isUnlocked = false;
    public GameObject prefab;
    public GameObject ghost;
}

public class BuildTypeManager : MonoBehaviour
{
    public static BuildTypeManager Instance;

    public List<BuildType> buildTypes;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public BuildType GetBuildType(string name)
    {
        BuildType build = buildTypes.Find(x => x.buildName == name);
        //Debug.Log("build.buildName: " + build.buildName + " " + "name: " + name);
        return build;
    }

    public void UnlockBuild(string name)
    {
        BuildType build = buildTypes.Find(x => x.buildName == name);
        build.isUnlocked = true;
        Debug.Log("Unlocking " + name);
    }

}