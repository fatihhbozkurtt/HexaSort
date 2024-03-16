using System.Collections.Generic;


[System.Serializable]
public class RollerClass
{
    public List<RopeClass> RollerRopeInfoList = new List<RopeClass>();

    [System.Serializable]
    public class RopeClass
    {
        public ColorInfo.ColorEnum ColorEnum;
        public int amount;

    }
}



