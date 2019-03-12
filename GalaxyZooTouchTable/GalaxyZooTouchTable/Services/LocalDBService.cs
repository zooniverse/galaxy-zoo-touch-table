using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using System.Collections.Generic;
using System.Data.SQLite;

namespace GalaxyZooTouchTable.Services
{
    public class LocalDBService : ILocalDBService
    {
        string HighestRaQuery = "select * from Subjects order by ra desc limit 1";
        string HighestDecQuery = "select * from Subjects order by dec desc limit 1";
        string LowestRaQuery = "select * from Subjects order by ra asc limit 1";
        string LowestDecQuery = "select * from Subjects order by dec asc limit 1";

        public TableSubject GetLocalSubject(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                TableSubject RetrievedSubject = null;
                try
                {
                    connection.Open();
                    string query = $"select * from Subjects where subject_id = {id}";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string image = reader["image"] as string;
                        double ra = (double)reader["ra"];
                        double dec = (double)reader["dec"];
                        RetrievedSubject = new TableSubject(id, image, ra, dec);
                    }

                    connection.Close();
                } catch (SQLiteException exception)
                {
                    string ErrorMessage = $"Error Connecting to Database. Error: {exception.Message}";
                    Messenger.Default.Send(ErrorMessage, "DatabaseError");
                }
                return RetrievedSubject;
            }
        }

        public List<TableSubject> GetQueuedSubjects()
        {
            string query = "select * from Subjects order by classifications_count asc limit 10";
            return GetSubjects(query);
        }

        public List<TableSubject> GetLocalSubjects(double minRa = 0, double maxRa = 360, double minDec = -90, double maxDec = 90)
        {
            string query = $"select * from Subjects where dec > {minDec} and dec < {maxDec} and ra > {minRa} and ra < {maxRa}";
            return GetSubjects(query);
        }

        public List<TableSubject> GetSubjects(string query)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                List<TableSubject> Subjects = new List<TableSubject>();
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

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
                } catch (SQLiteException exception)
                {
                    string ErrorMessage = $"Error Connecting to Database. Error: {exception.Message}";
                    Messenger.Default.Send(ErrorMessage, "DatabaseError");
                }
                return Subjects;
            }
        }

        public SpacePoint GetPoint(string query)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                SpacePoint point = new SpacePoint();
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        double ra = (double)reader["ra"];
                        double dec = (double)reader["dec"];
                        point = new SpacePoint(ra, dec);
                    }

                    connection.Close();
                } catch (SQLiteException exception)
                {
                    string ErrorMessage = $"Error Connecting to Database. Error: {exception.Message}";
                    Messenger.Default.Send(ErrorMessage, "DatabaseError");
                }
                return point;
            }
        }

        public SpacePoint GetRandomPoint()
        {
            string query = "select * from Subjects order by random() limit 1";
            return GetPoint(query);
        }

        public SpacePoint FindNextAscendingRa(double RaLowerBounds)
        {
            string query = $"select * from Subjects where ra > {RaLowerBounds} order by ra asc limit 1";
            SpacePoint Center = GetPoint(query);

            if (Center == null)
            {
                Center = GetPoint(LowestRaQuery);
            }

            return Center;
        }

        public SpacePoint FindNextDescendingRa(double RaUpperBounds)
        {
            string query = $"select * from Subjects where ra < {RaUpperBounds} order by ra desc limit 1";
            SpacePoint Center = GetPoint(query);

            if (Center == null)
            {
                Center = GetPoint(HighestRaQuery);
            }

            return Center;
        }

        public SpacePoint FindNextAscendingDec(double DecLowerBounds)
        {
            string query = $"select * from Subjects where dec > {DecLowerBounds} order by dec asc limit 1";
            SpacePoint Center = GetPoint(query);

            if (Center == null)
            {
                Center = GetPoint(LowestDecQuery);
            }

            return Center;
        }

        public SpacePoint FindNextDescendingDec(double DecUpperBounds)
        {
            string query = $"select * from Subjects where dec < {DecUpperBounds} order by dec desc limit 1";
            SpacePoint Center = GetPoint(query);

            if (Center == null)
            {
                Center = GetPoint(HighestDecQuery);
            }

            return Center;
        }
    }
}
