using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Examples.EFCore.Complete.Migrations
{
	/// <summary>
	/// Hooks in to the generation of migration scripts to replace spaces with tabs.
	/// </summary>
	public sealed class TabMigrationsGenerator : CSharpMigrationsGenerator
	{
		/// <inheritdoc/>
		public TabMigrationsGenerator(MigrationsCodeGeneratorDependencies dependencies, CSharpMigrationsGeneratorDependencies csharpDependencies) : base(dependencies, csharpDependencies)
		{
		}

		/// <inheritdoc/>
		public override string GenerateMigration(string migrationNamespace, string migrationName, IReadOnlyList<MigrationOperation> upOperations, IReadOnlyList<MigrationOperation> downOperations)
		{
			return base.GenerateMigration(migrationNamespace, migrationName, upOperations, downOperations)
				.Replace("    ", "\t");
		}

		/// <inheritdoc/>
		public override string GenerateMetadata(string migrationNamespace, Type contextType, string migrationName, string migrationId, IModel targetModel)
		{
			return base.GenerateMetadata(migrationNamespace, contextType, migrationName, migrationId, targetModel)
				.Replace("    ", "\t");
		}
	}
}
