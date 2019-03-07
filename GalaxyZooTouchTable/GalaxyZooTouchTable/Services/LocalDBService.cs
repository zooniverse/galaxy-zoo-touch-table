﻿using GalaxyZooTouchTable.Models;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GalaxyZooTouchTable.Services
{
    public static class LocalDBService
    {
        public static List<TableSubject> GetLocalSubjects(double minRa = 0, double maxRa = 360, double minDec = -90, double maxDec = 90)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                string query = $"select * from Subject where dec > {minDec} and dec < {maxDec} and ra > {minRa} and ra < {maxRa}";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                List<TableSubject> SubjectsWithinBounds = new List<TableSubject>();

                while (reader.Read())
                {
                    string id = reader["subject_id"] as string;
                    string image = reader["image"] as string;
                    double ra = (double)reader["ra"];
                    double dec = (double)reader["dec"];
                    TableSubject subjectFromDatabase = new TableSubject(id, image, ra, dec);
                    SubjectsWithinBounds.Add(subjectFromDatabase);
                }

                connection.Close();
                return SubjectsWithinBounds;
            }
        }
    }
}
