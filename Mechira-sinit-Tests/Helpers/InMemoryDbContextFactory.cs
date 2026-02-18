using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using WebApplication1.Data;

namespace WebApplication1.Tests.Helpers
{
    /// <summary>
    /// Factory for creating InMemory DbContext instances for integration tests.
    /// Each test gets an isolated database context to ensure test isolation.
    /// </summary>
    public static class InMemoryDbContextFactory
    {
        /// <summary>
        /// Creates a new InMemory StoreContext with a unique database name.
        /// </summary>
        /// <returns>A configured StoreContext instance using InMemory provider.</returns>
        public static StoreContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new StoreContext(options);
        }

        /// <summary>
        /// Creates a new InMemory StoreContext with seed data.
        /// The context is initialized with the provided seed action.
        /// </summary>
        /// <param name="seedAction">Action to seed initial data into the context.</param>
        /// <returns>A configured and seeded StoreContext instance.</returns>
        public static StoreContext CreateInMemoryContextWithSeed(Action<StoreContext> seedAction)
        {
            var context = CreateInMemoryContext();
            seedAction(context);
            context.SaveChanges();
            return context;
        }
    }
}
