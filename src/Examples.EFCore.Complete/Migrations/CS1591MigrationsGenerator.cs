using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Examples.EFCore.Complete.Migrations
{
	/// <summary>
	/// Hooks in to the generation of migration scripts to disable CS1591 warnings related to no documentation.
	/// </summary>
	public sealed class CS1591MigrationsGenerator : CSharpMigrationsGenerator
	{
		private const string PragmaWarningDisable = @"#pragma warning disable 1591";
		private const string PragmaWarningRestore = @"#pragma warning restore 1591";
		
		/// <inheritdoc/>
		public CS1591MigrationsGenerator(MigrationsCodeGeneratorDependencies dependencies, CSharpMigrationsGeneratorDependencies csharpDependencies) : base(dependencies, csharpDependencies)
		{
		}

		/// <inheritdoc/>
		public override string GenerateMigration(string migrationNamespace, string migrationName, IReadOnlyList<MigrationOperation> upOperations, IReadOnlyList<MigrationOperation> downOperations)
		{
			return string.Concat(
				PragmaWarningDisable,
				Environment.NewLine,
				base.GenerateMigration(migrationNamespace, migrationName, upOperations, downOperations),
				Environment.NewLine,
				PragmaWarningRestore,
				Environment.NewLine
			);
		}

		/// <inheritdoc/>
		public override string GenerateMetadata(string migrationNamespace, Type contextType, string migrationName, string migrationId, IModel targetModel)
		{
			return string.Concat(
				PragmaWarningDisable,
				Environment.NewLine,
				base.GenerateMetadata(migrationNamespace, contextType, migrationName, migrationId, targetModel),
				Environment.NewLine,
				PragmaWarningRestore,
				Environment.NewLine
			);
		}
	}
}
