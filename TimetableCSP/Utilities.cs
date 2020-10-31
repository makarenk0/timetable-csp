using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using TimetableCSP;

namespace TimetableGeneticGeneration
{
    class Utilities
    {
        public enum LessonType { Lecture, Practice };


        public static int ChooseRandomly(int from, int to)
        {
            using (RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider())
            {
                byte[] rno = new byte[5];
                rg.GetBytes(rno);

                int n = from + (Math.Abs(BitConverter.ToInt32(rno, 0)) % to);
                return n;
            }
        }

        public static T GetAsObjectJSON<T>(JsonElement element, String property)
        {
            return JsonSerializer.Deserialize<T>(element.GetProperty(property).GetRawText());
        }

        public static Dictionary<String, String> GetAsObjectJSON(JsonElement element, String property, String ofProperties)
        {
            string[] props = GetAsObjectJSON<string[]>(element, ofProperties);
            JsonElement from = element.GetProperty(property);
            var result = new Dictionary<String, String>();

            foreach (var prop in props)
            {
                result[prop] = JsonSerializer.Deserialize<String>(from.GetProperty(prop).GetRawText());
            }

            return result;
        }

        public static List<Variable> Hard1Sequence()  // 10378 steps, 5183 back steps using pure backtracking
        {
            List<Variable> hardSequence = new List<Variable>();
            hardSequence.Add(new Variable(LessonType.Practice, "Databases", "SE1"));
            hardSequence.Add(new Variable(LessonType.Practice, "Information retrieval", "SE1"));
            hardSequence.Add(new Variable(LessonType.Practice, "Information retrieval", "CS1"));
            hardSequence.Add(new Variable(LessonType.Practice, "Information retrieval", "SE2"));
            hardSequence.Add(new Variable(LessonType.Lecture, "Databases", "CS1, CS2"));
            hardSequence.Add(new Variable(LessonType.Practice, "Information retrieval", "CS2"));
            hardSequence.Add(new Variable(LessonType.Lecture, "Information retrieval", "CS1, CS2"));
            hardSequence.Add(new Variable(LessonType.Practice, "Databases", "CS1"));
            hardSequence.Add(new Variable(LessonType.Lecture, "Databases", "SE1, SE2"));
            hardSequence.Add(new Variable(LessonType.Lecture, "Information retrieval", "SE1, SE2"));
            hardSequence.Add(new Variable(LessonType.Practice, "Databases", "CS2"));
            hardSequence.Add(new Variable(LessonType.Practice, "Databases", "SE2"));
            return hardSequence;
        }

        public static List<Variable> Hard2Sequence()  // 10378 steps, 5183 back steps using pure backtracking
        {
            List<Variable> hardSequence = new List<Variable>();
            hardSequence.Add(new Variable(LessonType.Lecture, "Information retrieval", "SE1, SE2"));
            hardSequence.Add(new Variable(LessonType.Lecture, "Databases", "CS1, CS2"));
            hardSequence.Add(new Variable(LessonType.Practice, "Information retrieval", "CS2"));
            hardSequence.Add(new Variable(LessonType.Practice, "Information retrieval", "SE1"));
            hardSequence.Add(new Variable(LessonType.Practice, "Databases", "SE2"));
            hardSequence.Add(new Variable(LessonType.Practice, "Databases", "CS2"));
            hardSequence.Add(new Variable(LessonType.Lecture, "Databases", "SE1, SE2"));
            hardSequence.Add(new Variable(LessonType.Lecture, "Information retrieval", "CS1, CS2"));
            hardSequence.Add(new Variable(LessonType.Practice, "Information retrieval", "SE2"));
            hardSequence.Add(new Variable(LessonType.Practice, "Databases", "SE1"));  
            hardSequence.Add(new Variable(LessonType.Practice, "Information retrieval", "CS1"));
            hardSequence.Add(new Variable(LessonType.Practice, "Databases", "CS1"));
            return hardSequence;
        }


    }
}
