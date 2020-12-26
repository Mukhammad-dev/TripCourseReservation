using System.IO;

namespace TripCourseReservation
{
    public static class RepoCreation
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void createDataFile(string[] pathes)
        {
            log.Info(pathes);
            foreach(string path in pathes)
            {
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (!File.Exists(path))
                {
                    File.Create(path);
                }
            }
        }        
    }
}
