using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CheeseManager
{
    public static byte[] m_CurrentCheeseCount = new byte[5];
    private static byte[] m_TotalCheeseCount = new byte[5];

    public static void RegisterCheese()
    {
        uint level = AudioManager.GetCurrentLevel();
        m_CurrentCheeseCount[level]++;
    }

    public static byte GetCurrentLevelCheese() => m_CurrentCheeseCount[AudioManager.GetCurrentLevel()];
}
