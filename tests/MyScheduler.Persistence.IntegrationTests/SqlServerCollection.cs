namespace MyScheduler.Persistence.IntegrationTests;

[CollectionDefinition(nameof(SqlServerCollection))]
public sealed class SqlServerCollection : ICollectionFixture<SqlServerContainerFixture>;
