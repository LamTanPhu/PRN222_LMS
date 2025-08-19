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
