using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class EncryptionTool : MonoBehaviour
{
    public InputField webSiteField;
    public InputField keyWordField;
    public InputField NameField;
    public GameObject dropDown;
    public int safeLevel = 12;

    public Text debugTxt;

    Dropdown drop;

    public TextAsset dataSet;
    string[] dataSetArray;
    string[] dataSetArrayCache;

    string[] dataFromSet;
    string keyWord;

    string keyLong = "1234567890abcdefghijklmnopqrstuvwxyz";
    string keySpecial = "_,.?";
    string keyFull;

    public string finalKey = "";
    public InputField finalKeyDisplay;

    public GameObject panalAccount;
    public GameObject panalPassword;
    bool isMainPage=true;

    //Account Page:
    public GameObject accountScroll;
    public GameObject textItem;
    public GameObject styleDropBox;
    public Transform contentAccount;
    public Dropdown dropFreqLevel;
    public Dropdown dropStyle;
    bool En = false;

    public Text[] LanguageButton=new Text[2];
    
    public InputField randomField;
    public GameObject detailPanel;

    string[] connections = new string[2];
    string[] connectionsCache = new string[2];

    public Dropdown emailAccount;
    public Dropdown emailAddress;

    public GameObject resetAlertPanel;
    public Dropdown ListDropDown; 

    string _path;
    string _pathCache;
    string content;
    string contentCache;
    private void Awake()
    {
        _path = Application.persistentDataPath + "/data.txt";
        _pathCache = Application.persistentDataPath + "/dataCache.txt";
        if (!File.Exists(_path))
        {
            File.Create(_path);
        }
        if (!File.Exists(_pathCache))
        {
            File.Exists(_pathCache);
        }
        content = dataSet.text.ToString();//for reset
        contentCache = dataSet.text.ToString();//for update
    }
    private void Start()
    {
        // poolFullCache= contentCache.Split('\n');
        // poolFull = content.Split('\n');

        dataSetArray = content.Split('[');
        dataSetArrayCache = contentCache.Split('[');

        keyFull = keyLong + keyLong;
        dataFromSet = dataSetArray[0].Split(',');

        webSiteField.text = dataFromSet[0];
        NameField.text = dataFromSet[1];
        keyWord = dataFromSet[2];

        webSiteField.text = GUIUtility.systemCopyBuffer;

        //Accounts:
        connections = dataSetArray[4].Split('\n');
        connectionsCache = dataSetArrayCache[4].Split('\n');

        File.WriteAllText(_path,content);
        updateList();
    }
  
    public void newFile()
    {
        if (File.Exists(_path))
        {
            //read file test
            print(File.ReadAllText(_path));
            debugTxt.text = File.ReadAllText(_path);
            GUIUtility.systemCopyBuffer = _path;
        }
        if (File.Exists(_pathCache))
        {
            //read file test
            print(File.ReadAllText(_pathCache));
            debugTxt.text = File.ReadAllText(_pathCache);
            GUIUtility.systemCopyBuffer = _pathCache;
        }
    }

    string getWebName(string webSite)
    {
        string outWebName="";
        webSite = webSite.ToLower();
        //print(webSite);
       
        if(webSite.Contains("."))
        { 
            string[] keyList = webSite.Split('.');
            outWebName = keyList[1]; 
        }
        else
        {
            outWebName = webSite;
        }
        return outWebName;
    }
    string getName(int i)
    {
        string name;
        name = NameField.text;
        string halfName;
        if (i == 1)
        {
            //firstName
             halfName=name.Split(' ')[1];
        }
        else
        {
            //secondName
            halfName=name.Split(' ')[0];
        }

        halfName.Replace(" ","");
        //print("//" + halfName + "//");
        return halfName;
    }
    string getKeyLevel1()
    {//get base key
        string k1 = getWebName(webSiteField.text.ToString());
        if ((keyWordField.text != null)&&(keyWordField.text!=""))
        {
            keyWord = keyWordField.text.ToString(); 
        }
        //print(keyWord);
        string N1 = getName(1);
        string N2 = getName(0);
        //print("N1,N2=" + N1 + "&" + N2+"^");
        string kA = getKeys(k1, keyWord);
        string kB = getKeys(N1, keyWord);
        string kC = getKeys(N2, keyWord);
        return kA + kB + kC;
    }
    string getKeyLevel2(string keyLevel1, int keyLen)
    {//set up length
      
        string rK = keyLevel1;
        int len = keyLevel1.Length;
        int lenTarget = keyLen;
        //print("1:  "+keyLevel1);
        if (lenTarget > len)
        { 
            int restlen = lenTarget - len;
            //print(rK+"before +=");
            rK += keyLevel1.Substring(0, restlen);
            //print("rest is adding" + rK+"rest:"+restlen);
        }
        else
        {
            rK = keyLevel1.Substring(0, lenTarget);
        }
        return rK;
    }
    string getKeyLevel3(string keyLevel2)
    {
        string k = keyLevel2;
        string c = keySpecial[(Int32.Parse(keyWord))%4].ToString();
          
        if (safeLevel != 6)
        {
            Regex reg2 = new Regex("[a-z]");
            Regex reg = new Regex(@"\d");
            if (!reg.IsMatch(k))
            { 
               // print(k + "无数字");
                k = '0' + k.Substring(1, (k.Length - 1));
            }
            if (reg2.Matches(k).Count > 1)
            {
               // print("len>1="+ reg2.Matches(k).Count);
                char letter = reg2.Match(k).Value[0];
                k = reg2.Replace(k, Char.ToUpper(letter).ToString(), 1);
                k = reg2.Replace(k, c, 1, 1);
            }
            else if (reg2.Matches(k).Count == 1)
            {
               // print("Length="+ reg2.Matches(k).Count);
                char letter = reg2.Match(k).Value[0];
                k = reg2.Replace(k, Char.ToUpper(letter).ToString() + c, 1);
                k = k.Substring(0, k.Length - 1);
            }
            else if (reg2.Matches(k).Count < 1)
            {
               // print("+A");
                char letter = 'A';
                k = letter+ c + k.Substring(1, (k.Length - 2)); 
            }
        }
        else
        {
            string r = "";
            foreach (char x in k)
            {
                r += Convert.ToInt32(x).ToString();
            }
            k = r.Substring(0, safeLevel);
        } 
        //print(k);
        return k;
    }
    string getKeys(string k1,string k2)
    {
        string kn = "";
       
        List<string> listX = new List<string>();
        List<string> listY = new List<string>();
        // print("K1 before bug:[" + k1+"]//");
        foreach (char x in k1)
        {
            int ix;
            if (keyLong.IndexOf(x) != -1)
            {
                ix = keyLong.IndexOf(x);
                listX.Add(ix.ToString());
            }
            
        }
        foreach (char y in k2)
        {
            int iy = keyLong.IndexOf(y);
            listY.Add(iy.ToString());
        }

        if (listX.Count > listY.Count)
        {
           // print("x:" + listX.Count + "Y:" + listY.Count);
            List<string> listCache = listY;
            listY = new List<string>();
            for (int n = 0; n < listX.Count; n++)
            {
                listY.Add(listCache[n % (listCache.Count)]);
            }
            //print("x:" + listX.Count + "Y:" + listY.Count);
            //print("listY:"+listY);
            //print("listCache:"+listCache);
        }
        for (int i = 0; i < listX.Count; i++)
        {
            int x = Int32.Parse(listX[i]);
            int y = Int32.Parse(listY[i]);
            int sum = x + y;
            string m = keyFull[sum].ToString();
            kn += m;
           // print("i=:" + i);
        }
       // print("-kn:" + kn);
        return kn;
    }
    public void GenKeys()
    {  
        finalKey=getKeyLevel3(getKeyLevel2(getKeyLevel1(), safeLevel));
        finalKeyDisplay.text=finalKey;
        GUIUtility.systemCopyBuffer = finalKey;
    }
    public void dropSelection()
    {
        drop = dropDown.GetComponent<Dropdown>();
    
        if (drop.value == 0)
            safeLevel = 12;
        else if (drop.value == 1)
            safeLevel = 14;
        else if (drop.value == 2)
            safeLevel = 10;
        else if (drop.value == 3)
            safeLevel = 6;
    }
    public void copyToHere()
    {
        webSiteField.text = GUIUtility.systemCopyBuffer;
        debugTxt.text = "Data pasted here";
    }
    public void getData()
    {
        NameField.text = PlayerPrefs.GetString("Name");
        keyWordField.text = PlayerPrefs.GetString("Key");
        webSiteField.text = PlayerPrefs.GetString("Web");
        string data = PlayerPrefs.GetString("Name") + ";" + PlayerPrefs.GetString("Key") + ";" + PlayerPrefs.GetString("Web");

        debugTxt.gameObject.SetActive(true);
        debugTxt.text = "Data Got";
        //Invoke("displayText", 0.2f);
    }
    public void saveData()
    {
        if (keyWordField.text != "")
        {
            PlayerPrefs.SetString("Key", keyWordField.text);
        }
        else
        {
            PlayerPrefs.SetString("Key", dataFromSet[2]);
        }
        if (NameField.text != "")
        {
            PlayerPrefs.SetString("Name", NameField.text);
        }
        else
        {
            PlayerPrefs.SetString("Key", dataFromSet[1]);
        }
        if (webSiteField.text != "")
        {
            PlayerPrefs.SetString("Web", webSiteField.text);
        }
        else
        {
            PlayerPrefs.SetString("Key", dataFromSet[0]);
        }
        //string data = PlayerPrefs.GetString("Name") + ";" + PlayerPrefs.GetString("Key") + ";" + PlayerPrefs.GetString("Web");

        debugTxt.gameObject.SetActive(true);
        debugTxt.text = "Data Saved"; 
    }

    void HideConsole()
    {
        debugTxt.gameObject.SetActive(false);
    }
    public void password()
    {
        panalAccount.SetActive(false);
        panalPassword.SetActive(true);
        isMainPage = true;
    }
    public void account()
    {
        panalPassword.SetActive(false);
        panalAccount.SetActive(true);
        isMainPage = false;
    }

    //PAGE 2 : ACCOUNTS

    string saveToCache(string itemsSave,int arrayIndexOfDataSet,int arrayIndexOfItemsLine)
    {
        string contentTemp = "";
        for (int i = 0; i < dataSetArrayCache.Length; i++)
        {
            if (i == arrayIndexOfDataSet)
            {
                string[] itemsArray = dataSetArrayCache[i].Split('\n');
                print("dataSetArray[i]"+i+"/"+dataSetArrayCache[i].Split('\n'));
                string arrayTemp = "";
                for (int n = 0; n < itemsArray.Length; n++)
                {
                    if (n == arrayIndexOfItemsLine)
                    {
                        itemsArray[n] = itemsSave;
                        print(itemsSave);
                    }
                    if((itemsArray[n]!="") && (itemsArray[n] != "\n"))
                    {
                        print("turn to next");
                        arrayTemp += (itemsArray[n] + '\n');
                    }
                    print(itemsArray[n]);
                }
                print("arrayTempN"  + arrayTemp+"/");
                dataSetArrayCache[i] = arrayTemp;
            }
            contentTemp += dataSetArrayCache[i] + "[";
        }
        print("contentTemp:"+contentTemp);
        return contentTemp;
    }
    public void saveItems()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("ItemLists");
        string itemsSave = "";
        for (int i = 0; i < items.Length; i++)
        {
            if (i == items.Length - 1)
            {
                itemsSave += items[i].GetComponentInChildren<Text>().text;
            }
            else
                itemsSave += (items[i].GetComponentInChildren<Text>().text + ",");
        }
        print("itemsSave"+itemsSave);
        if (ListDropDown.value == 0)
        {
            if (!En)
            {
                contentCache = saveToCache(itemsSave, 1, 0);
                
            }
            else
            {
                contentCache = saveToCache(itemsSave, 1, 1);
            }
        }
        else if (ListDropDown.value == 1)
        {
            contentCache = saveToCache(itemsSave, 1, 2);
        }
        else if (ListDropDown.value == 2)
        {
            contentCache = saveToCache(itemsSave, 1, 3);
        }
        else if (ListDropDown.value == 3)
        {
            if (En)
            {
                contentCache = saveToCache(itemsSave, 4, 0);
            }
            else
            {
                contentCache = saveToCache(itemsSave, 4, 1);
            }
        }

        File.WriteAllText(_pathCache,contentCache);
        debugTxt.gameObject.SetActive(true);
    }
    public void ResetOrGetItems(string listsOfPool,string listsOfPoolInCache,bool isTemp)
    { //true if save and get; false if reset;
        GameObject[] items = GameObject.FindGameObjectsWithTag("ItemLists");
        for (int i = 0; i < items.Length; i++)
        {
            DestroyImmediate(items[i]);
        }
        string[] basics;
       
        if (!isTemp)
        {
            basics = listsOfPool.Split(',');
        }
        else
        {
            basics = listsOfPoolInCache.Split(',');
        }
        for (int i = 0; i < basics.Length; i++)
        {
            if (basics[i]!="\r")
            {
                GameObject addedItem = Instantiate(textItem, contentAccount.transform);
                addedItem.SetActive(true);
                addedItem.GetComponentInChildren<Text>().text = basics[i];
            }
        }
        debugTxt.gameObject.SetActive(true);
    }

    void accountsList(bool isTemp)
    {
        string listsOfPool;
        string listsOfPoolInCache;
        if (En)
        {
             listsOfPool = subArray(dataSetArray, 1,'\n')[1];
             listsOfPoolInCache = subArray(dataSetArrayCache, 1, '\n')[1];
        }
        else
        {
             listsOfPool = subArray(dataSetArray, 1, '\n')[0];
             listsOfPoolInCache = subArray(dataSetArrayCache, 1, '\n')[0];
        }  
        ResetOrGetItems(listsOfPool, listsOfPoolInCache, isTemp);
        // debugTxt.text = "Reset accounts";
    }
    void abbreviations(bool isTemp)
    {
        string listsOfPool = subArray(dataSetArray, 1, '\n')[2];
        string listsOfPoolInCache = subArray(dataSetArrayCache, 1, '\n')[2];

        ResetOrGetItems(listsOfPool, listsOfPoolInCache, isTemp);
        debugTxt.text = "Reset abbreviations";
    }
    void numbers(bool isTemp)
    {
        string listsOfPool = subArray(dataSetArray, 1, '\n')[3];
        string listsOfPoolInCache = subArray(dataSetArrayCache, 1, '\n')[3];

        ResetOrGetItems(listsOfPool, listsOfPoolInCache, isTemp);
        debugTxt.text = "Reset numbers";
    }
    void symbol(bool isTemp)
    {
        string listsOfPool;
        string listsOfPoolInCache;
        if (En)
        {
            listsOfPool = connections[0];
            listsOfPoolInCache = connectionsCache[0];
        }
        else
        {
            listsOfPool = connections[1];
            listsOfPoolInCache = connectionsCache[1];
        }
        ResetOrGetItems(listsOfPool, listsOfPoolInCache, isTemp);
        debugTxt.text = "Reset symbols"; 
    }
    public void updateList()
    {    //GET ACCOUNTS
       
        if (ListDropDown.value == 0)
        {
            accountsList(true);
        }
        else if (ListDropDown.value==1)
        {
            abbreviations(true);
        }
        else if (ListDropDown.value == 2)
        {
            numbers(true);
        }
        else if (ListDropDown.value == 3)
        {
            symbol(true);
        }
    }
    public void reset()
    {//page 1 reset
        if (isMainPage)
        {
            webSiteField.text = dataFromSet[0];
            NameField.text = dataFromSet[1];
            keyWordField.text = "";
            keyWord = dataFromSet[2];
            finalKeyDisplay.text = "";

            debugTxt.gameObject.SetActive(true);
            debugTxt.text = "Data Reseted";
            // Invoke("displayText", 0.2f);
        }
        else
        {
            if (ListDropDown.value == 0)
            {
                accountsList(false);
            }
            else if (ListDropDown.value == 1)
            {
                abbreviations(false);
            }
            else if (ListDropDown.value == 2)
            {
                numbers(false);
            }
            else if (ListDropDown.value == 3)
            {
                symbol(false);
            }
            HideResetAlert();
        }
    }

    string RandomSelect(string selectingPool,char splitChar)
    {
        string[] pool = selectingPool.Split(splitChar);
        int r = UnityEngine.Random.Range(0, pool.Length);
        if (pool[r] == "\r"|| pool[r] == "\n" ||pool[r]=="")
        {
            print("loop");
            return ( RandomSelect(selectingPool, splitChar));
        }
        else
        {
            GUIUtility.systemCopyBuffer = pool[r];
            return pool[r];
        }
    }
    string RandomInsert(string selectingPool,string charList,char splitChar)
    {
        string x = "";
        string[] pool = selectingPool.Split(splitChar);
     
        int rI = UnityEngine.Random.Range(0, pool.Length);
        x = pool[rI];

        int rPos = UnityEngine.Random.Range(0, x.Length + 1);
        int rChar = UnityEngine.Random.Range(0, charList.Split(splitChar).Length);

        if (rPos == 0)
        {
            x = charList[rChar].ToString() + x;
        }
        else
        {
            x = x.Substring(0, rPos) + charList[rChar].ToString() + x.Substring(rPos, x.Length - rPos);
        }

        return x;
    }
    string RandomGenHighLevel(string[] selectingPool,string[] connections)
    {
        string selectedItem = "";
        if (En)
        {
           string aPart= RandomSelect(selectingPool[2], ',');
           string connectPart = RandomSelect(connections[0], ',');
           string bPart = RandomSelect(selectingPool[3], ',');
           selectedItem = aPart + connectPart + bPart;
            print("High frequency /En");
        }
        else
        {
            selectedItem = RandomSelect(selectingPool[0], ',');
            selectedItem = RandomInsert(selectedItem, connections[1], ',');
           
            print("High frequency /Ch");
        }
        print("/" + selectedItem + "/");
        return selectedItem; 
    }
    string RandomGenLowLevel(string[] selectingPool,string[] charList,int styles)
    {
        //High Protection level, low connection to person info.
        string selectedItem = "";

        if (En)
        {//no style selections in En of Low level
            string[] pool = selectingPool[0].Split(' ');
            int rS = UnityEngine.Random.Range(0, pool.Length / 2);
            int rF = UnityEngine.Random.Range(0, pool.Length / 2);
            selectedItem = pool[rF * 2] + " " + pool[rS * 2 + 1];
            print("Low frequency /EN");
        }
        else
        {
            string x="";
  
            string[] rPool=new string[5];
            print("Low frequency /Ch :current array:");
            for (int i=0; i < rPool.Length; i++)
            {
                rPool[i] = RandomSelect(selectingPool[i + 1], ',');
                print(i+"---"+rPool[i]); 
            }
           
            if (styles == 0)
            {//common
                for (int i = 2; i < rPool.Length; i++)
                {
                    x += rPool[i];
                }
                print("common ,low frequency");
            }
            else if(styles==1)
            {//long
                for (int i = 0; i < rPool.Length; i++)
                {
                    x += rPool[i];
                }
                print("long ,low frequency");
            }
            else
            {//other
                x = RandomSelect(selectingPool[6],',');
                print("other ,low frequency");
            }
            selectedItem= RandomInsert(x,charList[1],',');
        }
        return selectedItem;
    }
    
    string[] subArray(string[] dataArray,int groupIndex,char splitChar)
    { 
        for (int i=0;i<dataArray[groupIndex].Split(splitChar).Length;i++)
        {
            print(dataArray[groupIndex].Split(splitChar)[i]);
        }
        return dataArray[groupIndex].Split(splitChar);
    }
    public void randomAccount() 
    {
        string selectedItem="";
        string[] poolCut;
        print(dropFreqLevel.value); 
        if (dropFreqLevel.value == 0)
        {
            poolCut = subArray(dataSetArrayCache,1, '\n');
            selectedItem= RandomGenHighLevel(poolCut,connections);
        }
        else if (dropFreqLevel.value == 1)
        {
            poolCut = subArray(dataSetArrayCache,2, '\n');
            selectedItem=RandomGenLowLevel(poolCut,connections,dropStyle.value);
        }
    // GUIUtility.systemCopyBuffer = selectedItem;
        print("Random:"+selectedItem);
        debugTxt.gameObject.SetActive(true);
        debugTxt.text = "Random Account Name";
        randomField.text = selectedItem;
    }
    public void addAccountName() 
    {
        if (randomField.text != "")
        {
            GameObject addedItem = Instantiate(textItem, contentAccount.transform);
            addedItem.SetActive(true);
            addedItem.GetComponentInChildren<Text>().text = randomField.text;

            print("ADD:" + randomField.text);
            debugTxt.gameObject.SetActive(true);
            debugTxt.text = "Account Added";
        }
    }
    public void switchLanguage()
    {
        if (En)
        {
            En = false;
            LanguageButton[1].color = new Color(0.6f, 0.6f, 0.6f, 1);
            LanguageButton[0].color = new Color(0.25f, 0.25f, 0.25f, 1);
            if (dropFreqLevel.value == 1)
            {
                styleDropBox.SetActive(true);
            } 
        }
        else
        {
            En = true;
            LanguageButton[0].color = new Color(0.6f, 0.6f, 0.6f, 1);
            LanguageButton[1].color = new Color(0.25f, 0.25f, 0.25f, 1);
            styleDropBox.SetActive(false);
        }
    }
    public void displayDetail()
    {
        Vector2 localPos;
#if UNITY_IOS || UNITY_ANDROID &&!UNITY_EDITOR
        Touch touchPos = Input.touches[0];
        localPos = touchPos.position;
#endif
#if UNITY_STANDALONE_WIN ||UNITY_EDITOR
        localPos =Input.mousePosition;
#endif
        print(detailPanel.GetComponent<RectTransform>().position);

        detailPanel.GetComponent<RectTransform>().position = localPos;
        detailPanel.SetActive(true);
        Invoke("HideDetail", 1f);
    }
    void HideDetail() 
    {
        detailPanel.SetActive(false);
    }
    public void displayStyleDrop()
    {
        if(!En)
        {
            if (dropFreqLevel.value == 1)
            {
                styleDropBox.SetActive(true);
            }
            else
            {
                styleDropBox.SetActive(false);
            }
        }
      
    }
  
    public void copyEmail()
    {
        string emailLink = emailAccount.captionText.text + "@" + emailAddress.captionText.text;
        GUIUtility.systemCopyBuffer = emailLink;
        debugTxt.gameObject.SetActive(true);
        debugTxt.text = "Email Address Copied";
    }
    public void ResetAlert()
    {
        resetAlertPanel.SetActive(true);
    }
    public void HideResetAlert()
    {
        resetAlertPanel.SetActive(false);
    }
   
}
