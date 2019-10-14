using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace programmersdigest.DataMapper.Migration {
    public class MigrationManager {
        internal const string META_VERSION_TABLE_NAME = "MetaVersion";

        private readonly Database _database;
        private readonly IEnumerable<Type> _migrations;

        public MigrationManager(Database database, MigrationLocator migrationLocator) {
            _database = database;
            _migrations = migrationLocator();
        }

        public async Task<UpdateCheckResult> UpdateRequired() {
            var databaseVersion = await GetDatabaseVersion().ConfigureAwait(false);

            var migrationsToExecute = _migrations.Select(m => new { Version = m.Name.Replace("Migration_", ""), Type = m })
                                                 .OrderByDescending(m => m.Version);
            var latestMigration = migrationsToExecute.FirstOrDefault();
            if (latestMigration == null) {
                throw new InvalidOperationException($"No migrations could be found. There has to be at least one migration creating the MetaVersion table.");
            }

            return new UpdateCheckResult(databaseVersion, latestMigration.Version, string.CompareOrdinal(latestMigration.Version, databaseVersion) > 0);
        }

        public async Task Migrate() {
            var databaseVersion = await GetDatabaseVersion().ConfigureAwait(false);

            var migrationsToExecute = _migrations.Select(m => new { Version = m.Name.Replace("Migration_", ""), Type = m })
                                                 .Where(m => string.CompareOrdinal(m.Version, databaseVersion) > 0);

            foreach (var migrationToExecute in migrationsToExecute) {
                var migration = Activator.CreateInstance(migrationToExecute.Type) as IMigration;
                migration.Execute(_database);

                // Insert version number.
                await _database.Insert(new DatabaseVersionEntry {
                    Version = migrationToExecute.Version,
                    CreationDate = DateTime.Now
                }).ConfigureAwait(false);
            }
        }

        private async Task<string> GetDatabaseVersion() {
            try {
                var versions = await _database.Select<DatabaseVersionEntry>($"SELECT * FROM {META_VERSION_TABLE_NAME} ORDER BY Version DESC LIMIT 1").ConfigureAwait(false);
                return versions.FirstOrDefault()?.Version;
            }
            catch (Exception) {
                return "00000";
            }
        }
    }
}
