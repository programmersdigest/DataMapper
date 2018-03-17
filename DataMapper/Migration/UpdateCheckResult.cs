namespace programmersdigest.DataMapper.Migration {
    public class UpdateCheckResult {
        public bool UpdateRequired { get; }
        public string CurrentVersion { get; }
        public string LatestVersion { get; }

        public UpdateCheckResult(string currentVersion, string latestVersion, bool updateRequired) {
            CurrentVersion = currentVersion;
            LatestVersion = latestVersion;
            UpdateRequired = updateRequired;
        }
    }
}
