﻿using System.Globalization;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class Util
{
    public static void PrintSection(string section)
    {
        section = "=[ " + section + " ]";
        while (section.Length < 62)
        {
            section = "-" + section;
        }
        LogManager.Log(section);
    }

    public static char[] ILLEGAL_CHARACTERS =
    {
        '/',
        '\n',
        '\r',
        '\t',
        '\0',
        '\f',
        '`',
        '?',
        '*',
        '\\',
        '<',
        '>',
        '|',
        '\"',
        '{',
        '}',
        '(',
        ')'
    };

    public static int HexStringToInt(string hex)
    {
        return int.Parse(hex, NumberStyles.HexNumber);
    }
}
