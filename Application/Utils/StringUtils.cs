﻿using Domain.Enums;
using System.ComponentModel;

namespace Application.Utils
{
    public static class StringUtils
    {
        public static string Hash(this string input) => BCrypt.Net.BCrypt.HashPassword(input);

        public static bool CheckPassword(this string password, string hashPassword) => BCrypt.Net.BCrypt.Verify(password, hashPassword);

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool ThrowErrorIfNotValidEnum(this string myenum, Type type, string message = "Invalid Enum")
        {
            bool check = false; // checking if enum is valid
            foreach (string @enum in Enum.GetNames(type))
            {
                if (myenum.Equals(@enum, StringComparison.OrdinalIgnoreCase)) { check = true; break; }
            }

            if (!check)
            {
                //throw error with all the enum format 
                throw new InvalidEnumArgumentException($"{message}.\nTry using {string.Join(", ",Enum.GetNames(type))}");

            }
            //return true if needed
            return true;
        }
    }
}
