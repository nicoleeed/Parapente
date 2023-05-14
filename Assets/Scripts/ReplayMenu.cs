using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ReplayMenu : MonoBehaviour
{
    public Dropdown dropdownMenu;
    public Button replayButton;
    public Button deleteFileButton;
    public Canvas deleteFileCanvas;
    public Text replayName;

    private int selectedId = 0;
    private List<string> jsonFiles;

    // Start is called before the first frame update
    void Start()
    {
        jsonFiles = new List<string>();
        deleteFileButton.interactable = true;
        dropdownMenu.interactable = true;
        Populate();
    }

    /// <summary>
    /// Populates dropdown list of recorded demos with all the json files it can find in Saves subfolder
    /// </summary>
    private void Populate()
    {
        if (Directory.Exists(Globals.saveFolder))
        {
            foreach (string fileName in Directory.GetFiles(Globals.saveFolder))
            {
                if (fileName.Split('.')[fileName.Split('.').Length-1] == "json")
                {
                    jsonFiles.Add(fileName.Replace(Application.persistentDataPath, ""));
                }
            }
        }
        if (jsonFiles.Count > 0)
        {
            dropdownMenu.interactable = true;
            dropdownMenu.AddOptions(jsonFiles);
        } else
        {
            deleteFileButton.interactable = false;
            replayButton.interactable = false;
            dropdownMenu.ClearOptions();
            dropdownMenu.interactable = false;
        }
    }

    /// <summary>
    /// Changes the selected demo id
    /// </summary>
    public void OnValueSelected()
    {
        selectedId = dropdownMenu.value;
        replayButton.interactable = true;
        deleteFileButton.interactable = true;
    }

    /// <summary>
    /// Shows up file deletion confirmation menu
    /// </summary>
    public void OnDeleteButton()
    {
        deleteFileCanvas.enabled = true;
        SetSelectedReplayNameToDelete();
    }

    /// <summary>
    /// Sets the text to match the name of the selected replay to delete
    /// </summary>
    public void SetSelectedReplayNameToDelete()
    {
        replayName.text = jsonFiles[selectedId];
    }

    /// <summary>
    /// Deletes the selected replay and clears the dropdown menu and returns to replay menu
    /// </summary>
    /// <exception cref="IOException">
    /// Returns exception if file deletion was not succesful
    /// </exception>
    public void DeleteReplay()
    {
        try
        {
            File.Delete(Application.persistentDataPath + jsonFiles[selectedId]);
            jsonFiles.Clear();
            dropdownMenu.options.Clear();
            selectedId = dropdownMenu.value;
            Populate();
        } catch (IOException)
        {
            Debug.LogError("Error deleting file");
        } finally {
            ReturnToReplayMenu();
        }

    }

    /// <summary>
    /// Hides the file deletion confirmation menu
    /// </summary>
    public void ReturnToReplayMenu()
    {
        deleteFileCanvas.enabled = false;
    }

    /// <summary>
    /// Returns the name of selected replay
    /// </summary>
    /// <returns>
    /// String of a name of selected replay
    /// </returns>
    public string GetSelected()
    {
        return jsonFiles[selectedId];
    }

}
