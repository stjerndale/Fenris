using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveToJson
{
    public void SaveMapToJson(BoxLogic.Type[] types, bool[] dryness, Flower[] plantList)
    {
        MapSaveData mapSaveData = new MapSaveData();
        mapSaveData.tileTypes = types;
        mapSaveData.tilesDryness = dryness;
        mapSaveData.plants = PlantToPlantSaveData(plantList);
        mapSaveData.TimeStamp = GameInformation.TimeTicks;

        string json = JsonUtility.ToJson(mapSaveData, true);
        File.WriteAllText(Application.dataPath +  "/Save Files/" + SceneManager.GetActiveScene().name + ".json", json);
        Debug.Log("Map Saved");
    }

    public void LoadMapFromJson(Transform grid)
    {
        string json = File.ReadAllText(Application.dataPath + "/Save Files/" + SceneManager.GetActiveScene().name + ".json");
        MapSaveData mapSaveData = JsonUtility.FromJson<MapSaveData>(json);
        BoxLogic box = grid.GetChild(0).GetComponent<BoxLogic>();
        int TimePassed = GameInformation.TimeTicks - mapSaveData.TimeStamp;

        // set all the box types (ground, water etc)
        for (int i = 0; i < grid.childCount; i++)
        {
            box = grid.GetChild(i).GetComponent<BoxLogic>();
            box.SetActiveType(mapSaveData.tileTypes[i]);
        }

        // tile type for all tiles must be set before plant is planted so the growth requirements are checking correctly from the start
        for(int i = 0; i < grid.childCount; i++)
        {
            box = grid.GetChild(i).GetComponent<BoxLogic>();
            box.SetDryness(mapSaveData.tilesDryness[i]); // dryness needs to be updated after the water tiles have been correctly placed

            if (box.plant != null)
            {
                box.DestroyLocalPlant(); // Remove all flowers previously in the grid
            }
            if(mapSaveData.plants[i].plantInfo != null) // Setup the flowers based on saved information
            {
                LoadPlantFromSaveData(mapSaveData.plants[i], box, TimePassed);
            }
        }
        box.GetComponentInParent<GridHandler>().UpdateStats();
        Debug.Log("Map Loaded");
    }

    private PlantSaveData[] PlantToPlantSaveData(Flower[] plants)
    {
        PlantSaveData[] plantSaveData = new PlantSaveData[plants.Length];
        Flower plant;

        for(int i = 0; i < plants.Length; i++)
        {
            if (plants[i] != null)
            {
                plant = plants[i].GetComponent<Flower>();
                plantSaveData[i] = new PlantSaveData(plant.flowerInfo, plant.GetState());
                if (plantSaveData[i].state == Flower.State.Growing)
                {
                    plantSaveData[i].state = plant.GetPreviousState();
                }
            }
        }

        return plantSaveData;
    }

    // Setup a flower based on saved information
    private void LoadPlantFromSaveData(PlantSaveData plantSaveData, BoxLogic box, int TimePassed)
    {
        box.AddFlower(plantSaveData.plantInfo);
        Flower plant = box.plant.GetComponent<Flower>();
        plant.SetupFlower(plantSaveData.plantInfo);
        plant.SetState(plantSaveData.state);
        if (!box.isDry()) // if box is wet, prompt flower growth
        {
            plant.StartCoroutine(plant.Grow()); // this *can* cause issues if one repeatedly loads in quick succession while plants are growing (get destroyed while coroutine is running)
        }
    }

    // Setup a flower based on saved information
    // If enough time has passed that the plant should have grown while the player was away, then have it grow
    // Note: should the plants be able to spread off-screen as well?
    private void LoadPlantFromSaveDataAndGrow(PlantSaveData plantSaveData, BoxLogic box, int TimePassed)
    {
        box.AddFlower(plantSaveData.plantInfo);
        Flower plant = box.plant.GetComponent<Flower>();
        plant.SetupFlower(plantSaveData.plantInfo);
        Flower.State state = plantSaveData.state;

        if (plant.GrowthRequirementsFulfilled(box) && state != Flower.State.Bloom && (!box.isDry() || !plant.NeedsWaterToGrow()))
        {
            int j = 1;
            while (TimePassed > plantSaveData.plantInfo.growthSpeed * j)
            {
                state = plant.ProgressState(state);
                j++;
                if (plant.NeedsWaterToGrow() && ! box.IsWaterAdjecent()) // if the plant is not adjecent to water, the ground would go dry after growing once
                {
                    break;
                }
            }
        }

        plant.SetState(state);
        if (!box.isDry()) // if box is wet, prompt flower growth
        {
            plant.StartCoroutine(plant.Grow()); // this *can* cause issues if one repeatedly loads in quick succession while plants are growing (get destroyed while coroutine is running)
        }
    }

    public void SaveGameInformationToJson()
    {
        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.TimeTicksPassed = GameInformation.TimeTicks;

        string json = JsonUtility.ToJson(gameSaveData, true);
        File.WriteAllText(Application.dataPath + "/Save Files/gameInfo.json", json);
        Debug.Log("Game Saved");
    }

    public void LoadGameInformationFromJson()
    {

        string json = File.ReadAllText(Application.dataPath + "/Save Files/gameInfo.json");
        GameSaveData gameSaveData = JsonUtility.FromJson<GameSaveData>(json);

        GameInformation.TimeTicks = gameSaveData.TimeTicksPassed;
    }
}
