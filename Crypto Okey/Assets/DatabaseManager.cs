using Firebase;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    FirebaseDatabase database;
    private DatabaseReference usersRef;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeDatabase());
    }

    private IEnumerator InitializeDatabase()
    {
        var task = FirebaseApp.CheckAndFixDependenciesAsync();
        while (!task.IsCompleted)
        {
            yield return null;
        }

        if(task.IsCanceled || task.IsFaulted)
        {
            Debug.Log("Databae Error" + task.Exception.ToString());
        }

        var dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available)
        {
            database = FirebaseDatabase.GetInstance("https://cryptookey-default-rtdb.firebaseio.com/");
            usersRef = database.GetReference("Users");
            Debug.Log("Database Init Complete");
        }

        else Debug.Log("Database Error");
    }
}
