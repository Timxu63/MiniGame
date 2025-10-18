using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public int a;
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    public void OnStart()
    {
        Addressables.LoadAssetAsync<GameObject>("Assets/_Resources/Prefab/Main/Cube.prefab").Completed += op =>
        {
            GameObject obj = Instantiate(op.Result);
            obj.transform.position = Vector3.zero;
        };
        Addressables.LoadAssetAsync<GameObject>("Assets/_Resources/Prefab/Battle/Sphere.prefab").Completed += op =>
        {
            GameObject obj = Instantiate(op.Result);
            obj.transform.position = new Vector3(0, 1, 0);
        };
        
    }

    // Update is called once per frame
    void Update()
    {

        text.text = a.ToString();
        a++;
    }
}
