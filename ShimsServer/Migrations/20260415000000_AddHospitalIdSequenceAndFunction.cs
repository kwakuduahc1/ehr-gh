using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class AddHospitalIdSequenceAndFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE SEQUENCE hospital_id_sequence START 1 INCREMENT 1;
                """);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION generate_hospital_id()
                RETURNS VARCHAR AS $$
                DECLARE
                    v_sequence BIGINT;
                BEGIN
                    v_sequence := nextval('hospital_id_sequence');
                    RETURN 'MYHOSP-' || TO_CHAR(NOW(), 'YYYY-MM-') || LPAD(v_sequence::text, 6, '0');
                END;
                $$ LANGUAGE plpgsql;
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE "patientschemes"
                ADD CONSTRAINT df_patientschemes_hospitalid
                DEFAULT generate_hospital_id() FOR "hospitalid";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "patientschemes"
                DROP CONSTRAINT df_patientschemes_hospitalid;
                """);

            migrationBuilder.Sql(
                """
                DROP FUNCTION IF EXISTS generate_hospital_id();
                """);

            migrationBuilder.Sql(
                """
                DROP SEQUENCE IF EXISTS hospital_id_sequence;
                """);
        }
    }
}
