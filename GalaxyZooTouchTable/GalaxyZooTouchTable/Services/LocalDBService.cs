using GalaxyZooTouchTable.Models;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GalaxyZooTouchTable.Services
{
    public static class LocalDBService
    {
        private static string HighestRaQuery = "select * from Subjects order by ra desc limit 1";
        private static string HighestDecQuery = "select * from Subjects order by dec desc limit 1";
        private static string LowestRaQuery = "select * from Subjects order by ra asc limit 1";
        private static string LowestDecQuery = "select * from Subjects order by dec asc limit 1";

        public static TableSubject GetLocalSubject(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                string query = $"select * from Subjects where subject_id = {id}";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                TableSubject RetrievedSubject = null;

                while (reader.Read())
                {
                    string image = reader["image"] as string;
                    double ra = (double)reader["ra"];
                    double dec = (double)reader["dec"];
                    RetrievedSubject = new TableSubject(id, image, ra, dec);
                }

                connection.Close();
                return RetrievedSubject;
            }
        }

        public static List<TableSubject> GetQueuedSubjects()
        {
            string query = "select * from Subjects order by classifications_count asc limit 10";
            return GetSubjects(query);
        }

        public static List<TableSubject> GetLocalSubjects(double minRa = 0, double maxRa = 360, double minDec = -90, double maxDec = 90)
        {
            string query = $"select * from Subjects where dec > {minDec} and dec < {maxDec} and ra > {minRa} and ra < {maxRa}";
            return GetSubjects(query);
        }

        public static List<TableSubject> GetSubjects(string query)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                List<TableSubject> Subjects = new List<TableSubject>();

                while (reader.Read())
                {
                    string id = reader["subject_id"] as string;
                    string image = reader["image"] as string;
                    double ra = (double)reader["ra"];
                    double dec = (double)reader["dec"];
                    TableSubject RetrievedSubject = new TableSubject(id, image, ra, dec);
                    Subjects.Add(RetrievedSubject);
                }

                connection.Close();
                return Subjects;
            }
        }

        public static SpacePoint GetPoint(string query)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=C:\\sqlite\\databases\\test_database.db"))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                SpacePoint point = null;

                while (reader.Read())
                {
                    double ra = (double)reader["ra"];
                    double dec = (double)reader["dec"];
                    point = new SpacePoint(ra, dec);
                }

                connection.Close();
                return point;
            }
        }

        public static SpacePoint GetRandomPoint()
        {
            string query = "select * from Subjects order by random() limit 1";
            return GetPoint(query);
        }

        public static SpacePoint FindNextAscendingRa(double RaLowerBounds)
        {
            string query = $"select * from Subjects where ra > {RaLowerBounds} limit 1";
            SpacePoint Center = GetPoint(query);

            if (Center == null)
            {
                Center = GetPoint(LowestRaQuery);
            }

            return Center;
        }

        public static SpacePoint FindNextDescendingRa(double RaUpperBounds)
        {
            string query = $"select * from Subjects where ra < {RaUpperBounds} limit 1";
            SpacePoint Center = GetPoint(query);

            if (Center == null)
            {
                Center = GetPoint(HighestRaQuery);
            }

            return Center;
        }

        public static SpacePoint FindNextAscendingDec(double DecLowerBounds)
        {
            string query = $"select * from Subjects where dec > {DecLowerBounds} limit 1";
            SpacePoint Center = GetPoint(query);

            if (Center == null)
            {
                Center = GetPoint(LowestDecQuery);
            }

            return Center;
        }

        public static SpacePoint FindNextDescendingDec(double DecUpperBounds)
        {
            string query = $"select * from Subjects where dec < {DecUpperBounds} limit 1";
            SpacePoint Center = GetPoint(query);

            if (Center == null)
            {
                Center = GetPoint(HighestDecQuery);
            }

            return Center;
        }
    }
}
