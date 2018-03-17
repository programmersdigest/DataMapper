using programmersdigest.DataMapper.Attributes;
using System;

namespace programmersdigest.DataMapper.Migration {
    [Name(MigrationManager.META_VERSION_TABLE_NAME)]
    public class DatabaseVersionEntry {
        public int Id { get; set; }
        public string Version { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
