CREATE TABLE Freelancer (
    ID INT PRIMARY KEY,
    UserTelegramID VARCHAR(255),
    Name VARCHAR(255),
    Stacks VARCHAR(255),
    ExperienceTime DECIMAL(5,2),
    Portfolio VARCHAR(255),
    ContactTelegram VARCHAR(255),
    ContactEmail VARCHAR(255),
    ContactPhone VARCHAR(255),
    OtherContacts VARCHAR(255),
    Status BOOLEAN,
    Verified BOOLEAN,
    LastUpdate DATETIME,
    RegistrationDate DATETIME,
    InactiveDate DATETIME
);


CREATE TABLE Company (
    ID INT PRIMARY KEY,
    UserTelegramID VARCHAR(255),
    Name VARCHAR(255),
    State VARCHAR(100),
    Country VARCHAR(100),
    ContactTelegram VARCHAR(255),
    ContactEmail VARCHAR(255),
    ContactPhone VARCHAR(255),
    OtherContacts VARCHAR(255),
    Status BOOLEAN,
    Verified BOOLEAN,
    LastUpdate DATETIME,
    RegistrationDate DATETIME,
    InactiveDate DATETIME
);

CREATE TABLE Vacant (
    ID INT PRIMARY KEY,
    CompanyID INT,
    VacantName VARCHAR(255),
    Description VARCHAR(255),
    SkillsRequired VARCHAR(255),
    SpecialSkills VARCHAR(255),
    Benefits VARCHAR(255),
    Modality VARCHAR(100),
    CoverageCity VARCHAR(100),
    CoverageState VARCHAR(100),
    CoverageCountry VARCHAR(100),
    BudgetMin FLOAT,
    BudgetMax FLOAT,
    Status BOOLEAN,
    CreationDate DATETIME,
    LastUpdate DATETIME,
    ExclusionDate DATETIME,
    FOREIGN KEY (CompanyID) REFERENCES Company(ID)
);

CREATE TABLE FreelancerJob (
    ID INT PRIMARY KEY,
    EmployerID INT,
    Description VARCHAR(255),
    ProjectType VARCHAR(50),
    SkillsRequired VARCHAR(255),
    SpecialSkills VARCHAR(255),
    BudgetMin FLOAT,
    BudgetMax FLOAT,
    Status BOOLEAN,
    ExpirationDate DATETIME,
    CreationDate DATETIME,
    LastUpdate DATETIME,
    ExclusionDate DATETIME,
    FOREIGN KEY (EmployerID) REFERENCES Company(ID)
);

CREATE TABLE Application (
    ID INT PRIMARY KEY,
    FreelancerID INT,
    JobID INT,
    VacantID INT,
    ProposalDetails VARCHAR(255),
    ProposedHourlyRate FLOAT,
    TotalHours INT,
    Status BOOLEAN,
    SubmissionDate DATETIME,
    AcceptedDate DATETIME,
    RejectedDate DATETIME,
    EmployerObservation VARCHAR(255),
    FOREIGN KEY (FreelancerID) REFERENCES Freelancer(ID),
    FOREIGN KEY (JobID) REFERENCES FreelancerJob(ID),
    FOREIGN KEY (VacantID) REFERENCES Vacant(ID)
);

CREATE TABLE CompanyToFreelancerReview (
    ID INT PRIMARY KEY,
    CompanyID INT,
    FreelancerID INT,
    JobID INT,
    Rating INT,
    Comment VARCHAR(255),
    ReviewDate DATETIME,
    FOREIGN KEY (CompanyID) REFERENCES Company(ID),
    FOREIGN KEY (FreelancerID) REFERENCES Freelancer(ID),
    FOREIGN KEY (JobID) REFERENCES Vacant(ID)
);

CREATE TABLE FreelancerToCompanyReview (
    ID INT PRIMARY KEY,
    FreelancerID INT,
    CompanyID INT,
    JobID INT,
    Rating INT,
    Comment VARCHAR(255),
    ReviewDate DATETIME,
    FOREIGN KEY (FreelancerID) REFERENCES Freelancer(ID),
    FOREIGN KEY (CompanyID) REFERENCES Company(ID),
    FOREIGN KEY (JobID) REFERENCES Vacant(ID)
);