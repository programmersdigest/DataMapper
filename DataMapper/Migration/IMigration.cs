namespace programmersdigest.DataMapper.Migration {
    public interface IMigration {
        string Description { get; }

        void Execute(Database database);
    }
}
