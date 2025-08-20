-- =============================================
-- QUIZ TEST DATA SCRIPT
-- File: Quiz_Test_Data.sql
-- Purpose: Create test data for quiz functionality
-- =============================================

-- Clean up existing data
DELETE FROM StudentQuizAttempts;
DELETE FROM QuizAnswers;
DELETE FROM QuizQuestions;
DELETE FROM Quizzes;

-- Reset identity columns
DBCC CHECKIDENT ('Quizzes', RESEED, 0);
DBCC CHECKIDENT ('QuizQuestions', RESEED, 0);
DBCC CHECKIDENT ('QuizAnswers', RESEED, 0);

-- Get first lesson ID
DECLARE @LessonId INT;
SELECT TOP 1 @LessonId = LessonId FROM Lessons ORDER BY LessonId;

IF @LessonId IS NULL
BEGIN
    PRINT 'No lessons found! Please create lessons first.';
    RETURN;
END

-- Create quiz
INSERT INTO Quizzes (LessonId, Title, Description, TimeLimit, PassingScore, MaxAttempts, IsRandomized)
VALUES (@LessonId, N'Quiz Test - Basic Knowledge', N'Test quiz for basic knowledge assessment', 15, 50.00, 3, 0);

-- Get quiz ID
DECLARE @QuizId INT = SCOPE_IDENTITY();

-- Create questions
INSERT INTO QuizQuestions (QuizId, QuestionText, QuestionType, Points, SortOrder)
VALUES (@QuizId, N'What is 2 + 2?', 'MultipleChoice', 1, 1);

INSERT INTO QuizQuestions (QuizId, QuestionText, QuestionType, Points, SortOrder)
VALUES (@QuizId, N'What is the capital of Vietnam?', 'MultipleChoice', 1, 2);

INSERT INTO QuizQuestions (QuizId, QuestionText, QuestionType, Points, SortOrder)
VALUES (@QuizId, N'Which programming language is this project built with?', 'MultipleChoice', 1, 3);

-- Get question IDs
DECLARE @QuestionId1 INT, @QuestionId2 INT, @QuestionId3 INT;
SELECT @QuestionId1 = QuestionId FROM QuizQuestions WHERE QuizId = @QuizId AND SortOrder = 1;
SELECT @QuestionId2 = QuestionId FROM QuizQuestions WHERE QuizId = @QuizId AND SortOrder = 2;
SELECT @QuestionId3 = QuestionId FROM QuizQuestions WHERE QuizId = @QuizId AND SortOrder = 3;

-- Create answers for question 1
INSERT INTO QuizAnswers (QuestionId, AnswerText, IsCorrect, SortOrder)
VALUES (@QuestionId1, N'3', 0, 1);

INSERT INTO QuizAnswers (QuestionId, AnswerText, IsCorrect, SortOrder)
VALUES (@QuestionId1, N'4', 1, 2); -- Correct answer

INSERT INTO QuizAnswers (QuestionId, AnswerText, IsCorrect, SortOrder)
VALUES (@QuestionId1, N'5', 0, 3);

-- Create answers for question 2
INSERT INTO QuizAnswers (QuestionId, AnswerText, IsCorrect, SortOrder)
VALUES (@QuestionId2, N'Hanoi', 1, 1); -- Correct answer

INSERT INTO QuizAnswers (QuestionId, AnswerText, IsCorrect, SortOrder)
VALUES (@QuestionId2, N'Ho Chi Minh City', 0, 2);

INSERT INTO QuizAnswers (QuestionId, AnswerText, IsCorrect, SortOrder)
VALUES (@QuestionId2, N'Da Nang', 0, 3);

-- Create answers for question 3
INSERT INTO QuizAnswers (QuestionId, AnswerText, IsCorrect, SortOrder)
VALUES (@QuestionId3, N'Java', 0, 1);

INSERT INTO QuizAnswers (QuestionId, AnswerText, IsCorrect, SortOrder)
VALUES (@QuestionId3, N'Python', 0, 2);

INSERT INTO QuizAnswers (QuestionId, AnswerText, IsCorrect, SortOrder)
VALUES (@QuestionId3, N'C#', 1, 3); -- Correct answer

-- Verify data
PRINT '=== QUIZ DATA CREATED SUCCESSFULLY ===';
PRINT 'Quiz ID: ' + CAST(@QuizId AS VARCHAR);
PRINT 'Lesson ID: ' + CAST(@LessonId AS VARCHAR);
PRINT 'Questions created: 3';
PRINT 'Answers created: 9';

-- Show created data
SELECT 'QUIZ' as TableName, QuizId, Title, LessonId FROM Quizzes WHERE QuizId = @QuizId;
SELECT 'QUESTIONS' as TableName, QuestionId, QuestionText, QuizId FROM QuizQuestions WHERE QuizId = @QuizId;
SELECT 'ANSWERS' as TableName, AnswerId, AnswerText, IsCorrect, QuestionId FROM QuizAnswers WHERE QuestionId IN (@QuestionId1, @QuestionId2, @QuestionId3);

PRINT '=== SCRIPT COMPLETED ===';
PRINT 'You can now test the quiz functionality!';


-----------------------
-- Create Database
CREATE DATABASE CourseraStyleLMS;
GO
USE CourseraStyleLMS;
GO

-- Core Tables

-- Table: Roles
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- Table: Categories (for course organization)
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    ParentCategoryId INT NULL, -- For subcategories
    IsActive BIT DEFAULT 1
);
GO

-- Table: Users
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    FullName NVARCHAR(255) NOT NULL,
    ProfileImage NVARCHAR(500) NULL,
    Bio NVARCHAR(MAX) NULL,
    RoleId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastLoginAt DATETIME NULL,
    IsActive BIT DEFAULT 1,
    EmailVerified BIT DEFAULT 0
);
GO

-- Table: InstructorProfiles (additional info for instructors)
CREATE TABLE InstructorProfiles (
    InstructorId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    Headline NVARCHAR(255) NULL,
    Biography NVARCHAR(MAX) NULL,
    Website NVARCHAR(500) NULL,
    LinkedInProfile NVARCHAR(500) NULL,
    TwitterHandle NVARCHAR(100) NULL,
    YearsOfExperience INT NULL,
    TotalStudents INT DEFAULT 0,
    AverageRating DECIMAL(3,2) DEFAULT 0.00,
    TotalCourses INT DEFAULT 0
);
GO

-- Table: Courses
CREATE TABLE Courses (
    CourseId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Subtitle NVARCHAR(500) NULL,
    Description NVARCHAR(MAX) NOT NULL,
    WhatYouWillLearn NVARCHAR(MAX) NULL,
    Prerequisites NVARCHAR(MAX) NULL,
    TargetAudience NVARCHAR(MAX) NULL,
    CourseImage NVARCHAR(500) NULL,
    PromoVideoUrl NVARCHAR(500) NULL,
    InstructorId INT NOT NULL,
    CategoryId INT NOT NULL,
    DifficultyLevel NVARCHAR(20) CHECK (DifficultyLevel IN ('Beginner', 'Intermediate', 'Advanced')) DEFAULT 'Beginner',
    Language NVARCHAR(50) DEFAULT 'English',
    Price DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    DiscountPrice DECIMAL(10,2) NULL,
    EstimatedDuration INT NULL, -- in hours
    IsPublished BIT DEFAULT 0,
    IsFree BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    PublishedAt DATETIME NULL,
    TotalEnrollments INT DEFAULT 0,
    AverageRating DECIMAL(3,2) DEFAULT 0.00,
    TotalReviews INT DEFAULT 0
);
GO

-- Table: Lessons
CREATE TABLE Lessons (
    LessonId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    LessonType NVARCHAR(50) CHECK (LessonType IN ('Video', 'Article', 'Quiz', 'Assignment', 'Download')) NOT NULL,
    ContentUrl NVARCHAR(500) NULL, -- Video URL, file path, etc.
    Duration INT NULL, -- in minutes
    SortOrder INT NOT NULL,
    IsPreview BIT DEFAULT 0, -- Free preview lesson
    IsCompleted BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- Table: Enrollments
CREATE TABLE Enrollments (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CourseId INT NOT NULL,
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    CompletionDate DATETIME NULL,
    ProgressPercentage DECIMAL(5,2) DEFAULT 0.00,
    LastAccessedAt DATETIME NULL,
    Status NVARCHAR(20) CHECK (Status IN ('Active', 'Completed', 'Dropped', 'Suspended')) DEFAULT 'Active',
    PaymentStatus NVARCHAR(20) CHECK (PaymentStatus IN ('Free', 'Paid', 'Refunded')) DEFAULT 'Free'
);
GO

-- Table: Orders
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    DiscountAmount DECIMAL(10,2) DEFAULT 0.00,
    FinalAmount DECIMAL(10,2) NOT NULL,
    OrderStatus NVARCHAR(20) CHECK (OrderStatus IN ('Pending', 'Completed', 'Failed', 'Refunded')) DEFAULT 'Pending',
    PaymentMethod NVARCHAR(50) NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    CompletedAt DATETIME NULL
);
GO

-- Table: OrderItems
CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    CourseId INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    DiscountApplied DECIMAL(10,2) DEFAULT 0.00
);
GO

-- Table: Payments
CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    PaymentGateway NVARCHAR(50) NOT NULL, -- PayPal, Stripe, etc.
    TransactionId NVARCHAR(255) NULL,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentDate DATETIME DEFAULT GETDATE(),
    PaymentStatus NVARCHAR(20) CHECK (PaymentStatus IN ('Pending', 'Completed', 'Failed', 'Refunded')) DEFAULT 'Pending',
    GatewayResponse NVARCHAR(MAX) NULL
);
GO

-- Table: StudentProgress
CREATE TABLE StudentProgress (
    ProgressId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    LessonId INT NOT NULL,
    CourseId INT NOT NULL,
    IsCompleted BIT DEFAULT 0,
    TimeSpent INT DEFAULT 0, -- in minutes
    LastAccessedAt DATETIME DEFAULT GETDATE(),
    CompletedAt DATETIME NULL
);
GO

-- Table: CourseReviews
CREATE TABLE CourseReviews (
    ReviewId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    UserId INT NOT NULL,
    Rating INT CHECK (Rating >= 1 AND Rating <= 5) NOT NULL,
    ReviewTitle NVARCHAR(255) NULL,
    ReviewText NVARCHAR(MAX) NULL,
    ReviewDate DATETIME DEFAULT GETDATE(),
    IsPublished BIT DEFAULT 1,
    HelpfulVotes INT DEFAULT 0
);
GO

-- Table: Certificates
CREATE TABLE Certificates (
    CertificateId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CourseId INT NOT NULL,
    CertificateCode NVARCHAR(50) NOT NULL UNIQUE,
    IssuedDate DATETIME DEFAULT GETDATE(),
    CertificateUrl NVARCHAR(500) NULL, -- PDF download link
    IsVerified BIT DEFAULT 1
);
GO

-- Table: Quizzes
CREATE TABLE Quizzes (
    QuizId INT IDENTITY(1,1) PRIMARY KEY,
    LessonId INT NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    TimeLimit INT NULL, -- in minutes
    PassingScore DECIMAL(5,2) DEFAULT 70.00,
    MaxAttempts INT DEFAULT 3,
    IsRandomized BIT DEFAULT 0
);
GO

-- Table: QuizQuestions
CREATE TABLE QuizQuestions (
    QuestionId INT IDENTITY(1,1) PRIMARY KEY,
    QuizId INT NOT NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    QuestionType NVARCHAR(20) CHECK (QuestionType IN ('MultipleChoice', 'TrueFalse', 'ShortAnswer')) NOT NULL,
    Points DECIMAL(5,2) DEFAULT 1.00,
    SortOrder INT NOT NULL
);
GO

-- Table: QuizAnswers
CREATE TABLE QuizAnswers (
    AnswerId INT IDENTITY(1,1) PRIMARY KEY,
    QuestionId INT NOT NULL,
    AnswerText NVARCHAR(MAX) NOT NULL,
    IsCorrect BIT DEFAULT 0,
    SortOrder INT NOT NULL
);
GO

-- Table: StudentQuizAttempts
CREATE TABLE StudentQuizAttempts (
    AttemptId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    QuizId INT NOT NULL,
    AttemptNumber INT NOT NULL,
    Score DECIMAL(5,2) DEFAULT 0.00,
    StartedAt DATETIME DEFAULT GETDATE(),
    CompletedAt DATETIME NULL,
    TimeSpent INT NULL, -- in minutes
    IsPassed BIT DEFAULT 0
);
GO

-- Table: Forums
CREATE TABLE Forums (
    PostId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    UserId INT NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    PostDate DATETIME DEFAULT GETDATE(),
    LastReplyDate DATETIME NULL,
    ReplyCount INT DEFAULT 0,
    IsSticky BIT DEFAULT 0,
    IsClosed BIT DEFAULT 0
);
GO

-- Table: ForumReplies
CREATE TABLE ForumReplies (
    ReplyId INT IDENTITY(1,1) PRIMARY KEY,
    PostId INT NOT NULL,
    UserId INT NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    ReplyDate DATETIME DEFAULT GETDATE(),
    ParentReplyId INT NULL, -- For nested replies
    IsInstructorReply BIT DEFAULT 0
);
GO

-- Table: Coupons
CREATE TABLE Coupons (
    CouponId INT IDENTITY(1,1) PRIMARY KEY,
    CouponCode NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    DiscountType NVARCHAR(20) CHECK (DiscountType IN ('Percentage', 'Fixed')) NOT NULL,
    DiscountValue DECIMAL(10,2) NOT NULL,
    MinimumAmount DECIMAL(10,2) DEFAULT 0.00,
    MaxUsageCount INT NULL,
    CurrentUsageCount INT DEFAULT 0,
    ValidFrom DATETIME NOT NULL,
    ValidUntil DATETIME NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedBy INT NOT NULL
);
GO

-- Table: Wishlist
CREATE TABLE Wishlist (
    WishlistId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CourseId INT NOT NULL,
    AddedDate DATETIME DEFAULT GETDATE(),
    UNIQUE(UserId, CourseId)
);
GO

-- Table: Announcements (replaced News and Events)
CREATE TABLE Announcements (
    AnnouncementId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    AnnouncementType NVARCHAR(20) CHECK (AnnouncementType IN ('News', 'Event', 'Update', 'Promotion')) NOT NULL,
    AuthorId INT NOT NULL,
    PublishDate DATETIME DEFAULT GETDATE(),
    ExpiryDate DATETIME NULL,
    IsActive BIT DEFAULT 1,
    IsFeatured BIT DEFAULT 0
);
GO

-- Add Foreign Key Constraints
ALTER TABLE Categories ADD CONSTRAINT FK_Categories_Parent FOREIGN KEY (ParentCategoryId) REFERENCES Categories(CategoryId);
ALTER TABLE Users ADD CONSTRAINT FK_Users_Role FOREIGN KEY (RoleId) REFERENCES Roles(RoleId);
ALTER TABLE InstructorProfiles ADD CONSTRAINT FK_InstructorProfiles_User FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE;
ALTER TABLE Courses ADD CONSTRAINT FK_Courses_Instructor FOREIGN KEY (InstructorId) REFERENCES Users(UserId);
ALTER TABLE Courses ADD CONSTRAINT FK_Courses_Category FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId);
ALTER TABLE Lessons ADD CONSTRAINT FK_Lessons_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE;
ALTER TABLE Enrollments ADD CONSTRAINT FK_Enrollments_User FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE;
ALTER TABLE Enrollments ADD CONSTRAINT FK_Enrollments_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE;
ALTER TABLE Orders ADD CONSTRAINT FK_Orders_User FOREIGN KEY (UserId) REFERENCES Users(UserId);
ALTER TABLE OrderItems ADD CONSTRAINT FK_OrderItems_Order FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE;
ALTER TABLE OrderItems ADD CONSTRAINT FK_OrderItems_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId);
ALTER TABLE Payments ADD CONSTRAINT FK_Payments_Order FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE;
ALTER TABLE StudentProgress ADD CONSTRAINT FK_StudentProgress_User FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE;
ALTER TABLE StudentProgress ADD CONSTRAINT FK_StudentProgress_Lesson FOREIGN KEY (LessonId) REFERENCES Lessons(LessonId) ON DELETE CASCADE;
ALTER TABLE StudentProgress ADD CONSTRAINT FK_StudentProgress_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId);
ALTER TABLE CourseReviews ADD CONSTRAINT FK_CourseReviews_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE;
ALTER TABLE CourseReviews ADD CONSTRAINT FK_CourseReviews_User FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE;
ALTER TABLE Certificates ADD CONSTRAINT FK_Certificates_User FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE;
ALTER TABLE Certificates ADD CONSTRAINT FK_Certificates_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE;
ALTER TABLE Quizzes ADD CONSTRAINT FK_Quizzes_Lesson FOREIGN KEY (LessonId) REFERENCES Lessons(LessonId) ON DELETE CASCADE;
ALTER TABLE QuizQuestions ADD CONSTRAINT FK_QuizQuestions_Quiz FOREIGN KEY (QuizId) REFERENCES Quizzes(QuizId) ON DELETE CASCADE;
ALTER TABLE QuizAnswers ADD CONSTRAINT FK_QuizAnswers_Question FOREIGN KEY (QuestionId) REFERENCES QuizQuestions(QuestionId) ON DELETE CASCADE;
ALTER TABLE StudentQuizAttempts ADD CONSTRAINT FK_StudentQuizAttempts_User FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE;
ALTER TABLE StudentQuizAttempts ADD CONSTRAINT FK_StudentQuizAttempts_Quiz FOREIGN KEY (QuizId) REFERENCES Quizzes(QuizId) ON DELETE CASCADE;
ALTER TABLE Forums ADD CONSTRAINT FK_Forums_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE;
ALTER TABLE Forums ADD CONSTRAINT FK_Forums_User FOREIGN KEY (UserId) REFERENCES Users(UserId);
ALTER TABLE ForumReplies ADD CONSTRAINT FK_ForumReplies_Post FOREIGN KEY (PostId) REFERENCES Forums(PostId) ON DELETE CASCADE;
ALTER TABLE ForumReplies ADD CONSTRAINT FK_ForumReplies_User FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE NO ACTION;
ALTER TABLE ForumReplies ADD CONSTRAINT FK_ForumReplies_Parent FOREIGN KEY (ParentReplyId) REFERENCES ForumReplies(ReplyId) ON DELETE NO ACTION;
ALTER TABLE Coupons ADD CONSTRAINT FK_Coupons_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId);
ALTER TABLE Wishlist ADD CONSTRAINT FK_Wishlist_User FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE;
ALTER TABLE Wishlist ADD CONSTRAINT FK_Wishlist_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE;
ALTER TABLE Announcements ADD CONSTRAINT FK_Announcements_Author FOREIGN KEY (AuthorId) REFERENCES Users(UserId) ON DELETE NO ACTION;
GO

-- Create Indexes for Performance
CREATE INDEX IX_Courses_Category ON Courses(CategoryId);
CREATE INDEX IX_Courses_Instructor ON Courses(InstructorId);
CREATE INDEX IX_Courses_Published ON Courses(IsPublished, PublishedAt);
CREATE INDEX IX_Enrollments_User_Course ON Enrollments(UserId, CourseId);
CREATE INDEX IX_StudentProgress_User_Course ON StudentProgress(UserId, CourseId);
CREATE INDEX IX_CourseReviews_Course ON CourseReviews(CourseId);
CREATE INDEX IX_Lessons_Course_Order ON Lessons(CourseId, SortOrder);
GO

-- Insert Sample Data
-- Roles
INSERT INTO Roles (RoleName) VALUES 
('Admin'), 
('Instructor'), 
('Student');
GO

-- Categories
INSERT INTO Categories (Name, Description) VALUES 
('Technology', 'Programming, AI, Web Development'),
('Business', 'Marketing, Finance, Entrepreneurship'),
('Creative Arts', 'Design, Photography, Music'),
('Personal Development', 'Leadership, Communication, Productivity'),
('Science', 'Mathematics, Physics, Chemistry');
GO

-- Sample Users
INSERT INTO Users (Username, PasswordHash, Email, FullName, RoleId, Bio, EmailVerified) VALUES 
('admin_user', 'hashed_password_1', 'admin@courseralike.com', 'Admin User', 1, 'Platform Administrator', 1),
('john_instructor', 'hashed_password_2', 'john@courseralike.com', 'John Smith', 2, 'Senior Software Developer with 10+ years experience', 1),
('jane_student', 'hashed_password_3', 'jane@student.com', 'Jane Doe', 3, 'Computer Science Student', 1),
('mike_instructor', 'hashed_password_4', 'mike@courseralike.com', 'Mike Johnson', 2, 'Business Strategy Consultant', 1);
GO

-- Instructor Profiles
INSERT INTO InstructorProfiles (UserId, Headline, Biography, Website, YearsOfExperience) VALUES 
(2, 'Full-Stack Developer & Tech Educator', 'Passionate about teaching modern web development technologies. Author of 3 programming books.', 'https://johnsmith.dev', 10),
(4, 'Business Strategy & Digital Marketing Expert', 'Former Fortune 500 consultant helping businesses grow through digital transformation.', 'https://mikejohnson.biz', 15);
GO

-- Sample Courses
INSERT INTO Courses (Title, Subtitle, Description, InstructorId, CategoryId, Price, EstimatedDuration, IsPublished, DifficultyLevel) VALUES 
('Complete Web Development Bootcamp', 'Learn HTML, CSS, JavaScript, React, and Node.js', 'Master web development from basics to advanced concepts. Build real projects and deploy them.', 2, 1, 99.99, 40, 1, 'Beginner'),
('Digital Marketing Masterclass', 'SEO, Social Media, PPC & Analytics', 'Complete guide to digital marketing. Learn to create campaigns that convert and grow your business.', 4, 2, 79.99, 25, 1, 'Intermediate'),
('React.js Advanced Patterns', 'Advanced React concepts and patterns', 'Deep dive into advanced React patterns, performance optimization, and scalable architecture.', 2, 1, 149.99, 30, 1, 'Advanced');
GO

-- Sample Lessons
INSERT INTO Lessons (CourseId, Title, LessonType, Duration, SortOrder, IsPreview) VALUES 
(1, 'Introduction to Web Development', 'Video', 15, 1, 1),
(1, 'HTML Fundamentals', 'Video', 45, 2, 1),
(1, 'CSS Basics', 'Video', 60, 3, 0),
(1, 'JavaScript Introduction', 'Video', 90, 4, 0),
(2, 'Digital Marketing Overview', 'Video', 20, 1, 1),
(2, 'SEO Fundamentals', 'Video', 75, 2, 0),
(2, 'Social Media Strategy', 'Video', 60, 3, 0);
GO

-- Sample Enrollments
INSERT INTO Enrollments (UserId, CourseId, ProgressPercentage, PaymentStatus) VALUES 
(3, 1, 25.50, 'Paid'),
(3, 2, 0.00, 'Paid');
GO

-- Sample Reviews
INSERT INTO CourseReviews (CourseId, UserId, Rating, ReviewTitle, ReviewText) VALUES 
(1, 3, 5, 'Excellent Course!', 'This course exceeded my expectations. The instructor explains complex concepts very clearly.');
GO

-- Sample Announcements
INSERT INTO Announcements (Title, Content, AnnouncementType, AuthorId, IsFeatured) VALUES 
('New Course Launch: React Advanced Patterns', 'We are excited to announce our new advanced React course!', 'News', 1, 1),
('Black Friday Sale - 50% Off All Courses', 'Limited time offer! Get 50% off on all courses this Black Friday.', 'Promotion', 1, 1);
GO

PRINT 'Coursera-Style LMS Database created successfully!';
GO
