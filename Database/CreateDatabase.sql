/* ============================================================
   1. TABLES
   ============================================================ */

CREATE TABLE dbo.Patients
(
    Id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FullName        NVARCHAR(200)     NOT NULL,
    IdentityNumber  NVARCHAR(40)      NULL,
    FileNumber      NVARCHAR(40)      NULL,
    MedicalAid      NVARCHAR(100)     NULL,
    Email           NVARCHAR(254)     NOT NULL,
    DateOfBirth     DATE              NULL,
    Notes           NVARCHAR(MAX)     NULL,
    Phone           NVARCHAR(30)      NULL
);
GO

CREATE TABLE dbo.Practitioners
(
    Id          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FullName    NVARCHAR(200)     NOT NULL,
    Specialty   NVARCHAR(120)     NULL,
    PracticeId  INT               NOT NULL
);
GO

CREATE TABLE dbo.PracticeLocations
(
    Id    INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name  NVARCHAR(150)     NOT NULL
);
GO

CREATE TABLE dbo.Appointments
(
    Id                     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PatientId              INT               NOT NULL REFERENCES dbo.Patients(Id),
    PractitionerId         INT               NOT NULL REFERENCES dbo.Practitioners(Id),
    LocationId             INT               NOT NULL REFERENCES dbo.PracticeLocations(Id),
    StartsAt               DATETIME2         NOT NULL,
    EndsAt                 DATETIME2         NOT NULL,
    BufferMinutes          INT               NOT NULL CONSTRAINT DF_Appointments_Buffer       DEFAULT 30,
    Status                 NVARCHAR(24)      NOT NULL  CONSTRAINT DF_Appointments_Status       DEFAULT N'Confirmed',
    Notes                  NVARCHAR(1000)    NULL,
    CreatedAt              DATETIME2         NOT NULL CONSTRAINT DF_Appointments_Created      DEFAULT SYSDATETIME(),
    CancelledAt            DATETIME2         NULL,
    ConfirmationEmailSent  BIT               NOT NULL CONSTRAINT DF_Appointments_Confirmation DEFAULT 0,
    ReminderEmailSent      BIT               NOT NULL CONSTRAINT DF_Appointments_Reminder     DEFAULT 0,

    CONSTRAINT CK_Appointments_Duration CHECK (EndsAt > StartsAt),
    CONSTRAINT CK_Appointments_Buffer   CHECK (BufferMinutes >= 0)
);
GO

CREATE INDEX IX_Appointments_Practitioner_StartsAt
    ON dbo.Appointments (PractitionerId, StartsAt)
    INCLUDE (EndsAt, BufferMinutes, Status);
GO

CREATE TABLE dbo.Users
(
    Id             INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    Email          NVARCHAR(256)     NOT NULL,
    Username       NVARCHAR(100)     NOT NULL,
    DisplayName    NVARCHAR(150)     NOT NULL,
    JobTitle       NVARCHAR(150)     NOT NULL CONSTRAINT DF_Users_JobTitle  DEFAULT (N''),
    PasswordHash   NVARCHAR(512)     NULL,   
    GoogleSubject  NVARCHAR(100)     NULL,  
    IsActive       BIT               NOT NULL CONSTRAINT DF_Users_IsActive  DEFAULT (1),
    CreatedAt      DATETIME2         NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
    LastLoginAt    DATETIME2         NULL
);
GO

CREATE UNIQUE INDEX IX_Users_Email    ON dbo.Users (Email);
CREATE UNIQUE INDEX IX_Users_Username ON dbo.Users (Username);
CREATE UNIQUE INDEX IX_Users_GoogleSubject ON dbo.Users (GoogleSubject)
    WHERE GoogleSubject IS NOT NULL;
GO

/* ============================================================
   2. SEED DATA
   ============================================================ */

SET IDENTITY_INSERT dbo.Patients ON;

INSERT dbo.Patients (Id, FullName, FileNumber, MedicalAid, Email)
VALUES
    (1, N'Sanele C Khumalo',       N'VTYZ6E7KC39R', N'UMVUZO HEALTH', N'sanele@example.test'),
    (2, N'Madumetja d Mushi',      N'VTSWCQ1GXZ4E', N'BONITAS',       N'madumetja@example.test'),
    (3, N'Patrick Buhle Ngcetane', N'VTES9NLC7FM4', N'BONITAS',       N'patrick@example.test'),
    (4, N'Happy Morebodi',         N'VTZG5US74FJ9', N'GEMS',          N'happy@example.test');

SET IDENTITY_INSERT dbo.Patients OFF;
GO

SET IDENTITY_INSERT dbo.Practitioners ON;

INSERT dbo.Practitioners (Id, FullName, Specialty, PracticeId)
VALUES
    (1, N'Lebo Maluleka',    N'Audiologist',      1),
    (2, N'Thabiso Mogashoa', N'Speech Therapist', 1);

SET IDENTITY_INSERT dbo.Practitioners OFF;
GO

SET IDENTITY_INSERT dbo.PracticeLocations ON;

INSERT dbo.PracticeLocations (Id, Name)
VALUES
    (1, N'Pretoria'),
    (2, N'Tembisa'),
    (3, N'Kempton Park'),
    (4, N'Benoni'),
    (5, N'Boksburg'),
    (6, N'Lenasia');

SET IDENTITY_INSERT dbo.PracticeLocations OFF;
GO

-- Login user (username: thabiso, password: ChangeMe!123, change it after first sign-in)
INSERT dbo.Users (Email, Username, DisplayName, JobTitle, PasswordHash)
VALUES
(
    N'thabiso@example.com',
    N'thabiso',
    N'Thabiso Mokoena',
    N'Practice administrator',
    N'AQAAAAEAAYagAAAAEJoT9F9N1J4qx0eOSLBaM1QjfkWASnihcCJHlfWFNQVtwzbYgbtOa4uwjhcp3ICxow=='
);
GO

SELECT Id, Email, Username, DisplayName, JobTitle FROM dbo.Users;
GO