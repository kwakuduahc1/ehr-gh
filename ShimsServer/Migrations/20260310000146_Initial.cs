using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ShimsServer.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aspnetroles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    normalizedname = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    concurrencystamp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetroles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnetusers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    phonenumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    fullname = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    username = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    normalizedusername = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    normalizedemail = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    emailconfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    passwordhash = table.Column<string>(type: "character varying(168)", maxLength: 168, nullable: true),
                    securitystamp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    concurrencystamp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    phonenumberconfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    twofactorenabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockoutend = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockoutenabled = table.Column<bool>(type: "boolean", nullable: false),
                    accessfailedcount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetusers", x => x.id);
                    table.UniqueConstraint("ak_aspnetusers_username", x => x.username);
                });

            migrationBuilder.CreateTable(
                name: "diagnoses",
                columns: table => new
                {
                    diagnosisid = table.Column<Guid>(type: "uuid", nullable: false),
                    icd = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    gdrg = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    snomed = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_diagnoses", x => x.diagnosisid);
                });

            migrationBuilder.CreateTable(
                name: "drugs",
                columns: table => new
                {
                    drugsid = table.Column<Guid>(type: "uuid", nullable: false),
                    drug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    tags = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    dateadded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drugs", x => x.drugsid);
                });

            migrationBuilder.CreateTable(
                name: "labgroups",
                columns: table => new
                {
                    labgroupsid = table.Column<Guid>(type: "uuid", nullable: false),
                    labgroup = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    labdescription = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labgroups", x => x.labgroupsid);
                });

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    patientsid = table.Column<Guid>(type: "uuid", nullable: false),
                    hospitalid = table.Column<string>(type: "text", nullable: false),
                    surname = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    othernames = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dateofbirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sex = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    phonenumber = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patients", x => x.patientsid);
                });

            migrationBuilder.CreateTable(
                name: "patientschemes",
                columns: table => new
                {
                    patientschemesid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientsid = table.Column<Guid>(type: "uuid", nullable: false),
                    schemesid = table.Column<Guid>(type: "uuid", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    cardid = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    expirydate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastupdatedate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patientschemes", x => x.patientschemesid);
                });

            migrationBuilder.CreateTable(
                name: "patientsignsandsymptoms",
                columns: table => new
                {
                    patientsignsid = table.Column<Guid>(type: "uuid", nullable: false),
                    signandsymptoms = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patientsignsandsymptoms", x => x.patientsignsid);
                });

            migrationBuilder.CreateTable(
                name: "schemes",
                columns: table => new
                {
                    schemesid = table.Column<Guid>(type: "uuid", nullable: false),
                    schemename = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    coverage = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    maxpayable = table.Column<decimal>(type: "numeric", nullable: false),
                    recovery = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schemes", x => x.schemesid);
                });

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    servicesid = table.Column<Guid>(type: "uuid", nullable: false),
                    service = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    servicegroup = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_services", x => x.servicesid);
                });

            migrationBuilder.CreateTable(
                name: "wards",
                columns: table => new
                {
                    wardsid = table.Column<Guid>(type: "uuid", nullable: false),
                    ward = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    wardtags = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    capacity = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wards", x => x.wardsid);
                });

            migrationBuilder.CreateTable(
                name: "aspnetroleclaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleid = table.Column<Guid>(type: "uuid", nullable: false),
                    claimtype = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    claimvalue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetroleclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_aspnetroleclaims_aspnetroles_roleid",
                        column: x => x.roleid,
                        principalTable: "aspnetroles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetuserclaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    claimtype = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    claimvalue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetuserclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_aspnetuserclaims_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "aspnetusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetuserlogins",
                columns: table => new
                {
                    loginprovider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    providerkey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    providerdisplayname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    userid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetuserlogins", x => new { x.loginprovider, x.providerkey });
                    table.ForeignKey(
                        name: "fk_aspnetuserlogins_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "aspnetusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetuserroles",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    roleid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetuserroles", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "fk_aspnetuserroles_aspnetroles_roleid",
                        column: x => x.roleid,
                        principalTable: "aspnetroles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_aspnetuserroles_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "aspnetusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetusertokens",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    loginprovider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    value = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetusertokens", x => new { x.userid, x.loginprovider, x.name });
                    table.ForeignKey(
                        name: "fk_aspnetusertokens_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "aspnetusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "drugsstocks",
                columns: table => new
                {
                    drugsstockid = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<short>(type: "smallint", nullable: false),
                    trandate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    drugsid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drugsstocks", x => x.drugsstockid);
                    table.ForeignKey(
                        name: "fk_drugsstocks_drugs_drugsid",
                        column: x => x.drugsid,
                        principalTable: "drugs",
                        principalColumn: "drugsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "labparameters",
                columns: table => new
                {
                    labparametersid = table.Column<Guid>(type: "uuid", nullable: false),
                    labparameter = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    order = table.Column<short>(type: "smallint", nullable: false),
                    labgroupsid = table.Column<int>(type: "integer", nullable: false),
                    labgroupsid1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labparameters", x => x.labparametersid);
                    table.ForeignKey(
                        name: "fk_labparameters_labgroups_labgroupsid1",
                        column: x => x.labgroupsid1,
                        principalTable: "labgroups",
                        principalColumn: "labgroupsid");
                });

            migrationBuilder.CreateTable(
                name: "patientattendances",
                columns: table => new
                {
                    patientattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientschemesid = table.Column<Guid>(type: "uuid", nullable: false),
                    visittype = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    dateseen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patientattendances", x => x.patientattendancesid);
                    table.ForeignKey(
                        name: "fk_patientattendances_patientschemes_patientschemesid",
                        column: x => x.patientschemesid,
                        principalTable: "patientschemes",
                        principalColumn: "patientschemesid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "schemedrugs",
                columns: table => new
                {
                    schemdrugsid = table.Column<Guid>(type: "uuid", nullable: false),
                    schemesid = table.Column<Guid>(type: "uuid", nullable: false),
                    drugsid = table.Column<Guid>(type: "uuid", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    dateset = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schemedrugs", x => x.schemdrugsid);
                    table.ForeignKey(
                        name: "fk_schemedrugs_drugs_drugsid",
                        column: x => x.drugsid,
                        principalTable: "drugs",
                        principalColumn: "drugsid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_schemedrugs_schemes_schemesid",
                        column: x => x.schemesid,
                        principalTable: "schemes",
                        principalColumn: "schemesid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "schemelabs",
                columns: table => new
                {
                    schemelabsid = table.Column<Guid>(type: "uuid", nullable: false),
                    labsgroupid = table.Column<Guid>(type: "uuid", nullable: false),
                    schemesid = table.Column<Guid>(type: "uuid", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    dateset = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    labgroupsid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schemelabs", x => x.schemelabsid);
                    table.ForeignKey(
                        name: "fk_schemelabs_labgroups_labgroupsid",
                        column: x => x.labgroupsid,
                        principalTable: "labgroups",
                        principalColumn: "labgroupsid");
                    table.ForeignKey(
                        name: "fk_schemelabs_schemes_schemesid",
                        column: x => x.schemesid,
                        principalTable: "schemes",
                        principalColumn: "schemesid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "schemeservices",
                columns: table => new
                {
                    schemeservicesid = table.Column<Guid>(type: "uuid", nullable: false),
                    schemesid = table.Column<Guid>(type: "uuid", nullable: false),
                    servicesid = table.Column<Guid>(type: "uuid", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    dateset = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schemeservices", x => x.schemeservicesid);
                    table.ForeignKey(
                        name: "fk_schemeservices_schemes_schemesid",
                        column: x => x.schemesid,
                        principalTable: "schemes",
                        principalColumn: "schemesid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_schemeservices_services_servicesid",
                        column: x => x.servicesid,
                        principalTable: "services",
                        principalColumn: "servicesid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patientconsultations",
                columns: table => new
                {
                    patientconsultationid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientsattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    complaints = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    odq = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    avpu = table.Column<byte>(type: "smallint", nullable: false),
                    dateadded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    patientattendancesid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patientconsultations", x => x.patientconsultationid);
                    table.ForeignKey(
                        name: "fk_patientconsultations_patientattendances_patientattendancesid",
                        column: x => x.patientattendancesid,
                        principalTable: "patientattendances",
                        principalColumn: "patientattendancesid");
                });

            migrationBuilder.CreateTable(
                name: "patientdiagnoses",
                columns: table => new
                {
                    patientdiagnosisid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientsattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    diagnosisid = table.Column<Guid>(type: "uuid", nullable: false),
                    secondarydiagnoses = table.Column<Guid[]>(type: "uuid[]", nullable: true),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    dateadded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    patientattendancesid = table.Column<Guid>(type: "uuid", nullable: true),
                    diagnosesdiagnosisid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patientdiagnoses", x => x.patientdiagnosisid);
                    table.ForeignKey(
                        name: "fk_patientdiagnoses_diagnoses_diagnosesdiagnosisid",
                        column: x => x.diagnosesdiagnosisid,
                        principalTable: "diagnoses",
                        principalColumn: "diagnosisid");
                    table.ForeignKey(
                        name: "fk_patientdiagnoses_patientattendances_patientattendancesid",
                        column: x => x.patientattendancesid,
                        principalTable: "patientattendances",
                        principalColumn: "patientattendancesid");
                });

            migrationBuilder.CreateTable(
                name: "patientoutcomes",
                columns: table => new
                {
                    patientsattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    outcome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    outcomedate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patientoutcomes", x => x.patientsattendancesid);
                    table.ForeignKey(
                        name: "fk_patientoutcomes_patientattendances_patientsattendancesid",
                        column: x => x.patientsattendancesid,
                        principalTable: "patientattendances",
                        principalColumn: "patientattendancesid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    paymentsid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientsattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    drugs = table.Column<decimal>(type: "numeric", nullable: false),
                    drugslist = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    labs = table.Column<decimal>(type: "numeric", nullable: false),
                    labslist = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    services = table.Column<decimal>(type: "numeric", nullable: false),
                    serviceslist = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    registration = table.Column<decimal>(type: "numeric", nullable: false),
                    attendance = table.Column<decimal>(type: "numeric", nullable: false),
                    paymentdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    cash = table.Column<decimal>(type: "numeric", nullable: false),
                    mobilemoney = table.Column<decimal>(type: "numeric", nullable: false),
                    patientattendancesid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.paymentsid);
                    table.ForeignKey(
                        name: "fk_payments_patientattendances_patientattendancesid",
                        column: x => x.patientattendancesid,
                        principalTable: "patientattendances",
                        principalColumn: "patientattendancesid");
                });

            migrationBuilder.CreateTable(
                name: "vitals",
                columns: table => new
                {
                    vitalsid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientsattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    dateseen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    temperature = table.Column<double>(type: "double precision", nullable: false),
                    weight = table.Column<double>(type: "double precision", nullable: false),
                    pulse = table.Column<double>(type: "double precision", nullable: false),
                    systol = table.Column<double>(type: "double precision", nullable: false),
                    diastol = table.Column<double>(type: "double precision", nullable: false),
                    respiration = table.Column<double>(type: "double precision", nullable: false),
                    spo2 = table.Column<double>(type: "double precision", nullable: true),
                    complaints = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    patientattendancesid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vitals", x => x.vitalsid);
                    table.ForeignKey(
                        name: "fk_vitals_patientattendances_patientattendancesid",
                        column: x => x.patientattendancesid,
                        principalTable: "patientattendances",
                        principalColumn: "patientattendancesid");
                });

            migrationBuilder.CreateTable(
                name: "wardadmissions",
                columns: table => new
                {
                    wardadmissionsid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientsattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    wardsid = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    dateadmitted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    datedischarged = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    patientattendancesid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wardadmissions", x => x.wardadmissionsid);
                    table.ForeignKey(
                        name: "fk_wardadmissions_patientattendances_patientattendancesid",
                        column: x => x.patientattendancesid,
                        principalTable: "patientattendances",
                        principalColumn: "patientattendancesid");
                    table.ForeignKey(
                        name: "fk_wardadmissions_wards_wardsid",
                        column: x => x.wardsid,
                        principalTable: "wards",
                        principalColumn: "wardsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "drugsrequests",
                columns: table => new
                {
                    drugsrequestsid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientsattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    schemedrugsid = table.Column<Guid>(type: "uuid", nullable: false),
                    frequency = table.Column<byte>(type: "smallint", nullable: false),
                    days = table.Column<byte>(type: "smallint", nullable: false),
                    daterequested = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantityrequested = table.Column<byte>(type: "smallint", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    ispaid = table.Column<bool>(type: "boolean", nullable: false),
                    isdispensed = table.Column<bool>(type: "boolean", nullable: false),
                    drugsid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drugsrequests", x => x.drugsrequestsid);
                    table.ForeignKey(
                        name: "fk_drugsrequests_drugs_drugsid",
                        column: x => x.drugsid,
                        principalTable: "drugs",
                        principalColumn: "drugsid");
                    table.ForeignKey(
                        name: "fk_drugsrequests_patientattendances_patientsattendancesid",
                        column: x => x.patientsattendancesid,
                        principalTable: "patientattendances",
                        principalColumn: "patientattendancesid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_drugsrequests_schemedrugs_schemedrugsid",
                        column: x => x.schemedrugsid,
                        principalTable: "schemedrugs",
                        principalColumn: "schemdrugsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "labrequests",
                columns: table => new
                {
                    labrequestsid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientsattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    schemelabsid = table.Column<Guid>(type: "uuid", nullable: false),
                    daterequested = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    patientattendancesid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labrequests", x => x.labrequestsid);
                    table.ForeignKey(
                        name: "fk_labrequests_patientattendances_patientattendancesid",
                        column: x => x.patientattendancesid,
                        principalTable: "patientattendances",
                        principalColumn: "patientattendancesid");
                    table.ForeignKey(
                        name: "fk_labrequests_schemelabs_schemelabsid",
                        column: x => x.schemelabsid,
                        principalTable: "schemelabs",
                        principalColumn: "schemelabsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "servicerequests",
                columns: table => new
                {
                    servicerequestid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientsattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    schemeservicesid = table.Column<Guid>(type: "uuid", nullable: false),
                    frequency = table.Column<byte>(type: "smallint", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    daterequested = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    patientattendancesid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_servicerequests", x => x.servicerequestid);
                    table.ForeignKey(
                        name: "fk_servicerequests_patientattendances_patientattendancesid",
                        column: x => x.patientattendancesid,
                        principalTable: "patientattendances",
                        principalColumn: "patientattendancesid");
                    table.ForeignKey(
                        name: "fk_servicerequests_schemeservices_schemeservicesid",
                        column: x => x.schemeservicesid,
                        principalTable: "schemeservices",
                        principalColumn: "schemeservicesid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dispensingcalculations",
                columns: table => new
                {
                    drugsrequestsid = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<byte>(type: "smallint", nullable: false),
                    datedone = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    notes = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dispensingcalculations", x => x.drugsrequestsid);
                    table.ForeignKey(
                        name: "fk_dispensingcalculations_drugsrequests_drugsrequestsid",
                        column: x => x.drugsrequestsid,
                        principalTable: "drugsrequests",
                        principalColumn: "drugsrequestsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "labpayments",
                columns: table => new
                {
                    labrequestsid = table.Column<Guid>(type: "uuid", nullable: false),
                    receipt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    datepaid = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    paymenttypesid = table.Column<Guid>(type: "uuid", nullable: true),
                    paymentreceiver = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labpayments", x => x.labrequestsid);
                    table.ForeignKey(
                        name: "fk_labpayments_labrequests_labrequestsid",
                        column: x => x.labrequestsid,
                        principalTable: "labrequests",
                        principalColumn: "labrequestsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "servicepayments",
                columns: table => new
                {
                    servicerequestid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientsattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    paymentmethod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    datepaid = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    receipt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_servicepayments", x => x.servicerequestid);
                    table.ForeignKey(
                        name: "fk_servicepayments_servicerequests_servicerequestid",
                        column: x => x.servicerequestid,
                        principalTable: "servicerequests",
                        principalColumn: "servicerequestid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "drugpayments",
                columns: table => new
                {
                    dispensingcaculationsid = table.Column<Guid>(type: "uuid", nullable: false),
                    receipt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    quantitypaid = table.Column<byte>(type: "smallint", nullable: false),
                    datepaid = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    paymenttypesid = table.Column<Guid>(type: "uuid", nullable: true),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drugpayments", x => x.dispensingcaculationsid);
                    table.ForeignKey(
                        name: "fk_drugpayments_dispensingcalculations_dispensingcaculationsid",
                        column: x => x.dispensingcaculationsid,
                        principalTable: "dispensingcalculations",
                        principalColumn: "drugsrequestsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "labresults",
                columns: table => new
                {
                    labpaymentid = table.Column<Guid>(type: "uuid", nullable: false),
                    labparametersid = table.Column<Guid>(type: "uuid", nullable: false),
                    result = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    datetested = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    servername = table.Column<string>(type: "text", nullable: true),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labresults", x => x.labpaymentid);
                    table.ForeignKey(
                        name: "fk_labresults_labparameters_labparametersid",
                        column: x => x.labparametersid,
                        principalTable: "labparameters",
                        principalColumn: "labparametersid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_labresults_labpayments_labpaymentid",
                        column: x => x.labpaymentid,
                        principalTable: "labpayments",
                        principalColumn: "labrequestsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "servicerenderings",
                columns: table => new
                {
                    servicepaymentid = table.Column<Guid>(type: "uuid", nullable: false),
                    patientsattendancesid = table.Column<Guid>(type: "uuid", nullable: false),
                    dateserved = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    report = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_servicerenderings", x => x.servicepaymentid);
                    table.ForeignKey(
                        name: "fk_servicerenderings_servicepayments_servicepaymentid",
                        column: x => x.servicepaymentid,
                        principalTable: "servicepayments",
                        principalColumn: "servicerequestid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dispensings",
                columns: table => new
                {
                    drugpaymentsid = table.Column<Guid>(type: "uuid", nullable: false),
                    datedispensed = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantitydispensed = table.Column<byte>(type: "smallint", nullable: false),
                    username = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dispensings", x => x.drugpaymentsid);
                    table.ForeignKey(
                        name: "fk_dispensings_drugpayments_drugpaymentsid",
                        column: x => x.drugpaymentsid,
                        principalTable: "drugpayments",
                        principalColumn: "dispensingcaculationsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aspnetroleclaims_roleid",
                table: "aspnetroleclaims",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetroles_normalizedname",
                table: "aspnetroles",
                column: "normalizedname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_aspnetuserclaims_userid",
                table: "aspnetuserclaims",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetuserlogins_userid",
                table: "aspnetuserlogins",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetuserroles_roleid",
                table: "aspnetuserroles",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetusers_normalizedemail",
                table: "aspnetusers",
                column: "normalizedemail");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetusers_normalizedusername",
                table: "aspnetusers",
                column: "normalizedusername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_diagnoses_icd_gdrg_snomed",
                table: "diagnoses",
                columns: new[] { "icd", "gdrg", "snomed" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_drugsrequests_drugsid",
                table: "drugsrequests",
                column: "drugsid");

            migrationBuilder.CreateIndex(
                name: "IX_drugsrequests_patientsattendancesid",
                table: "drugsrequests",
                column: "patientsattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_drugsrequests_schemedrugsid",
                table: "drugsrequests",
                column: "schemedrugsid");

            migrationBuilder.CreateIndex(
                name: "IX_drugsstocks_drugsid",
                table: "drugsstocks",
                column: "drugsid");

            migrationBuilder.CreateIndex(
                name: "IX_labparameters_labgroupsid1",
                table: "labparameters",
                column: "labgroupsid1");

            migrationBuilder.CreateIndex(
                name: "IX_labrequests_patientattendancesid",
                table: "labrequests",
                column: "patientattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_labrequests_schemelabsid",
                table: "labrequests",
                column: "schemelabsid");

            migrationBuilder.CreateIndex(
                name: "IX_labresults_labparametersid",
                table: "labresults",
                column: "labparametersid");

            migrationBuilder.CreateIndex(
                name: "IX_patientattendances_patientschemesid",
                table: "patientattendances",
                column: "patientschemesid");

            migrationBuilder.CreateIndex(
                name: "IX_patientconsultations_patientattendancesid",
                table: "patientconsultations",
                column: "patientattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_patientdiagnoses_diagnosesdiagnosisid",
                table: "patientdiagnoses",
                column: "diagnosesdiagnosisid");

            migrationBuilder.CreateIndex(
                name: "IX_patientdiagnoses_patientattendancesid",
                table: "patientdiagnoses",
                column: "patientattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_payments_patientattendancesid",
                table: "payments",
                column: "patientattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_schemedrugs_drugsid",
                table: "schemedrugs",
                column: "drugsid");

            migrationBuilder.CreateIndex(
                name: "IX_schemedrugs_schemesid",
                table: "schemedrugs",
                column: "schemesid");

            migrationBuilder.CreateIndex(
                name: "IX_schemelabs_labgroupsid",
                table: "schemelabs",
                column: "labgroupsid");

            migrationBuilder.CreateIndex(
                name: "IX_schemelabs_schemesid",
                table: "schemelabs",
                column: "schemesid");

            migrationBuilder.CreateIndex(
                name: "IX_schemeservices_schemesid",
                table: "schemeservices",
                column: "schemesid");

            migrationBuilder.CreateIndex(
                name: "IX_schemeservices_servicesid",
                table: "schemeservices",
                column: "servicesid");

            migrationBuilder.CreateIndex(
                name: "IX_servicerequests_patientattendancesid",
                table: "servicerequests",
                column: "patientattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_servicerequests_schemeservicesid",
                table: "servicerequests",
                column: "schemeservicesid");

            migrationBuilder.CreateIndex(
                name: "IX_vitals_patientattendancesid",
                table: "vitals",
                column: "patientattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_wardadmissions_patientattendancesid",
                table: "wardadmissions",
                column: "patientattendancesid");

            migrationBuilder.CreateIndex(
                name: "IX_wardadmissions_wardsid",
                table: "wardadmissions",
                column: "wardsid");

            migrationBuilder.CreateIndex(
                name: "IX_wards_ward",
                table: "wards",
                column: "ward",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aspnetroleclaims");

            migrationBuilder.DropTable(
                name: "aspnetuserclaims");

            migrationBuilder.DropTable(
                name: "aspnetuserlogins");

            migrationBuilder.DropTable(
                name: "aspnetuserroles");

            migrationBuilder.DropTable(
                name: "aspnetusertokens");

            migrationBuilder.DropTable(
                name: "dispensings");

            migrationBuilder.DropTable(
                name: "drugsstocks");

            migrationBuilder.DropTable(
                name: "labresults");

            migrationBuilder.DropTable(
                name: "patientconsultations");

            migrationBuilder.DropTable(
                name: "patientdiagnoses");

            migrationBuilder.DropTable(
                name: "patientoutcomes");

            migrationBuilder.DropTable(
                name: "patients");

            migrationBuilder.DropTable(
                name: "patientsignsandsymptoms");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "servicerenderings");

            migrationBuilder.DropTable(
                name: "vitals");

            migrationBuilder.DropTable(
                name: "wardadmissions");

            migrationBuilder.DropTable(
                name: "aspnetroles");

            migrationBuilder.DropTable(
                name: "aspnetusers");

            migrationBuilder.DropTable(
                name: "drugpayments");

            migrationBuilder.DropTable(
                name: "labparameters");

            migrationBuilder.DropTable(
                name: "labpayments");

            migrationBuilder.DropTable(
                name: "diagnoses");

            migrationBuilder.DropTable(
                name: "servicepayments");

            migrationBuilder.DropTable(
                name: "wards");

            migrationBuilder.DropTable(
                name: "dispensingcalculations");

            migrationBuilder.DropTable(
                name: "labrequests");

            migrationBuilder.DropTable(
                name: "servicerequests");

            migrationBuilder.DropTable(
                name: "drugsrequests");

            migrationBuilder.DropTable(
                name: "schemelabs");

            migrationBuilder.DropTable(
                name: "schemeservices");

            migrationBuilder.DropTable(
                name: "patientattendances");

            migrationBuilder.DropTable(
                name: "schemedrugs");

            migrationBuilder.DropTable(
                name: "labgroups");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "patientschemes");

            migrationBuilder.DropTable(
                name: "drugs");

            migrationBuilder.DropTable(
                name: "schemes");
        }
    }
}
