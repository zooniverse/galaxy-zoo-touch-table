using GalaxyZooTouchTable.Lib;
using GalaxyZooTouchTable.Models;
using PanoptesNetClient.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GalaxyZooTouchTable.Services
{
    public class LocalDBService : ILocalDBService
    {
        readonly int RETIREMENT_LIMIT = 25;
        readonly string QueuedSubjectsQuery = "select * from Subjects order by classifications_count asc, random() limit 10";
        readonly string QueuedSubjectQuery = "select * from Subjects order by classifications_count asc, random() limit 1";
        string HighestRaQuery(int limit) { return $"select * from Subjects where classifications_count < {limit} order by ra desc limit 1"; }
        string HighestDecQuery(int limit) { return $"select * from Subjects where classifications_count < {limit} order by dec desc limit 1"; }
        string LowestRaQuery(int limit) { return $"select * from Subjects where classifications_count < {limit} order by ra asc limit 1"; }
        string LowestDecQuery(int limit) { return $"select * from Subjects order by dec asc limit 1"; }
        string RandomSubjectQuery(int limit) { return $"select * from Subjects where classifications_count < {limit} order by random() limit 1"; }
        string SubjectByIdQuery(string id) { return $"select * from Subjects where subject_id = {id}"; }
        string NextAscendingRaQuery(double bounds, int limit) { return $"select * from Subjects where ra > {bounds} and classifications_count < {limit} order by ra asc limit 1"; }
        string NextDescendingRaQuery(double bounds, int limit) { return $"select * from Subjects where ra < {bounds} and classifications_count < {limit} order by ra desc limit 1"; }
        string NextAscendingDecQuery(double bounds, int limit) { return $"select * from Subjects where dec > {bounds} and classifications_count < {limit} order by dec asc limit 1"; }
        string NextDescendingDecQuery(double bounds, int limit) { return $"select * from Subjects where dec < {bounds} and classifications_count < {limit} order by dec desc limit 1"; }
        string GetCurrentClassificationCount(string id) { return $"select * from Subjects where subject_id = {id}"; }
        string UpdateSubjectCounts(string id, ClassificationCounts counts) { return $"update Subjects set classifications_count = {counts.Total}, smooth = {counts.Smooth}, features = {counts.Features}, star = {counts.Star} where subject_id = {id}"; } 

        IGraphQLService _graphQLService { get; set; }

        public LocalDBService(IGraphQLService graphQLService)
        {
            _graphQLService = graphQLService;
        }

        string SubjectsWithinBoundsQuery(SpaceNavigation location)
        {
            return $"select * from Subjects where dec > {location.MinDec} and dec < {location.MaxDec} and ra > {location.MinRa} and ra < {location.MaxRa}";
        }

        public TableSubject GetLocalSubject(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                TableSubject RetrievedSubject = null;
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(SubjectByIdQuery(id), connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string filename = reader["filename"] as string;
                        string image = CheckLocalPath(filename) ?? reader["image"] as string;
                        double ra = (double)reader["ra"];
                        double dec = (double)reader["dec"];
                        int classificationCount = Convert.ToInt32(reader["classifications_count"]);
                        RetrievedSubject = new TableSubject(id, image, ra, dec, classificationCount);
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
            return GetSubjects(QueuedSubjectsQuery);
        }

        public List<TableSubject> GetLocalSubjects(SpaceNavigation currentLocation)
        {
            string query = SubjectsWithinBoundsQuery(currentLocation);
            return GetSubjects(query, currentLocation);
        }

        public List<TableSubject> GetSubjects(string query, SpaceNavigation currentLocation = null)
        {
            List<string> idsToUpdate = new List<string>();
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
                        idsToUpdate.Add(id);
                        string filename = reader["filename"] as string;
                        string image = CheckLocalPath(filename) ?? reader["image"] as string;
                        double ra = (double)reader["ra"];
                        double dec = (double)reader["dec"];
                        int classificationCount = Convert.ToInt32(reader["classifications_count"]);
                        TableSubject RetrievedSubject = new TableSubject(id, image, ra, dec, classificationCount, currentLocation);
                        Subjects.Add(RetrievedSubject);
                    }

                    connection.Close();
                } catch (SQLiteException exception)
                {
                    string ErrorMessage = $"Error Connecting to Database. Error: {exception.Message}";
                    Messenger.Default.Send(ErrorMessage, "DatabaseError");
                }
                UpdateDBFromIds(idsToUpdate);
                return Subjects.Any(subject => !subject.IsRetired) ? Subjects : new List<TableSubject>();
            }
        }

        async void UpdateDBFromIds(List<string> ids)
        {
            foreach (string id in ids) await UpdateDBFromGraphQL(id);
        }

        public SpacePoint GetPoint(string query)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                SpacePoint point = null;
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
            return GetPoint(RandomSubjectQuery(RETIREMENT_LIMIT)) ?? new SpacePoint(0,0);
        }

        public SpacePoint FindNextAscendingRa(double bounds)
        {
            return GetPoint(NextAscendingRaQuery(bounds, RETIREMENT_LIMIT)) ?? GetPoint(LowestRaQuery(RETIREMENT_LIMIT)) ?? GetPoint(QueuedSubjectQuery);
        }

        public SpacePoint FindNextDescendingRa(double bounds)
        {
            return GetPoint(NextDescendingRaQuery(bounds, RETIREMENT_LIMIT)) ?? GetPoint(HighestRaQuery(RETIREMENT_LIMIT)) ?? GetPoint(QueuedSubjectQuery);
        }

        public SpacePoint FindNextAscendingDec(double bounds)
        {
            return GetPoint(NextAscendingDecQuery(bounds, RETIREMENT_LIMIT)) ?? GetPoint(LowestDecQuery(RETIREMENT_LIMIT)) ?? GetPoint(QueuedSubjectQuery);
        }

        public SpacePoint FindNextDescendingDec(double bounds)
        {
            return GetPoint(NextDescendingDecQuery(bounds, RETIREMENT_LIMIT)) ?? GetPoint(HighestDecQuery(RETIREMENT_LIMIT)) ?? GetPoint(QueuedSubjectQuery);
        }

        public ClassificationCounts IncrementClassificationCount(Classification classification)
        {
            ClassificationCounts counts = GetCountsBySubjectId(classification.Links.Subjects[0]);
            return IncrementAndUpdateCounts(classification, counts);
        }

        public ClassificationCounts IncrementAndUpdateCounts(Classification classification, ClassificationCounts counts)
        {
            switch (classification.Annotations[0].Value)
            {
                case 0:
                    counts.Smooth += 1;
                    break;
                case 1:
                    counts.Features += 1;
                    break;
                case 2:
                    counts.Star += 1;
                    break;
            }
            counts.Total += 1;
            UpdateSubject(classification.Links.Subjects[0], counts);
            return counts;
        }

        public ClassificationCounts GetCountsBySubjectId(string id)
        {
            string query = GetCurrentClassificationCount(id);
            ClassificationCounts counts = new ClassificationCounts();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int total = Convert.ToInt32(reader["classifications_count"]);
                        int smooth = Convert.ToInt32(reader["smooth"]);
                        int features = Convert.ToInt32(reader["features"]);
                        int star = Convert.ToInt32(reader["star"]);
                        counts = new ClassificationCounts(total, smooth, features, star);
                    }
                }
                catch (SQLiteException exception)
                {
                    string ErrorMessage = $"Error Connecting to Database. Error: {exception.Message}";
                    Messenger.Default.Send(ErrorMessage, "DatabaseError");
                }
                return counts;
            }
        }

        public async Task UpdateDBFromGraphQL(string id)
        {
            ClassificationCounts counts = await _graphQLService.GetReductionAsync(id);

            if (counts.Total > 0)
                UpdateSubject(id, counts);
        }

        public void UpdateSubject(string id, ClassificationCounts counts)
        {
            string query = UpdateSubjectCounts(id, counts);
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={App.DatabasePath}"))
            {
                try
                {
                    connection.Open();
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException exception)
                {
                    string ErrorMessage = $"Error Connecting to Database. Error: {exception.Message}";
                    Messenger.Default.Send(ErrorMessage, "DatabaseError");
                }
            }
        }

        string CheckLocalPath(string filename)
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string subFolder = filename.Substring(0, 4);
            string path = Path.Combine(folderPath, "Subjects", subFolder, $"{filename}.png");
            return File.Exists(path) ? path : null;
        }
    }
}
