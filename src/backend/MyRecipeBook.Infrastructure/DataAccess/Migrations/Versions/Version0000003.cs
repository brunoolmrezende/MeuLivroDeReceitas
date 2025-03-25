using FluentMigrator;

namespace MyRecipeBook.Infrastructure.DataAccess.Migrations.Versions
{
    [Migration(DatabaseVersion.IMAGES_FOR_RECIPES, "Add collum on recipe table to save images.")]
    public class Version0000003 : VersionBase
    {
        public override void Up()
        {
            Alter.Table("Recipes").AddColumn("ImageIdentifier").AsString().Nullable();
        }
    }
}
