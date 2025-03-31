using FluentMigrator;

namespace MyRecipeBook.Infrastructure.DataAccess.Migrations.Versions
{
    [Migration(DatabaseVersion.TABLE_REFRESH_TOKENS, "Create table to save refresh tokens.")]
    public class Version0000004 : VersionBase
    {
        public override void Up()
        {
            CreateTable("RefreshTokens")
                .WithColumn("Value").AsString().NotNullable()
                .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("FK_RefreshTokens_User_Id", "Users", "Id");
        }
    }
}
