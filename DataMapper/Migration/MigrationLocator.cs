using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace programmersdigest.DataMapper.Migration {
    public delegate IEnumerable<Type> MigrationLocator();

    public static class DefaultMigrationLocator {
        public static IEnumerable<Type> Implementation() {
            return Assembly.GetEntryAssembly().GetTypes()
                           .Where(t => typeof(IMigration).IsAssignableFrom(t) && !t.IsInterface);
        }
    }
}
