using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class GameUtility 
{
    
    public const float ResolutionDelayTime = 1f;
    //para guardar el highscore entre juegos
    public const string SavePrefKey = "Game_Highscore_Value";

    //nombre del archivo xml para las preguntas
    public const string xmlFileName = "Questions_db.xml";
    public static string xmlFilePath
    {        get { return Application.dataPath + "/" + xmlFileName; }   }   

 }

[System.Serializable]
public class Data
{
    public Question[] Questions = new Question[0];

    //escribe en el archivo xml
    public static void Write(Data data)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Data));
        using(Stream stream = new FileStream(GameUtility.xmlFilePath,FileMode.Create))
        {
            serializer.Serialize(stream, data);
        }
    }

    //carga las preguntas desede el archivo xml -- retorna falso si no hay archivo xml para cargar.
    public static Data Cargar()
    {
        return Cargar(out bool result);
    }
    public static Data Cargar(out bool result)
    {
        if (!File.Exists(GameUtility.xmlFilePath))
        {
            result = false;
            return new Data();
        }

        XmlSerializer deserializer = new XmlSerializer(typeof(Data));
        using(Stream stream = new FileStream(GameUtility.xmlFilePath, FileMode.Open))
        {
            var data = (Data)deserializer.Deserialize(stream);
            result = true;
            return data;
        }        
    }

}
