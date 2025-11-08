using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    MaxCoursePosts = table.Column<int>(type: "integer", nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    ExpiredTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionType = table.Column<int>(type: "integer", nullable: false),
                    EntityName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    Details = table.Column<string>(type: "text", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CenterProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CenterName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OwnerName = table.Column<string>(type: "text", nullable: true),
                    LicenseNumber = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LicenseIssuedBy = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    ContactEmail = table.Column<string>(type: "text", nullable: true),
                    ContactPhone = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    VerificationRequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationCompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationNotes = table.Column<string>(type: "text", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    LicenseDocumentPath = table.Column<string>(type: "text", nullable: true),
                    BusinessRegistrationPath = table.Column<string>(type: "text", nullable: true),
                    TaxCodeDocumentPath = table.Column<string>(type: "text", nullable: true),
                    OtherDocumentsPath = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CenterProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportType = table.Column<int>(type: "integer", nullable: false),
                    ParametersJson = table.Column<string>(type: "text", nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Format = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    RequestedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneratedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedReports_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParentProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    PhoneSecondary = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParentProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CenterSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CenterProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionPackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "text", nullable: true),
                    AutoRenewalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AutoRenewalEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CenterSubscriptions_CenterProfiles_CenterProfileId",
                        column: x => x.CenterProfileId,
                        principalTable: "CenterProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CenterSubscriptions_SubscriptionPackages_SubscriptionPackag~",
                        column: x => x.SubscriptionPackageId,
                        principalTable: "SubscriptionPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CenterVerificationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CenterProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    InspectorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InspectorNotes = table.Column<string>(type: "text", nullable: true),
                    VerificationPhotos = table.Column<string>(type: "text", nullable: true),
                    DocumentChecklist = table.Column<string>(type: "text", nullable: true),
                    IsLocationVerified = table.Column<bool>(type: "boolean", nullable: false),
                    IsDocumentsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    IsLicenseValid = table.Column<bool>(type: "boolean", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    AdminDecisionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AdminDecision = table.Column<int>(type: "integer", nullable: false),
                    AdminNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterVerificationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CenterVerificationRequests_CenterProfiles_CenterProfileId",
                        column: x => x.CenterProfileId,
                        principalTable: "CenterProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CenterVerificationRequests_Users_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CenterVerificationRequests_Users_InspectorId",
                        column: x => x.InspectorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    YearOfExperience = table.Column<int>(type: "integer", nullable: false),
                    Qualifications = table.Column<string>(type: "text", nullable: false),
                    LicenseNumber = table.Column<string>(type: "text", nullable: false),
                    Subjects = table.Column<string>(type: "text", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    TeachingAtSchool = table.Column<string>(type: "text", nullable: true),
                    TeachAtClasses = table.Column<string>(type: "text", nullable: true),
                    CenterProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    VerificationStatus = table.Column<int>(type: "integer", nullable: false),
                    VerificationRequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationCompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerificationNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherProfiles_CenterProfiles_CenterProfileId",
                        column: x => x.CenterProfileId,
                        principalTable: "CenterProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeacherProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolName = table.Column<string>(type: "text", nullable: false),
                    SchoolYear = table.Column<string>(type: "text", nullable: false),
                    GradeLevel = table.Column<int>(type: "integer", nullable: false),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ParentProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentProfiles_ParentProfiles_ParentProfileId",
                        column: x => x.ParentProfileId,
                        principalTable: "ParentProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RoomOrLink = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    TeacherProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Subject = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Semester = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TeachingMethod = table.Column<int>(type: "integer", nullable: false),
                    TuitionFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    GradeLevel = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TeacherProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    CenterProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_CenterProfiles_CenterProfileId",
                        column: x => x.CenterProfileId,
                        principalTable: "CenterProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Courses_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Syllabuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SyllabusName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    GradeLevel = table.Column<int>(type: "integer", maxLength: 64, nullable: false),
                    AssessmentMethod = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CourseMaterial = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Syllabuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Syllabuses_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Syllabuses_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherVerificationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    QualificationCertificatePath = table.Column<string>(type: "text", nullable: true),
                    EmploymentContractPath = table.Column<string>(type: "text", nullable: true),
                    ApprovalFromCenterPath = table.Column<string>(type: "text", nullable: true),
                    OtherDocumentsPath = table.Column<string>(type: "text", nullable: true),
                    InspectorId = table.Column<Guid>(type: "uuid", nullable: true),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherVerificationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherVerificationRequests_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherVerificationRequests_Users_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherVerificationRequests_Users_InspectorId",
                        column: x => x.InspectorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherFeedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ModeratedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModeratedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModerationNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherFeedbacks", x => x.Id);
                    table.CheckConstraint("CK_TeacherFeedback_Rating", "\"Rating\" BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_TeacherFeedback_Reviewer", "(\"StudentProfileId\" IS NOT NULL) OR (\"ParentProfileId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_TeacherFeedbacks_ParentProfiles_ParentProfileId",
                        column: x => x.ParentProfileId,
                        principalTable: "ParentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherFeedbacks_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherFeedbacks_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherFeedbacks_Users_ModeratedByUserId",
                        column: x => x.ModeratedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Decision = table.Column<int>(type: "integer", nullable: false),
                    DecidedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DecidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_Users_DecidedByUserId",
                        column: x => x.DecidedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CourseFeedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ModeratedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModeratedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModerationNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseFeedbacks", x => x.Id);
                    table.CheckConstraint("CK_Review_Rating", "\"Rating\" BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_CourseFeedbacks_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseFeedbacks_ParentProfiles_ParentProfileId",
                        column: x => x.ParentProfileId,
                        principalTable: "ParentProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseFeedbacks_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseFeedbacks_Users_ModeratedByUserId",
                        column: x => x.ModeratedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Mark = table.Column<float>(type: "real", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseResults_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseResults_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseResults_TeacherProfiles_TeacherProfileId",
                        column: x => x.TeacherProfileId,
                        principalTable: "TeacherProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ConfirmedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CancelledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CancelReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectBuilder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectBuilder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectBuilder_ClassSchedules_ClassScheduleId",
                        column: x => x.ClassScheduleId,
                        principalTable: "ClassSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectBuilder_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectBuilder_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuspensionRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    SuspendedFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SuspendedTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActionByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuspensionRecords", x => x.Id);
                    table.CheckConstraint("CK_Suspension_Target", "(\"UserId\" IS NOT NULL) OR (\"CourseId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_SuspensionRecords_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SuspensionRecords_Users_ActionByUserId",
                        column: x => x.ActionByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SuspensionRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LessonPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SyllabusId = table.Column<Guid>(type: "uuid", nullable: false),
                    Topic = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    StudentTask = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    MaterialsUsed = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPlans_Syllabuses_SyllabusId",
                        column: x => x.SyllabusId,
                        principalTable: "Syllabuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillingRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BillingType = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: true),
                    EnrollmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChargedUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    PaidAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaymentReference = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillingRecords_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BillingRecords_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BillingRecords_Users_ChargedUserId",
                        column: x => x.ChargedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "LastUpdatedAt", "SubjectName" },
                values: new object[,]
                {
                    { new Guid("11111111-aaaa-bbbb-cccc-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Algebra, Geometry, and Calculus for high school students", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mathematics" },
                    { new Guid("22222222-aaaa-bbbb-cccc-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Classical mechanics, thermodynamics, and electromagnetism", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Physics" },
                    { new Guid("33333333-aaaa-bbbb-cccc-333333333333"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Organic and inorganic chemistry fundamentals", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chemistry" },
                    { new Guid("44444444-aaaa-bbbb-cccc-444444444444"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "English language and literature for academic excellence", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "English" }
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPackages",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "IsActive", "IsDeleted", "LastUpdatedAt", "MaxCoursePosts", "MonthlyPrice", "PackageName", "Tier" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-bbbb-cccc-dddd-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Perfect for small centers just starting out. Post up to 5 courses per month.", 1, true, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 500000m, "Basic Plan", 1 },
                    { new Guid("aaaaaaaa-bbbb-cccc-dddd-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ideal for growing centers. Post up to 15 courses per month.", 2, true, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, 1500000m, "Standard Plan", 2 },
                    { new Guid("aaaaaaaa-bbbb-cccc-dddd-333333333333"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Best for established centers. Post up to 50 courses per month.", 3, true, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 50, 4000000m, "Premium Plan", 3 },
                    { new Guid("aaaaaaaa-bbbb-cccc-dddd-444444444444"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Unlimited growth potential. Post up to 200 courses per month.", 4, true, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 200, 10000000m, "Enterprise Plan", 4 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsDeleted", "LastUpdatedAt", "PasswordHash", "PhoneNumber", "Role", "Status", "UserName" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@example.com", "System Admin", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337", "+10000000000", 1, 1, "admin" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "center.active@example.com", "Emily Clark", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337", "+10000000001", 2, 1, "center_active" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "center.pending@example.com", "Michael Brown", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337", "+10000000002", 2, 0, "center_pending" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "teacher.jane@example.com", "Jane Doe", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337", "+10000000003", 3, 1, "teacher_jane" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "teacher.john@example.com", "John Smith", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337", "+10000000004", 3, 0, "teacher_john" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "parent.liam@example.com", "Liam Johnson", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337", "+10000000005", 5, 1, "parent_liam" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "student.ava@example.com", "Ava Johnson", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337", "+10000000006", 4, 1, "student_ava" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "student.noah@example.com", "Noah Williams", false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337", "+10000000007", 4, 0, "student_noah" }
                });

            migrationBuilder.InsertData(
                table: "CenterProfiles",
                columns: new[] { "Id", "Address", "BusinessRegistrationPath", "CenterName", "City", "ContactEmail", "ContactPhone", "CreatedAt", "District", "IsDeleted", "IssueDate", "LastUpdatedAt", "Latitude", "LicenseDocumentPath", "LicenseIssuedBy", "LicenseNumber", "Longitude", "OtherDocumentsPath", "OwnerName", "RejectionReason", "Status", "TaxCodeDocumentPath", "UserId", "VerificationCompletedAt", "VerificationNotes", "VerificationRequestedAt" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999999"), "123 Learning Ave, Cityville", null, "Bright Future Center", null, "contact@brightfuture.example.com", "+10000001001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new DateOnly(2024, 1, 15), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Education Dept", "LIC-2024-0001", null, null, "Emily Clark", null, 4, null, new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Successfully verified and approved", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "456 Discovery Rd, Townsburg", null, "New Horizons Center", null, "hello@newhorizons.example.com", "+10000001002", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new DateOnly(2025, 2, 1), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Education Dept", "LIC-2025-0005", null, null, "Michael Brown", null, 0, null, new Guid("33333333-3333-3333-3333-333333333333"), null, null, null }
                });

            migrationBuilder.InsertData(
                table: "ParentProfiles",
                columns: new[] { "Id", "Address", "CreatedAt", "IsDeleted", "LastUpdatedAt", "PhoneSecondary", "UserId" },
                values: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "789 Family St, Suburbia", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "+10000002002", new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.InsertData(
                table: "CenterSubscriptions",
                columns: new[] { "Id", "AutoRenewalDate", "AutoRenewalEnabled", "CancellationReason", "CancelledAt", "CenterProfileId", "CreatedAt", "EndDate", "IsDeleted", "LastUpdatedAt", "StartDate", "Status", "SubscriptionPackageId" },
                values: new object[] { new Guid("bbbbbbbb-cccc-dddd-eeee-111111111111"), new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, null, new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new Guid("aaaaaaaa-bbbb-cccc-dddd-222222222222") });

            migrationBuilder.InsertData(
                table: "StudentProfiles",
                columns: new[] { "Id", "ClassName", "CreatedAt", "GradeLevel", "IsDeleted", "LastUpdatedAt", "ParentProfileId", "SchoolName", "SchoolYear", "UserId" },
                values: new object[,]
                {
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "10A01", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "City High School", "2024-2025", new Guid("77777777-7777-7777-7777-777777777777") },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "08A12", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Town Middle School", "2024-2025", new Guid("88888888-8888-8888-8888-888888888888") }
                });

            migrationBuilder.InsertData(
                table: "TeacherProfiles",
                columns: new[] { "Id", "Bio", "CenterProfileId", "CreatedAt", "IsDeleted", "LastUpdatedAt", "LicenseNumber", "Qualifications", "Subjects", "TeachAtClasses", "TeachingAtSchool", "UserId", "VerificationCompletedAt", "VerificationNotes", "VerificationRequestedAt", "VerificationStatus", "YearOfExperience" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Experienced STEM teacher", new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TCH-2020-123", "B.Ed, M.Ed", "Math,Physics", "10A01", "City High School", new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Verified per Circular 29", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 5 },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Chemistry enthusiast", new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TCH-2023-456", "B.Sc", "Chemistry", "08A05", "City High School", new Guid("55555555-5555-5555-5555-555555555555"), null, null, null, 0, 2 }
                });

            migrationBuilder.InsertData(
                table: "ClassSchedules",
                columns: new[] { "Id", "CreatedAt", "DayOfWeek", "EndDate", "EndTime", "IsDeleted", "LastUpdatedAt", "RoomOrLink", "StartDate", "StartTime", "TeacherProfileId" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-1111-aaaa-bbbb-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateOnly(2025, 5, 31), new TimeOnly(10, 30, 0), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Room 101, 123 Learning Ave, Cityville", new DateOnly(2025, 2, 1), new TimeOnly(9, 0, 0), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("bbbbbbbb-2222-aaaa-bbbb-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, new DateOnly(2025, 5, 31), new TimeOnly(15, 30, 0), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Room 101, 123 Learning Ave, Cityville", new DateOnly(2025, 2, 1), new TimeOnly(14, 0, 0), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("cccccccc-3333-aaaa-bbbb-333333333333"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, new DateOnly(2025, 5, 31), new TimeOnly(10, 30, 0), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Room 102, 123 Learning Ave, Cityville", new DateOnly(2025, 2, 1), new TimeOnly(9, 0, 0), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("dddddddd-4444-aaaa-bbbb-444444444444"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, new DateOnly(2025, 5, 31), new TimeOnly(16, 30, 0), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lab Room 1, 123 Learning Ave, Cityville", new DateOnly(2025, 2, 1), new TimeOnly(15, 0, 0), new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc") },
                    { new Guid("eeeeeeee-5555-aaaa-bbbb-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, new DateOnly(2025, 5, 31), new TimeOnly(11, 30, 0), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "https://zoom.us/j/1234567890", new DateOnly(2025, 2, 1), new TimeOnly(10, 0, 0), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") }
                });

            migrationBuilder.InsertData(
                table: "Syllabuses",
                columns: new[] { "Id", "AssessmentMethod", "CourseMaterial", "CreatedAt", "Description", "GradeLevel", "IsDeleted", "LastUpdatedAt", "SubjectId", "SyllabusName", "TeacherProfileId" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-1111-2222-3333-aaaaaaaaaaaa"), "Weekly quizzes (20%), Mid-term exam (30%), Final exam (50%)", "Textbook: Mathematics Grade 10, Calculator, Graph paper", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Comprehensive mathematics syllabus covering algebra, geometry, and trigonometry for grade 10 students.", 10, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-aaaa-bbbb-cccc-111111111111"), "Advanced Mathematics Grade 10", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("bbbbbbbb-1111-2222-3333-bbbbbbbbbbbb"), "Lab reports (25%), Weekly assignments (15%), Mid-term (30%), Final (30%)", "Textbook: Physics Grade 10, Lab equipment, Scientific calculator", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Introduction to physics covering mechanics, energy, and waves for grade 10 students.", 10, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-aaaa-bbbb-cccc-222222222222"), "Physics Fundamentals Grade 10", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("cccccccc-1111-2222-3333-cccccccccccc"), "Lab practicals (30%), Quizzes (20%), Projects (20%), Final exam (30%)", "Textbook: Chemistry Basics, Lab manual, Safety equipment", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Basic chemistry concepts including periodic table, chemical reactions, and laboratory safety.", 8, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-aaaa-bbbb-cccc-333333333333"), "Chemistry Basics Grade 8", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc") }
                });

            migrationBuilder.InsertData(
                table: "TeacherVerificationRequests",
                columns: new[] { "Id", "AdminId", "ApprovalFromCenterPath", "CompletedAt", "CreatedAt", "EmploymentContractPath", "InspectorId", "IsDeleted", "LastUpdatedAt", "Notes", "OtherDocumentsPath", "QualificationCertificatePath", "RequestedAt", "Status", "TeacherProfileId" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, "/docs/teachers/jane/center-approval.pdf", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "/docs/teachers/jane/contract.pdf", null, false, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "All documents verified", null, "/docs/teachers/jane/qualification.pdf", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), null, null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, 0, new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc") }
                });

            migrationBuilder.InsertData(
                table: "LessonPlans",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "LastUpdatedAt", "MaterialsUsed", "Notes", "StudentTask", "SyllabusId", "Topic" },
                values: new object[,]
                {
                    { new Guid("11111111-ffff-eeee-dddd-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Whiteboard, Graphing calculator, Textbook", "Focus on standard form ax² + bx + c = 0", "Complete exercises 1-20 from textbook. Submit solutions before next class.", new Guid("aaaaaaaa-1111-2222-3333-aaaaaaaaaaaa"), "Introduction to Quadratic Equations" },
                    { new Guid("22222222-ffff-eeee-dddd-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Graph paper, Graphing software, Calculator", "Practice identifying maximum and minimum points", "Graph 5 quadratic functions and identify vertex, axis of symmetry", new Guid("aaaaaaaa-1111-2222-3333-aaaaaaaaaaaa"), "Graphing Quadratic Functions" },
                    { new Guid("33333333-ffff-eeee-dddd-333333333333"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lab equipment, Stopwatch, Rulers, Masses", "Hands-on experiment to demonstrate F = ma", "Lab report on motion experiments. Calculate acceleration from data.", new Guid("bbbbbbbb-1111-2222-3333-bbbbbbbbbbbb"), "Newton's Laws of Motion" },
                    { new Guid("44444444-ffff-eeee-dddd-444444444444"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Textbook, Calculator, Diagram sheets", "Emphasize conservation of energy principle", "Solve energy conservation problems. Complete worksheet.", new Guid("bbbbbbbb-1111-2222-3333-bbbbbbbbbbbb"), "Energy and Work" },
                    { new Guid("55555555-ffff-eeee-dddd-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Periodic table poster, Flashcards, Worksheets", "Focus on element symbols and atomic numbers", "Memorize first 20 elements. Complete periodic table worksheet.", new Guid("cccccccc-1111-2222-3333-cccccccccccc"), "Introduction to Periodic Table" },
                    { new Guid("66666666-ffff-eeee-dddd-666666666666"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Molecular model kits, Lab chemicals, Safety goggles", "Emphasize safety procedures in lab", "Draw Lewis structures for 10 molecules. Lab: observe reactions.", new Guid("cccccccc-1111-2222-3333-cccccccccccc"), "Chemical Bonding Basics" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_CourseId",
                table: "ApprovalRequests",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_DecidedByUserId",
                table: "ApprovalRequests",
                column: "DecidedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName",
                table: "AuditLogs",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingRecords_BillingType_CourseId_EnrollmentId",
                table: "BillingRecords",
                columns: new[] { "BillingType", "CourseId", "EnrollmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_BillingRecords_ChargedUserId",
                table: "BillingRecords",
                column: "ChargedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingRecords_CourseId",
                table: "BillingRecords",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingRecords_EnrollmentId",
                table: "BillingRecords",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CenterProfiles_UserId",
                table: "CenterProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CenterSubscriptions_CenterProfileId",
                table: "CenterSubscriptions",
                column: "CenterProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CenterSubscriptions_CenterProfileId_Status",
                table: "CenterSubscriptions",
                columns: new[] { "CenterProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CenterSubscriptions_SubscriptionPackageId",
                table: "CenterSubscriptions",
                column: "SubscriptionPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_CenterVerificationRequests_AdminId",
                table: "CenterVerificationRequests",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_CenterVerificationRequests_CenterProfileId",
                table: "CenterVerificationRequests",
                column: "CenterProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CenterVerificationRequests_InspectorId",
                table: "CenterVerificationRequests",
                column: "InspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_TeacherProfileId",
                table: "ClassSchedules",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseFeedbacks_CourseId",
                table: "CourseFeedbacks",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseFeedbacks_ModeratedByUserId",
                table: "CourseFeedbacks",
                column: "ModeratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseFeedbacks_ParentProfileId",
                table: "CourseFeedbacks",
                column: "ParentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseFeedbacks_StudentProfileId",
                table: "CourseFeedbacks",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseResults_CourseId",
                table: "CourseResults",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseResults_StudentProfileId",
                table: "CourseResults",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseResults_TeacherProfileId",
                table: "CourseResults",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CenterProfileId",
                table: "Courses",
                column: "CenterProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherProfileId",
                table: "Courses",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId_StudentProfileId",
                table: "Enrollments",
                columns: new[] { "CourseId", "StudentProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentProfileId",
                table: "Enrollments",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_ReportType",
                table: "GeneratedReports",
                column: "ReportType");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_RequestedByUserId",
                table: "GeneratedReports",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlans_SyllabusId",
                table: "LessonPlans",
                column: "SyllabusId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentProfiles_UserId",
                table: "ParentProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_ParentProfileId",
                table: "StudentProfiles",
                column: "ParentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_UserId",
                table: "StudentProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectBuilder_ClassScheduleId",
                table: "SubjectBuilder",
                column: "ClassScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectBuilder_CourseId",
                table: "SubjectBuilder",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectBuilder_SubjectId",
                table: "SubjectBuilder",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPackages_Tier",
                table: "SubscriptionPackages",
                column: "Tier");

            migrationBuilder.CreateIndex(
                name: "IX_SuspensionRecords_ActionByUserId",
                table: "SuspensionRecords",
                column: "ActionByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SuspensionRecords_CourseId",
                table: "SuspensionRecords",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_SuspensionRecords_UserId",
                table: "SuspensionRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Syllabuses_SubjectId",
                table: "Syllabuses",
                column: "SubjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Syllabuses_TeacherProfileId",
                table: "Syllabuses",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherFeedbacks_ModeratedByUserId",
                table: "TeacherFeedbacks",
                column: "ModeratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherFeedbacks_ParentProfileId",
                table: "TeacherFeedbacks",
                column: "ParentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherFeedbacks_StudentProfileId",
                table: "TeacherFeedbacks",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherFeedbacks_TeacherProfileId",
                table: "TeacherFeedbacks",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherProfiles_CenterProfileId",
                table: "TeacherProfiles",
                column: "CenterProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherProfiles_UserId",
                table: "TeacherProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherVerificationRequests_AdminId",
                table: "TeacherVerificationRequests",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherVerificationRequests_InspectorId",
                table: "TeacherVerificationRequests",
                column: "InspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherVerificationRequests_TeacherProfileId",
                table: "TeacherVerificationRequests",
                column: "TeacherProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalRequests");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BillingRecords");

            migrationBuilder.DropTable(
                name: "CenterSubscriptions");

            migrationBuilder.DropTable(
                name: "CenterVerificationRequests");

            migrationBuilder.DropTable(
                name: "CourseFeedbacks");

            migrationBuilder.DropTable(
                name: "CourseResults");

            migrationBuilder.DropTable(
                name: "GeneratedReports");

            migrationBuilder.DropTable(
                name: "LessonPlans");

            migrationBuilder.DropTable(
                name: "SubjectBuilder");

            migrationBuilder.DropTable(
                name: "SuspensionRecords");

            migrationBuilder.DropTable(
                name: "TeacherFeedbacks");

            migrationBuilder.DropTable(
                name: "TeacherVerificationRequests");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "SubscriptionPackages");

            migrationBuilder.DropTable(
                name: "Syllabuses");

            migrationBuilder.DropTable(
                name: "ClassSchedules");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "StudentProfiles");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "TeacherProfiles");

            migrationBuilder.DropTable(
                name: "ParentProfiles");

            migrationBuilder.DropTable(
                name: "CenterProfiles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
